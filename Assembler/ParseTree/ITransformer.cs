namespace Caylang.Assembler.ParseTree
{
    public interface ITransformer
    {
        Tree Transform(Tree t);
        
        Function Transform(Function f);
        Definition Transform(Definition d);

        LabelStatement Transform(LabelStatement l);
        NullaryInstruction Transform(NullaryInstruction n);
        UnaryInstruction Transform(UnaryInstruction u);
        
        Operand Transform(Operand i);

        IntegerLiteral Transform(IntegerLiteral i);
        FloatLiteral Transform(FloatLiteral f);
        StringLiteral Transform(StringLiteral s);
        IdentifierLiteral Transform(IdentifierLiteral i);
    }
}