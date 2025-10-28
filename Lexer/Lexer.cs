using FLandT_laba1_ver4.Domain;
using FLandT_laba1_ver4.LexicalMachine;
using FLandT_laba1_ver4.Transliterator;

namespace FLandT_laba1_ver4.Lexer
{
    // Лексический анализатор читает посимвольно,
    // дергает два автомата (бин. и буквенный), собирает список токенов, считает типы.
    public sealed class Lexer
    {
        private readonly DfaFirst_Switch _dfaBin = new();
        private readonly DfaSecond_Table _dfaLet = new();

        public sealed class Result
        {
            public List<Token> Tokens { get; } = new();
            public bool IsOk { get; set; } = true;
            public string Message { get; set; } = "Статус: OK";
            public int FirstCount { get; set; }
            public int SecondCount { get; set; }
        }

        private CharSym? _hold;

        private CharSym Next(ICharSource src)
        {
            if (_hold is CharSym h) { _hold = null; return h; }
            return src.Read();
        }

        public Result Run(string input)
        {
            var src = new Transliterator.Transliterator(input);
            var res = new Result();
            int index = 1;

            while (true)
            {
                var startPos = src.CurrentPos;
                var sym = Next(src);

                if (sym.Kind == CharKind.EndOfText) break;

                if (sym.Kind is CharKind.Whitespace or CharKind.Newline) continue;

                if (sym.Kind == CharKind.Hash)
                {
                    while (true)
                    {
                        var s = src.Read();
                        if (s.Kind is CharKind.EndOfText or CharKind.Newline)
                        {
                            _hold = s;
                            break;
                        }
                    }
                    continue;
                }

                // Бинарный токен
                if (sym.Kind is CharKind.Zero or CharKind.One)
                {
                    if (!_dfaBin.TryRead(src, sym, startPos, out var lexeme, out var end, out var push))
                    {
                        res.IsOk = false;
                        res.Message = $"Ошибка: неверное бинарное слово в {startPos.Line}:{startPos.Column}.";
                        break;
                    }

                    res.Tokens.Add(new Token
                    {
                        Index = index++,
                        Value = lexeme,
                        Type = TokenType.BinaryWord,
                        Line = startPos.Line,
                        Column = startPos.Column
                    });
                    res.FirstCount++;

                    if (push is CharSym p) _hold = p;
                    continue;
                }

                // Буквенный токен
                if (sym.Kind is CharKind.A or CharKind.B or CharKind.C or CharKind.D)
                {
                    if (!_dfaLet.TryRead(src, sym, startPos, out var lexeme, out var end, out var push))
                    {
                        res.IsOk = false;
                        res.Message = $"Ошибка: неверное буквенное слово в {startPos.Line}:{startPos.Column}. " +
                                      $"Ожидалось: длина ≥ 3, 2-3 = 'ac'.";
                        break;
                    }

                    res.Tokens.Add(new Token
                    {
                        Index = index++,
                        Value = lexeme,
                        Type = TokenType.LetterWord,
                        Line = startPos.Line,
                        Column = startPos.Column
                    });
                    res.SecondCount++;

                    if (push is CharSym p) _hold = p;
                    continue;
                }

                // Любой прочий символ — ошибка алфавита
                {
                    res.IsOk = false;
                    res.Message = $"Ошибка: недопустимый символ '{sym.Value}' в {startPos.Line}:{startPos.Column}.";
                    break;
                }
            }

            return res;
        }
    }
}
