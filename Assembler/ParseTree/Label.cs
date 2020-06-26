namespace Caylang.Assembler.ParseTree
{
    public class Label : Statement
    {
        public Label(string name, int line) : base(line) => Name = name;
        
        public string Name { get; }

        public override string ToString()
        {
            return $"{GetType()} Name: {Name}";
        }
    }
}
