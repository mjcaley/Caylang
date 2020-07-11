using System.Collections.Generic;
using Caylang.Assembler.ParseTree;

namespace Caylang.Assembler.Passes
{
    public class GenerateSymbolTable : IVisitor
    {
        public void Visit(IEnumerable<Definition> d)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Literal l)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(FunctionDefinition f)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ConstantDefinition c)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(StructDefinition s)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Label l)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(NullaryInstruction n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(UnaryInstruction u)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Integer8Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Integer16Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Integer32Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Integer64Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(UnsignedInteger8Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(UnsignedInteger16Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(UnsignedInteger32Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(UnsignedInteger64Literal i)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Float32Literal f)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Float64Literal f)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(StringLiteral s)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(IdentifierLiteral i)
        {
            throw new System.NotImplementedException();
        }
    }
}