using FLandT_laba1_ver4.Domain;

namespace FLandT_laba1_ver4.Parser
{
    public sealed class LexerTokenStream : ITokenStream
    {
        private readonly Lexer.Lexer _lex;

        public LexerTokenStream(string input)
        {
            _lex = new Lexer.Lexer(input);
        }

        public bool HadLexError => _lex.SyntaxError;

        public Token? NextSignificant()
        {
            while (_lex.NextToken(out var t))
            {
                if (t.Type is TokenType.Whitespace or TokenType.Comment)
                    continue;
                return t;
            }
            return new Token() { Type = TokenType.EndOfText};
        }
    }
}