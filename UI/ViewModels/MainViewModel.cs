using FLandT_laba1_ver4.Common;
using FLandT_laba1_ver4.Domain;
using FLandT_laba1_ver4.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using LexRunner = FLandT_laba1_ver4.Lexer.Lexer;

namespace FLandT_laba1_ver4.UI.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
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

            RecognizedTokens.Clear();
            FirstCount = 0;
            SecondCount = 0;
            IsOk = true;
            ResultText = "Нажмите «Проверить», чтобы выполнить разбор.";
        }

        private void RunCore()
        {
            RecognizedTokens.Clear();
            FirstCount = 0;
            SecondCount = 0;
            IsOk = true;

            //LexRunner lexer = new (InputText);

            //while (lexer.NextToken(out var tok))
            //{
            //    if (tok.Type == TokenType.BinaryWord) FirstCount++;
            //    else if (tok.Type == TokenType.LetterWord) SecondCount++;

            //    if (tok.Type is TokenType.Whitespace or TokenType.Comment)
            //        continue;

            //    RecognizedTokens.Add(tok);
            //}
            var ts = new Parser.LexerTokenStream(InputText);
            var parser = new Parser.PredictiveParser(ts);
            var parsed = parser.ParseAll();

            IsOk = /*!lexer.SyntaxError &&*/ parsed && !ts.HadLexError;

            var sb = new StringBuilder();
            sb.AppendLine(IsOk ? "Статус: OK" : "Статус: Ошибка");
            sb.AppendLine($"(011)*000(001)*: {FirstCount}");
            sb.AppendLine("[a,b,c,d]+ (2-3=ac): " + SecondCount);

            if (!IsOk)
            {
                if (parser.HasError && !string.IsNullOrWhiteSpace(parser.Error))
                    sb.AppendLine(parser.Error);
                else if (/*lexer.SyntaxError ||*/ ts.HadLexError)
                    sb.AppendLine("Лексическая ошибка во входных данных.");
            }

            ResultText = sb.ToString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}