using FLandT_laba1_ver4.Domain;
using FLandT_laba1_ver4.Transliterator;
using System.Text;

namespace FLandT_laba1_ver4.LexicalMachine
{
    public sealed class DfaFirst_Switch
    {
        private enum S
        {
            S,      // ждём '0'
            A,      // ... '0'
            B,      // ... '01'
            C,      // ... '00'
            F,      // после "000" и после каждого "001"
            G,      // суффикс "001": прочли '0'
            H,      // суффикс "001": прочли "00"
            Error
        }

        private static bool IsBin(CharKind k) => k is CharKind.Zero or CharKind.One;
        private static bool IsAccept(S s) => s == S.F;

        private static S Next(S s, CharKind k) => s switch
        {
            S.S => k == CharKind.Zero ? S.A : S.Error,
            S.A => k == CharKind.One ? S.B : S.C,
            S.B => k == CharKind.One ? S.S : S.Error,
            S.C => k == CharKind.Zero ? S.F : S.Error,
            S.F => k == CharKind.Zero ? S.G : S.Error,
            S.G => k == CharKind.Zero ? S.H : S.Error,
            S.H => k == CharKind.One ? S.F : S.Error,
            _ => S.Error
        };

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

            if (!IsBin(first.Kind)) return false;

            var state = Next(S.S, first.Kind);
            if (state == S.Error) return false;

            sb.Append(first.Value);
            end = src.CurrentPos;

            while (true)
            {
                var sym = src.Read();

                if (sym.Kind == CharKind.EndOfText || !IsBin(sym.Kind))
                {
                    pushback = sym;
                    break;
                }

                var next = Next(state, sym.Kind);
                if (next == S.Error)
                {
                    pushback = sym;
                    break;
                }

                sb.Append(sym.Value);
                state = next;
                end = src.CurrentPos;
            }

            if (IsAccept(state))
            {
                lexeme = sb.ToString();
                return true;
            }

            lexeme = string.Empty;
            return false;
        }
    }
}