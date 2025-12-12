using System.Collections.Generic;
using FLandT_laba1_ver4.Domain;

namespace FLandT_laba1_ver4.Parser
{
    public sealed class UniqueIdentifierChecker : IAstVisitor
    {
        public bool HasError { get; private set; }
        public string Error { get; private set; } = string.Empty;

        public void Check(AstNode root)
        {
            HasError = false;
            Error = string.Empty;

            var ids = new HashSet<string>();

            CheckNode(root, ids);
        }


        private void CheckNode(AstNode node, HashSet<string> ids)
        {
            if (HasError || node is null)
                return;

            switch (node)
            {
                case C_BNode cB:
                    // C → B
                    CheckNode(cB.B, ids);
                    break;

                case C_PNode cP:
                    // C → P
                    CheckNode(cP.P, ids);
                    break;

                case BBracketNode bBr:
                    // B → [ P P ]
                    CheckNode(bBr.P1, ids);
                    if (!HasError)
                        CheckNode(bBr.P2, ids);
                    break;

                case BBinaryNode:
                    // B → <1>
                    break;

                case PParenNode pPar:
                    // P → ( B B )
                    CheckNode(pPar.B1, ids);
                    if (!HasError)
                        CheckNode(pPar.B2, ids);
                    break;

                case PLetterNode pLet:
                    // P → <2>
                    var name = pLet.Lexeme;
                    if (!ids.Add(name))
                    {
                        HasError = true;
                        if (string.IsNullOrEmpty(Error))
                        {
                            Error = $"Семантическая ошибка: идентификатор \"{name}\" " +
                                    $"использован повторно (позиция {pLet.Line}:{pLet.Column}).";
                        }
                    }
                    break;

                case LBracketNode:
                case RBracketNode:
                case LParenNode:
                case RParenNode:
                    break;

                default:
                    break;
            }
        }

        public void Visit(C_BNode n) { }

        public void Visit(C_PNode n) { }

        public void Visit(BBracketNode n) { }

        public void Visit(BBinaryNode n) { }

        public void Visit(PParenNode n) { }

        public void Visit(PLetterNode n) { }

        public void Visit(LBracketNode n) { }

        public void Visit(RBracketNode n) { }

        public void Visit(LParenNode n) { }

        public void Visit(RParenNode n) { }
    }
}