using System.Collections.Generic;

namespace Caylang.Assembler.AST
{
    public class Tree : ASTChildren
    {
        public Tree(IEnumerable<ASTNode> children, int line) : base(children, line) {}

        public Dictionary<string, Literal> Symbols { get; } = new Dictionary<string, Literal>();
    }
}
