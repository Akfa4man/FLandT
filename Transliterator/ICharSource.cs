using FLandT_laba1_ver4.Domain;

namespace FLandT_laba1_ver4.Transliterator
{
    public interface ICharSource
    {
        CharSym Read();           // прочитать один символ (либо EOT)
        bool IsEndOfText { get; } // удобный флажок
        Position CurrentPos { get; } // текущая позиция
    }
}
