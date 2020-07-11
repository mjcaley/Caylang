namespace Caylang.Assembler.AST
{
    public class LabelStatement : ASTLeaf
    {
        public LabelStatement(string name, int line) : base(line) => Name = name;
        
        public LabelStatement(string name, int position, int line) : base(line) => (Name, Position) = (name, position);
        
        public string Name { get; set; }
        
        public int? Position { get; set; }
    }
}
