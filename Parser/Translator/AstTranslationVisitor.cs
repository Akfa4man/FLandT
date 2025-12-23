using FLandT_laba1_ver4.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLandT_laba1_ver4.Parser.Translator
{
    public sealed class AstTranslationVisitor : IAstVisitor
    {
        private readonly TranslationContext _ctx = new();

        public TranslationResult Translate(AstNode root)
        {
            _ctx.Output.Clear();
            _ctx.IndentLevel = 0;

            root.Accept(this);

            return new TranslationResult(_ctx.Output.ToString().TrimEnd());
        }

        // C -> B
        public void Visit(C_BNode n)
        {
            n.B.Accept(this);
        }

        // C -> P
        public void Visit(C_PNode n)
        {
            n.P.Accept(this);
        }

        // B -> [ P P ]
        public void Visit(BBracketNode n)
        {
            _ctx.Line("[");
            _ctx.IndentLevel++;

            n.P1.Accept(this);
            n.P2.Accept(this);

            _ctx.IndentLevel--;
            _ctx.Line("]");
        }

        // B -> <1>
        public void Visit(BBinaryNode n)
        {
            _ctx.Line(n.Lexeme);
        }

        // P -> ( B B )
        public void Visit(PParenNode n)
        {
            _ctx.Line("(");
            _ctx.IndentLevel++;

            n.B1.Accept(this);
            n.B2.Accept(this);

            _ctx.IndentLevel--;
            _ctx.Line(")");
        }

        // P -> <2>
        public void Visit(PLetterNode n)
        {
            _ctx.Line(n.Lexeme);
        }

        public void Visit(LBracketNode n) { }
        public void Visit(RBracketNode n) { }
        public void Visit(LParenNode n) { }
        public void Visit(RParenNode n) { }
    }
}
