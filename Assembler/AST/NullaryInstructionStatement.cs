namespace Caylang.Assembler.AST
{
    public enum NullaryInstruction
    {
        Halt,
        Pop,
        Noop,
        Return,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        TestEqual,
        TestNotEqual,
        TestGreaterThan,
        TestLessThan,
        CallVirtual,
        LoadElement,
        StoreElement
    }

    public class NullaryInstructionStatement : ASTLeaf
    {
        public NullaryInstruction Instruction { get; set; }

        public NullaryInstructionStatement(NullaryInstruction instruction, int line) : base(line) => Instruction = instruction;
    }
}