using FLandT_laba1_ver4.Domain;
using FLandT_laba1_ver4.LexicalMachine;
using FLandT_laba1_ver4.Transliterator;

namespace FLandT_laba1_ver4.Lexer
{
    public sealed class Lexer
    {
        public bool SyntaxError { get; private set; }
        public string InputSnapshot { get; private set; } = string.Empty;

        private readonly DfaFirst_Switch _dfaBin = new();
        private readonly DfaSecond_Table _dfaLet = new();

        private readonly Transliterator.Transliterator _src;
        private CharSym? _hold;
        private int _indexCounter = 1;

        public Lexer(string input)
        {
            InputSnapshot = input ?? string.Empty;
            _src = new Transliterator.Transliterator(InputSnapshot);
            _hold = null;
            SyntaxError = false;
        }

        public bool NextToken(out Token token)
        {
            token = default!;

            CharSym Next()
            {
                if (_hold is CharSym h) { _hold = null; return h; }
                return _src.Read();
            }

            var startPos = _src.CurrentPos;
            var sym = Next();

            if (sym.Kind == CharKind.EndOfText) return false;

            // Пробельный символ -> свой токен (UI может игнорировать)
            if (sym.Kind is CharKind.Whitespace or CharKind.Newline)
            {
                token = new Token
                {
                    Index = _indexCounter++,
                    Value = sym.Value == '\n' ? "\\n" : sym.Value.ToString(),
                    Type = TokenType.Whitespace,
                    Line = startPos.Line,
                    Column = startPos.Column
                };
                return true;
            }

            // Комментарий "#... до \n/EOT" — БЕЗ while(true) в этом блоке
            if (sym.Kind == CharKind.Hash)
            {
                if (!TryReadCommentTail(_src, out var tail, out var boundary))
                {
                    SyntaxError = true;
                    return false;
                }

                _hold = boundary;

                token = new Token
                {
                    Index = _indexCounter++,
                    Value = "#" + tail,
                    Type = TokenType.Comment,
                    Line = startPos.Line,
                    Column = startPos.Column
                };
                return true;
            }

            // Бинарный токен
            if (sym.Kind is CharKind.Zero or CharKind.One)
            {
                if (!_dfaBin.TryRead(_src, sym, startPos, out var lexeme, out _, out var push))
                {
                    SyntaxError = true;
                    return false;
                }

                token = new Token
                {
                    Index = _indexCounter++,
                    Value = lexeme,
                    Type = TokenType.BinaryWord,
                    Line = startPos.Line,
                    Column = startPos.Column
                };
                if (push is CharSym p) _hold = p;
                return true;
            }

            // Буквенный токен
            if (sym.Kind is CharKind.A or CharKind.B or CharKind.C or CharKind.D)
            {
                if (!_dfaLet.TryRead(_src, sym, startPos, out var lexeme, out _, out var push))
                {
                    SyntaxError = true;
                    return false;
                }

                token = new Token
                {
                    Index = _indexCounter++,
                    Value = lexeme,
                    Type = TokenType.LetterWord,
                    Line = startPos.Line,
                    Column = startPos.Column
                };
                if (push is CharSym p) _hold = p;
                return true;
            }

            // Любой прочий символ — ошибка алфавита
            SyntaxError = true;
            return false;
        }

        private static bool TryReadCommentTail(
            ICharSource src,
            out string tail,
            out CharSym boundary)
        {
            var sb = new System.Text.StringBuilder();
            while (true)
            {
                var s = src.Read();
                if (s.Kind is CharKind.EndOfText or CharKind.Newline)
                {
                    boundary = s;
                    tail = sb.ToString();
                    return true;
                }
                sb.Append(s.Value);
            }
        }
    }
}