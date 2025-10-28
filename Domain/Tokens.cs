namespace FLandT_laba1_ver4.Domain
{
    // Unknown — на случай, если захотим явно сигналить про непонятные штуки.
    public enum TokenType { BinaryWord, LetterWord, Comment, Whitespace, Unknown }

    // Позиция в тексте.
    public sealed class Position
    {
        public int Line { get; }
        public int Column { get; }

        public Position(int line, int column) { Line = line; Column = column; }

        public override string ToString() => $"({Line},{Column})";
    }

    //Фиксированная точка.
    public sealed class Token
    {
        public int Index { get; set; }

        public string Value { get; set; } = string.Empty;

        public TokenType Type { get; set; }

        public string TypeName => Type switch
        {
            TokenType.BinaryWord => "(011)*000(001)*",
            TokenType.LetterWord => "[a,b,c,d]+ (2-3=ac)",
            _ => Type.ToString()
        };

        public int Line { get; set; }
        public int Column { get; set; }
    }
}
