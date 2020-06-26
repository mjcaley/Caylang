using Caylang.Assembler.ParseTree;
using System;
using System.Collections.Generic;

namespace Caylang.Assembler.Passes
{
    public class DefaultVisitor : IVisitor
    {
        public virtual void Visit(IEnumerable<Definition> d)
        {
            foreach (var definition in d)
            {
                switch (definition)
                {
                    case StructDefinition s:
                        Visit(s);
                        break;
                    case FunctionDefinition f:
                        Visit(f);
                        break;
                    case ConstantDefinition c:
                        Visit(c);
                        break;
                }
            }
        }

        public virtual void Visit(Literal l)
        {
            switch (l)
            {
                case Integer8Literal i:
                    Visit(i);
                    break;
                case Integer16Literal i:
                    Visit(i);
                    break;
                case Integer32Literal i:
                    Visit(i);
                    break;
                case Integer64Literal i:
                    Visit(i);
                    break;
                case UnsignedInteger8Literal i:
                    Visit(i);
                    break;
                case UnsignedInteger16Literal i:
                    Visit(i);
                    break;
                case UnsignedInteger32Literal i:
                    Visit(i);
                    break;
                case UnsignedInteger64Literal i:
                    Visit(i);
                    break;
                case Float32Literal f:
                    Visit(f);
                    break;
                case Float64Literal f:
                    Visit(f);
                    break;
                case StringLiteral s:
                    Visit(s);
                    break;
                case IdentifierLiteral i:
                    Visit(i);
                    break;
            }
        }

        public virtual void Visit(FunctionDefinition f)
        {
            foreach (var statement in f.Statements)
            {
                switch (statement)
                {
                    case Label l:
                        Visit(l);
                        break;
                    case NullaryInstruction n:
                        Visit(n);
                        break;
                    case UnaryInstruction u:
                        Visit(u);
                        break;
                }
            }
        }

        public virtual void Visit(ConstantDefinition c)
        {
            Visit(c.Value);
        }

        public virtual void Visit(StructDefinition s) { }

        public virtual void Visit(Label l) { }

        public virtual void Visit(NullaryInstruction n) { }

        public virtual void Visit(UnaryInstruction u)
        {
            Visit(u.Operand);
        }

        public virtual void Visit(Integer8Literal i) { }

        public virtual void Visit(Integer16Literal i) { }

        public virtual void Visit(Integer32Literal i) { }

        public virtual void Visit(Integer64Literal i) { }

        public virtual void Visit(UnsignedInteger8Literal i) { }

        public virtual void Visit(UnsignedInteger16Literal i) { }

        public virtual void Visit(UnsignedInteger32Literal i) { }

        public virtual void Visit(UnsignedInteger64Literal i) { }

        public virtual void Visit(Float32Literal f) { }

        public virtual void Visit(Float64Literal f) { }

        public virtual void Visit(StringLiteral s) { }

        public virtual void Visit(IdentifierLiteral i) { }
    }
}
