using FLandT_laba1_ver4.Common;
using FLandT_laba1_ver4.Domain;
using FLandT_laba1_ver4.Parser;
using FLandT_laba1_ver4.Parser.Translator;
using FLandT_laba1_ver4.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FLandT_laba1_ver4.UI.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        // Ввод пользователя
        private string _inputText = string.Empty;
        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText == value) return;
                _inputText = value;
                OnPropertyChanged();
            }
        }

        // Дерево разбора для TreeView
        public ObservableCollection<TreeItem> AstItems { get; } = new();

        // На всякий случай оставлен список токенов
        public ObservableCollection<Token> RecognizedTokens { get; } = new();

        private string _resultText = string.Empty;
        public string ResultText
        {
            get => _resultText;
            set { if (_resultText == value) return; _resultText = value; OnPropertyChanged(); }
        }

        private bool _isOk = true;
        public bool IsOk
        {
            get => _isOk;
            set { if (_isOk == value) return; _isOk = value; OnPropertyChanged(); }
        }

        private int _firstCount;
        public int FirstCount
        {
            get => _firstCount;
            set { if (_firstCount == value) return; _firstCount = value; OnPropertyChanged(); }
        }

        private int _secondCount;
        public int SecondCount
        {
            get => _secondCount;
            set { if (_secondCount == value) return; _secondCount = value; OnPropertyChanged(); }
        }

        public DelegateCommand RunCommand { get; }
        public DelegateCommand ClearCommand { get; }
        public DelegateCommand CopyReportCommand { get; }

        public MainViewModel()
        {
            RunCommand = new DelegateCommand(RunCore);
            ClearCommand = new DelegateCommand(Clear);
            CopyReportCommand = new DelegateCommand(() => ClipboardService.SetTextSafe(ResultText));

            ResultText = "Нажмите «Проверить», чтобы выполнить разбор.";
        }

        private void Clear()
        {
            _inputText = string.Empty;
            OnPropertyChanged(nameof(InputText));

            AstItems.Clear();
            RecognizedTokens.Clear();
            FirstCount = 0;
            SecondCount = 0;
            IsOk = true;
            ResultText = "Нажмите «Проверить», чтобы выполнить разбор.";
        }

        private void RunCore()
        {
            AstItems.Clear();
            FirstCount = 0;
            SecondCount = 0;
            IsOk = true;

            // Лексика → поток токенов
            var ts = new LexerTokenStream(InputText);

            //  Синтаксис → AST
            var parser = new PredictiveParser(ts);
            var parsed = parser.ParseAll();

            UniqueIdentifierChecker? sem = null;
            TranslationResult? trRes = null;

            if (parsed && parser.Root is not null)
            {

                var toTree = new AstToTreeVisitor();
                AstItems.Add(parser.Root.Accept(toTree));

                void Count(AstNode n, ref int bin, ref int let)
                {
                    switch (n)
                    {
                        case BBinaryNode:
                            bin++;
                            return;

                        case PLetterNode:
                            let++;
                            return;

                        case C_BNode cB:
                            Count(cB.B, ref bin, ref let);
                            return;

                        case C_PNode cP:
                            Count(cP.P, ref bin, ref let);
                            return;

                        case BBracketNode bBr:
                            Count(bBr.P1, ref bin, ref let);
                            Count(bBr.P2, ref bin, ref let);
                            return;

                        case PParenNode pPar:
                            Count(pPar.B1, ref bin, ref let);
                            Count(pPar.B2, ref bin, ref let);
                            return;

                        case LBracketNode:
                        case RBracketNode:
                        case LParenNode:
                        case RParenNode:
                            return;
                    }
                }

                int bin = 0, let = 0;
                Count(parser.Root, ref bin, ref let);
                FirstCount = bin;
                SecondCount = let;

                sem = new UniqueIdentifierChecker();
                sem.Check(parser.Root);

                if (!ts.HadLexError && !parser.HasError && !sem.HasError)
                {
                    var tr = new AstTranslationVisitor();
                    trRes = tr.Translate(parser.Root);
                }
            }

            // Итоговый статус
            IsOk = parsed
                   && !ts.HadLexError
                   && !parser.HasError
                   && !(sem?.HasError ?? false);

            // Отчёт
            var sb = new StringBuilder();
            sb.AppendLine(IsOk ? "Статус: OK" : "Статус: Ошибка");
            sb.AppendLine($"(011)*000(001)*: {FirstCount}");
            sb.AppendLine("[a,b,c,d]+ (2-3=ac): " + SecondCount);

            if (!IsOk)
            {
                if (ts.HadLexError)
                    sb.AppendLine("Лексическая ошибка во входных данных.");
                else if (parser.HasError && !string.IsNullOrWhiteSpace(parser.Error))
                    sb.AppendLine(parser.Error);
                else if (sem?.HasError == true && !string.IsNullOrWhiteSpace(sem.Error))
                    sb.AppendLine(sem.Error);
            }
            else if (trRes is not null)
            {
                sb.AppendLine();
                sb.AppendLine("Выходной текст:");
                sb.AppendLine(trRes.Output);
            }

            ResultText = sb.ToString();
        }

        private static void CountByAst(AstNode node, ref int bin, ref int let)
        {
            switch (node)
            {
                case BBinaryNode:
                    // B → <1>
                    bin++;
                    break;

                case PLetterNode:
                    // P → <2>
                    let++;
                    break;

                case C_BNode cB:
                    // C → B
                    CountByAst(cB.B, ref bin, ref let);
                    break;

                case C_PNode cP:
                    // C → P
                    CountByAst(cP.P, ref bin, ref let);
                    break;

                case BBracketNode bBr:
                    // B → [ P P ]
                    CountByAst(bBr.P1, ref bin, ref let);
                    CountByAst(bBr.P2, ref bin, ref let);
                    break;

                case PParenNode pPar:
                    // P → ( B B )
                    CountByAst(pPar.B1, ref bin, ref let);
                    CountByAst(pPar.B2, ref bin, ref let);
                    break;

                default:
                    break;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}