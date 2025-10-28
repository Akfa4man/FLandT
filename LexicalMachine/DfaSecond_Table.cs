using FLandT_laba1_ver4.Domain;
using FLandT_laba1_ver4.Transliterator;
using System.Text;

namespace FLandT_laba1_ver4.LexicalMachine
{
    public sealed class DfaSecond_Table
    {
        // Состояния: S (start), A (есть 1-й), C (вторая = 'a'), T (после 'ac' — accept)
        private enum Q { S, A, C, T, Error }

        // Допускающее — только T
        private static readonly HashSet<Q> Accepting = new() { Q.T };

        // Таблица переходов: (state, CharKind) -> next
        private static readonly Dictionary<(Q, CharKind), Q> T = new()
        {
            // S: первая буква любая из a..d
            {(Q.S, CharKind.A), Q.A}, {(Q.S, CharKind.B), Q.A},
            {(Q.S, CharKind.C), Q.A}, {(Q.S, CharKind.D), Q.A},

            // A: вторая строго 'a'
            {(Q.A, CharKind.A), Q.C},

            // C: третья строго 'c'
            {(Q.C, CharKind.C), Q.T},

            // T: после "...ac" любые a..d остаются в T
            {(Q.T, CharKind.A), Q.T},
            {(Q.T, CharKind.B), Q.T},
            {(Q.T, CharKind.C), Q.T},
            {(Q.T, CharKind.D), Q.T},
        };

        private static bool IsLetter(CharKind k) =>
            k is CharKind.A or CharKind.B or CharKind.C or CharKind.D;

        public bool TryRead(
            ICharSource src,
            CharSym first,
            Position start,
            out string lexeme,
            out Position end,
            out CharSym? pushback)
        {
            var sb = new StringBuilder();
            end = start;
            pushback = null;
            lexeme = string.Empty;

            if (!IsLetter(first.Kind)) return false;
            if (!T.TryGetValue((Q.S, first.Kind), out var stateFirst)) return false;

            sb.Append(first.Value);
            Q state = stateFirst;
            end = src.CurrentPos;

            while (true)
            {
                var sym = src.Read();

                if (sym.Kind == CharKind.EndOfText || !IsLetter(sym.Kind))
                {
                    pushback = sym;
                    break;
                }

                if (!T.TryGetValue((state, sym.Kind), out var next))
                {
                    pushback = sym;
                    break;
                }

                sb.Append(sym.Value);
                state = next;
                end = src.CurrentPos;
            }

            if (Accepting.Contains(state))
            {
                lexeme = sb.ToString();
                return true;
            }

            lexeme = string.Empty;
            return false;
        }
    }
}