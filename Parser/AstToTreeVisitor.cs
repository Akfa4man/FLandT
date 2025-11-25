using FLandT_laba1_ver4.Domain;
using FLandT_laba1_ver4.UI;

namespace FLandT_laba1_ver4.Parser
{
    public sealed class AstToTreeVisitor : IAstVisitor<TreeItem>
    {
        public TreeItem Visit(C_BNode n)
        {
            // C → B
            var root = new TreeItem("C → B");
            root.Add(n.B.Accept(this));
            return root;
        }

        public TreeItem Visit(C_PNode n)
        {
            // C → P
            var root = new TreeItem("C → P");
            root.Add(n.P.Accept(this));
            return root;
        }

        public TreeItem Visit(BBracketNode n)
        {
            // B → [ P P ]
            var t = new TreeItem("B → [ P P ]");

            t.Add(n.LBracket.Accept(this)); 
            t.Add(n.P1.Accept(this));
            t.Add(n.P2.Accept(this));  
            t.Add(n.RBracket.Accept(this)); 

            return t;
        }

        public TreeItem Visit(BBinaryNode n)
        {
            // B → <1>
            return new TreeItem($"B → <1>   \"{n.Lexeme}\"  @{n.Line}:{n.Column}");
        }

        public TreeItem Visit(PParenNode n)
        {
            // P → ( B B )
            var t = new TreeItem("P → ( B B )");

            t.Add(n.LParen.Accept(this)); 
            t.Add(n.B1.Accept(this));    
            t.Add(n.B2.Accept(this));    
            t.Add(n.RParen.Accept(this)); 

            return t;
        }

        public TreeItem Visit(PLetterNode n)
        {
            // P → <2>
            return new TreeItem($"P → <2>   \"{n.Lexeme}\"  @{n.Line}:{n.Column}");
        }

        public TreeItem Visit(LBracketNode n)
        {
            return new TreeItem($"'[' @{n.Line}:{n.Column}");
        }

        public TreeItem Visit(RBracketNode n)
        {
            return new TreeItem($"']' @{n.Line}:{n.Column}");
        }

        public TreeItem Visit(LParenNode n)
        {
            return new TreeItem($"'(' @{n.Line}:{n.Column}");
        }

        public TreeItem Visit(RParenNode n)
        {
            return new TreeItem($"')' @{n.Line}:{n.Column}");
        }
    }
}