namespace Caylang.Assembler.AST
{
    public class ConstantDefinition : ASTChild
    {
        public string Name { get; set; }

        public ConstantDefinition(string name, ASTNode child, int line) : base(child, line) => Name = name;
    }
}