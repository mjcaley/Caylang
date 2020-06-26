using System.Collections.Generic;

namespace Caylang.Assembler.ParseTree
{
    public interface IVisitor
    {
        void Visit(IEnumerable<Definition> d);

        void Visit(Literal l);

        void Visit(FunctionDefinition f);
        void Visit(ConstantDefinition c);
        void Visit(StructDefinition s);
        
        void Visit(Label l);
        void Visit(NullaryInstruction n);
        void Visit(UnaryInstruction u);

        void Visit(Integer8Literal i);
        void Visit(Integer16Literal i);
        void Visit(Integer32Literal i);
        void Visit(Integer64Literal i);
        void Visit(UnsignedInteger8Literal i);
        void Visit(UnsignedInteger16Literal i);
        void Visit(UnsignedInteger32Literal i);
        void Visit(UnsignedInteger64Literal i);
        void Visit(Float32Literal f);
        void Visit(Float64Literal f);
        void Visit(StringLiteral s);
        void Visit(IdentifierLiteral i);
    }
}
