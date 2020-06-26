namespace Caylang.Assembler.ParseTree
{
    public class ConstantDefinition : Definition
    {
        public ConstantDefinition(string name, Literal value, int line) : base(line) => (Name, Value) = (name, value);

        public string Name { get; }
        
        public Literal Value { get; }

        public override string ToString()
        {
            return $"{GetType()} Name: {Name}";
        }
    }
}
