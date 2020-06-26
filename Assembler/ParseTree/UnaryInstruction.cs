namespace Caylang.Assembler.ParseTree
{
    public class UnaryInstruction : Statement
    {
        public UnaryInstruction(Instruction instruction, Literal operand, int line)
            : base(line) => (Instruction, Operand) = (instruction, operand);

        public Instruction Instruction { get; }

        public Literal Operand { get; }
    }
}
