using FLandT_laba1_ver4.Domain;

namespace FLandT_laba1_ver4.Parser
{
    public sealed class PredictiveParser
    {
        private readonly ITokenStream _ts;
        private Token _la; // текущий

        public bool HasError { get; private set; }
        public string Error { get; private set; } = string.Empty;

        public PredictiveParser(ITokenStream ts) => _ts = ts;

        public bool ParseAll()
        {
            Next();
            if (!_C()) return Fail();

            //if (_la is not null)
            //    return Fail($"Ожидался конец ввода, а встретилось '{_la.Value}' в {_la.Line}:{_la.Column}");

            return true;
        }

        private bool _C()
        {
            //if (_la is null) return Fail("Неожиданный конец ввода в начале C");

            return _la.Type switch
            {
                TokenType.LBracket or TokenType.BinaryWord => _B(),
                TokenType.LParen or TokenType.LetterWord => _P(),
                _ => Fail(Expected("C", "'[' , (011)*... , '(' , [a..d]+..."))
            };
        }

        // B → [ P P ] | <1>
        private bool _B()
        {
            //if (_la is null) return Fail("Неожиданный конец ввода в B");

            if (_la.Type == TokenType.LBracket)
            {
                if (!ComparDesired(TokenType.LBracket)) return false;
                if (!_P()) return false;
                if (!_P()) return false;
                if (!ComparDesired(TokenType.RBracket)) return false;
                return true;
            }
            else if (_la.Type == TokenType.BinaryWord)
            {
                return ComparDesired(TokenType.BinaryWord);
            }

            return Fail(Expected("B", "'[' или (011)*000(001)*"));
        }

        // P → ( B B ) | <2>
        private bool _P()
        {
            //if (_la is null) return Fail("Неожиданный конец ввода в P");

            if (_la.Type == TokenType.LParen)
            {
                if (!ComparDesired(TokenType.LParen)) return false;
                if (!_B()) return false;
                if (!_B()) return false;
                if (!ComparDesired(TokenType.RParen)) return false;
                return true;
            }
            else if (_la.Type == TokenType.LetterWord)
            {
                return ComparDesired(TokenType.LetterWord);
            }

            return Fail(Expected("P", "'(' или [a..d]+ (2-3=ac)"));
        }

        private void Next() => _la = _ts.NextSignificant();

        private bool ComparDesired(TokenType t)
        {
            //if (_la is null) return Fail($"Ожидался {t}, но вход закончился.");
            if (_la.Type != t)
                return Fail($"Ожидался {t}, а встретился {_la.Type} в {_la.Line}:{_la.Column}");
            Next();
            return true;
        }

        private bool Fail(string msg = "Синтаксическая ошибка")
        {
            HasError = true;
            if (string.IsNullOrEmpty(Error)) Error = msg;
            return false;
        }

        private static string Expected(string nonterm, string what) => $"В {nonterm}: ожидалось {what}.";
    }
}