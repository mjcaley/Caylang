namespace Caylang.Assembler.ParseTree
{
    public class NullaryInstruction : Statement
    {
        public NullaryInstruction(Instruction instruction, int line)
            : base(line) => Instruction = instruction;
        
        public Instruction Instruction { get; }

        public override string ToString()
        {
            return $"{GetType()} Instruction: {Instruction}";
        }
    }
}
