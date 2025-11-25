using FLandT_laba1_ver4.Domain;

namespace FLandT_laba1_ver4.Parser
{
    public sealed class PredictiveParser
    {
        private readonly ITokenStream _ts;
        private Token _la; // текущий токен

        public bool HasError { get; private set; }
        public string Error { get; private set; } = string.Empty;

        // Корень синтаксического дерева (C_BNode или C_PNode)
        public AstNode? Root { get; private set; }

        public PredictiveParser(ITokenStream ts) => _ts = ts;

        public bool ParseAll()
        {
            HasError = false;
            Error = string.Empty;
            Root = null;

            Next();
            var c = _C();
            if (HasError) return false;

            if (!ComparDesired(TokenType.EndOfText)) return false;

            Root = c;
            return true;
        }

        // C → B | P
        private AstNode _C()
        {
            if (_la.Type == TokenType.LBracket || _la.Type == TokenType.BinaryWord)
            {
                // C → B
                return Ast.C_B(_B());
            }

            if (_la.Type == TokenType.LParen || _la.Type == TokenType.LetterWord)
            {
                // C → P
                return Ast.C_P(_P());
            }

            Fail(Expected("C", "'[' , (011)*... , '(' , [a..d]+..."));
            return Ast.C_B(Ast.B_Bin(string.Empty, _la.Line, _la.Column));
        }

        // B → [ P P ] | <1>
        private BNode _B()
        {
            if (_la.Type == TokenType.LBracket)
            {
                var lbrLine = _la.Line;
                var lbrCol = _la.Column;

                if (!ComparDesired(TokenType.LBracket))
                    return Ast.B_Bin(string.Empty, lbrLine, lbrCol);

                var lbrNode = Ast.LBr(lbrLine, lbrCol);

                var p1 = _P();
                if (HasError)
                    return Ast.B_Bin(string.Empty, lbrLine, lbrCol);

                var p2 = _P();
                if (HasError)
                    return Ast.B_Bin(string.Empty, lbrLine, lbrCol);

                var rbrLine = _la.Line;
                var rbrCol = _la.Column;

                if (!ComparDesired(TokenType.RBracket))
                    return Ast.B_Bin(string.Empty, lbrLine, lbrCol);

                var rbrNode = Ast.RBr(rbrLine, rbrCol);

                // B → [ P P ]
                return Ast.B_Of(lbrNode, p1, p2, rbrNode);
            }

            if (_la.Type == TokenType.BinaryWord)
            {
                var lex = _la.Value;
                var ln = _la.Line;
                var col = _la.Column;

                if (!ComparDesired(TokenType.BinaryWord))
                    return Ast.B_Bin(string.Empty, ln, col);

                // B → <1>
                return Ast.B_Bin(lex, ln, col);
            }

            Fail(Expected("B", "'[' или (011)*000(001)*"));
            return Ast.B_Bin(string.Empty, _la.Line, _la.Column);
        }

        // P → ( B B ) | <2>
        private PNode _P()
        {
            if (_la.Type == TokenType.LParen)
            {
                var lparLine = _la.Line;
                var lparCol = _la.Column;

                if (!ComparDesired(TokenType.LParen))
                    return Ast.P_Let(string.Empty, lparLine, lparCol);

                var lparNode = Ast.LPar(lparLine, lparCol);

                var b1 = _B();
                if (HasError)
                    return Ast.P_Let(string.Empty, lparLine, lparCol);

                var b2 = _B();
                if (HasError)
                    return Ast.P_Let(string.Empty, lparLine, lparCol);

                var rparLine = _la.Line;
                var rparCol = _la.Column;

                if (!ComparDesired(TokenType.RParen))
                    return Ast.P_Let(string.Empty, lparLine, lparCol);

                var rparNode = Ast.RPar(rparLine, rparCol);

                // P → ( B B )
                return Ast.P_Of(lparNode, b1, b2, rparNode);
            }

            if (_la.Type == TokenType.LetterWord)
            {
                var lex = _la.Value;
                var ln = _la.Line;
                var col = _la.Column;

                if (!ComparDesired(TokenType.LetterWord))
                    return Ast.P_Let(string.Empty, ln, col);

                // P → <2>
                return Ast.P_Let(lex, ln, col);
            }

            Fail(Expected("P", "'(' или [a..d]+ (2-3=ac)"));
            return Ast.P_Let(string.Empty, _la.Line, _la.Column);
        }

        private void Next() => _la = _ts.NextSignificant();

        private bool ComparDesired(TokenType t)
        {
            if (_la.Type != t)
                return Fail($"Ожидался {t}, а встретился {_la.Type} в {_la.Line}:{_la.Column}");

            Next();
            return true;
        }

        private bool Fail(string msg = "Синтаксическая ошибка")
        {
            HasError = true;
            if (string.IsNullOrEmpty(Error))
                Error = msg;
            return false;
        }

        private static string Expected(string nonterm, string what) =>
            $"В {nonterm}: ожидалось {what}.";
    }
}