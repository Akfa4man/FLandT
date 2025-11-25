namespace FLandT_laba1_ver4.Domain
{
    public enum AstKind
    {
        C_B,        // C → B
        C_P,        // C → P

        B_Bracket,  // B → [ P P ]
        B_Binary,   // B → <1>

        P_Paren,    // P → ( B B )
        P_Letter,   // P → <2>

        LBracket,
        RBracket,
        LParen,
        RParen
    }

    // Базовый для всех узлов дерева
    public abstract record AstNode
    {
        public abstract AstKind Kind { get; }

        public abstract void Accept(IAstVisitor visitor);
        public abstract T Accept<T>(IAstVisitor<T> visitor);
    }
    public abstract record BNode : AstNode { }
    public abstract record PNode : AstNode { }

    public interface IAstVisitor
    {
        void Visit(C_BNode n);
        void Visit(C_PNode n);

        void Visit(BBracketNode n);
        void Visit(BBinaryNode n);

        void Visit(PParenNode n);
        void Visit(PLetterNode n);

        void Visit(LBracketNode n);
        void Visit(RBracketNode n);
        void Visit(LParenNode n);
        void Visit(RParenNode n);
    }

    public interface IAstVisitor<T>
    {
        T Visit(C_BNode n);
        T Visit(C_PNode n);

        T Visit(BBracketNode n);
        T Visit(BBinaryNode n);

        T Visit(PParenNode n);
        T Visit(PLetterNode n);

        T Visit(LBracketNode n);
        T Visit(RBracketNode n);
        T Visit(LParenNode n);
        T Visit(RParenNode n);
    }

    // C → B
    public sealed record C_BNode(BNode B) : AstNode
    {
        public override AstKind Kind => AstKind.C_B;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    // C → P
    public sealed record C_PNode(PNode P) : AstNode
    {
        public override AstKind Kind => AstKind.C_P;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    // B → [ P P ]
    public sealed record BBracketNode(
        LBracketNode LBracket,
        PNode P1,
        PNode P2,
        RBracketNode RBracket) : BNode
    {
        public override AstKind Kind => AstKind.B_Bracket;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    // B → <1>
    public sealed record BBinaryNode(string Lexeme, int Line, int Column) : BNode
    {
        public override AstKind Kind => AstKind.B_Binary;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    // P → ( B B )
    public sealed record PParenNode(
        LParenNode LParen,
        BNode B1,
        BNode B2,
        RParenNode RParen) : PNode
    {
        public override AstKind Kind => AstKind.P_Paren;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    // P → <2>
    public sealed record PLetterNode(string Lexeme, int Line, int Column) : PNode
    {
        public override AstKind Kind => AstKind.P_Letter;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    public sealed record LBracketNode(int Line, int Column) : AstNode
    {
        public override AstKind Kind => AstKind.LBracket;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    public sealed record RBracketNode(int Line, int Column) : AstNode
    {
        public override AstKind Kind => AstKind.RBracket;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    public sealed record LParenNode(int Line, int Column) : AstNode
    {
        public override AstKind Kind => AstKind.LParen;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    public sealed record RParenNode(int Line, int Column) : AstNode
    {
        public override AstKind Kind => AstKind.RParen;
        public override void Accept(IAstVisitor v) => v.Visit(this);
        public override T Accept<T>(IAstVisitor<T> v) => v.Visit(this);
    }

    public static class Ast
    {
        // Для C
        public static C_BNode C_B(BNode body) => new(body);   // C → B
        public static C_PNode C_P(PNode body) => new(body);   // C → P

        // Для B
        public static BBracketNode B_Of(
            LBracketNode lbr,
            PNode p1,
            PNode p2,
            RBracketNode rbr) => new(lbr, p1, p2, rbr);

        public static BBinaryNode B_Bin(string lex, int line, int col)
            => new(lex, line, col);

        // Для P
        public static PParenNode P_Of(
            LParenNode lpar,
            BNode b1,
            BNode b2,
            RParenNode rpar) => new(lpar, b1, b2, rpar);

        public static PLetterNode P_Let(string lex, int line, int col)
            => new(lex, line, col);
        public static LBracketNode LBr(int line, int col) => new(line, col);
        public static RBracketNode RBr(int line, int col) => new(line, col);
        public static LParenNode LPar(int line, int col) => new(line, col);
        public static RParenNode RPar(int line, int col) => new(line, col);
    }
}