using FLandT_laba1_ver4.Domain;

namespace FLandT_laba1_ver4.Transliterator
{
    public sealed class Transliterator : ICharSource
    {
        private readonly string _text;
        private int _index;
        private int _line = 1;
        private int _col = 0; // сколько символов уже прочитано в текущей строке

        public Transliterator(string text)
        {
            _text = text ?? string.Empty;
            _index = 0; _line = 1; _col = 0;
        }

        public bool IsEndOfText => _index >= _text.Length;

        public CharSym Read()
        {
            if (IsEndOfText) return new CharSym('\0', CharKind.EndOfText);

            char ch = _text[_index++];

            // CR/LF -> '\n'
            if (ch == '\r')
            {
                if (!IsEndOfText && _text[_index] == '\n') _index++; // проглотили LF
                _line++; _col = 0;
                return new CharSym('\n', CharKind.Newline);
            }
            if (ch == '\n')
            {
                _line++; _col = 0;
                return new CharSym('\n', CharKind.Newline);
            }

            _col++;
            return new CharSym(ch, Classify(ch));
        }

        public Position CurrentPos => new Position(_line, Math.Max(_col, 1));

        private static CharKind Classify(char ch) => ch switch
        {
            '0' => CharKind.Zero,
            '1' => CharKind.One,
            'a' => CharKind.A,
            'b' => CharKind.B,
            'c' => CharKind.C,
            'd' => CharKind.D,
            '#' => CharKind.Hash,
            '\t' or ' ' => CharKind.Whitespace,
            '\n' => CharKind.Newline, // сюда почти не попадём (мы уже нормализуем выше)
            _ => char.IsWhiteSpace(ch) ? CharKind.Whitespace : CharKind.Other
        };
    }
}
