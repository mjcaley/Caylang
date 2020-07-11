using System.Collections;
using System.Collections.Generic;

namespace Caylang.Assembler.AST
{
    public abstract class ASTNode
    {
        public ASTNode(int line) => Line = line;
        
        public int Line { get; set; }
    }

    public class ASTLeaf : ASTNode
    {
        public ASTLeaf(int line) : base(line) {}
    }

    public class ASTChild : ASTNode
    {
        public ASTChild(ASTNode child, int line) : base(line) => Child = child;
        
        public ASTNode Child { get; set; }
    }

    public class ASTChildren : ASTNode
    {
        public ASTChildren(IEnumerable<ASTNode> children, int line) : base(line) => Children = children;
        
        public IEnumerable<ASTNode> Children { get; set; }
    }
}
