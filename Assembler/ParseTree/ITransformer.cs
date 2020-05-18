namespace Caylang.Assembler.ParseTree
{
    public interface ITransformer
    {
        Tree Transform(Tree t);
        
        FunctionNode Transform(FunctionNode f);
        Definition Transform(Definition d);

        LabelStatement Transform(LabelStatement l);
        NullaryInstruction Transform(NullaryInstruction n);
        UnaryInstruction Transform(UnaryInstruction u);

        UnaryExpression Transform(UnaryExpression u);

        Literal Transform(Literal l);
        Integer8Literal Transform(Integer8Literal i);
        Integer16Literal Transform(Integer16Literal i);
        Integer32Literal Transform(Integer32Literal i);
        Integer64Literal Transform(Integer64Literal i);
        UnsignedInteger8Literal Transform(UnsignedInteger8Literal i);
        UnsignedInteger16Literal Transform(UnsignedInteger16Literal i);
        UnsignedInteger32Literal Transform(UnsignedInteger32Literal i);
        UnsignedInteger64Literal Transform(UnsignedInteger64Literal i);
        Float32Literal Transform(Float32Literal f);
        Float64Literal Transform(Float64Literal f);
        StringLiteral Transform(StringLiteral s);
        IdentifierLiteral Transform(IdentifierLiteral i);
    }
}