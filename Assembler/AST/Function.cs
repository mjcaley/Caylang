using System.Collections.Generic;

namespace Caylang.Assembler.AST
{
    public class FunctionDefinition : ASTChildren
    {
        public FunctionDefinition(string name, int locals, int arguments, IEnumerable<ASTNode> children, int line)
            : base(children, line)
            => (Name, Locals, Arguments) = (name, locals, arguments);
        
        public string Name { get; set; }
        
        public int Locals { get; set; }
        
        public int Arguments { get; set; }
    }
}