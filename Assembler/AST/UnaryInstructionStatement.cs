namespace Caylang.Assembler.AST
{
    public enum UnaryInstruction
    {
        LoadConst,
        LoadLocal,
        StoreLocal,
        Jump,
        JumpTrue,
        JumpFalse,
        CallFunc,
        NewStruct,
        LoadField,
        StoreField,
        NewArray
    }
    
    public class UnaryInstructionStatement : ASTChild
    {
        public UnaryInstruction Instruction { get; set; }

        public UnaryInstructionStatement(UnaryInstruction instruction, ASTNode child, int line)
            : base(child, line)
            => Instruction = instruction;
    }
}