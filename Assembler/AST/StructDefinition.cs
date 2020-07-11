using System.Collections.Generic;

namespace Caylang.Assembler.AST
{
    public class StructDefinition : ASTChildren
    {
        public string Name { get; set; }

        public StructDefinition(string name, IEnumerable<ASTNode> children, int line)
            : base(children, line)
            => Name = name;
    }
}
