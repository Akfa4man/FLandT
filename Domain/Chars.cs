namespace FLandT_laba1_ver4.Domain
{
    public enum CharKind
    {
        Zero, One,
        A, B, C, D,
        Hash,
        Newline,
        Whitespace,
        Other,
        EndOfText
    }

    public readonly record struct CharSym(char Value, CharKind Kind);
}
