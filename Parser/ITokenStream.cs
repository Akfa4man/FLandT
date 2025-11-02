using FLandT_laba1_ver4.Domain;

namespace FLandT_laba1_ver4.Parser
{
    public interface ITokenStream
    {
        Token? NextSignificant();
        bool HadLexError { get; }
    }
}