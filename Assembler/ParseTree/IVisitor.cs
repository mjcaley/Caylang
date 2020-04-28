using System;

namespace Caylang.Assembler.ParseTree
{
    public interface IVisitor
    {
        void Visit(Tree t);
        
        void Visit(Function f);
        void Visit(Definition d);

        void Visit(LabelStatement l);
        void Visit(NullaryInstruction n);
        void Visit(UnaryInstruction u);
        
        void Visit(Operand i);

        void Visit(IntegerLiteral i);
        void Visit(FloatLiteral f);
        void Visit(StringLiteral s);
        void Visit(IdentifierLiteral i);
    }
}