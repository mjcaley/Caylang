using System;

namespace Caylang.Assembler.ParseTree
{
    public interface IVisitor
    {   
        void Visit(Tree t);
        
        void Visit(FunctionNode f);
        void Visit(Definition d);

        void Visit(LabelStatement l);
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

    public class ParseTreeVisitor : IVisitor
    {
        public virtual void Visit(ParseNode p)
        {
            switch (p)
            {
                case Tree t:
                    Visit(t);
                    break;
                case FunctionNode f:
                    Visit(f);
                    break;
                case Definition d:
                    Visit(d);
                    break;
                case Statement s:
                    Visit(s);
                    break;
                case Literal l:
                    Visit(l);
                    break;
            };
        }

        public virtual void Visit(Tree t)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(FunctionNode f)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(Definition d)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(LabelStatement l)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(NullaryInstruction n)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(UnaryInstruction u)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(Integer8Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(Integer16Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(Integer32Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(Integer64Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(UnsignedInteger8Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(UnsignedInteger16Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(UnsignedInteger32Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(UnsignedInteger64Literal i)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(Float32Literal f)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(Float64Literal f)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(StringLiteral s)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(IdentifierLiteral i)
        {
            throw new NotImplementedException();
        }

        protected virtual void Visit(Statement s)
        {
            switch (s)
            {
                case LabelStatement l:
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

        protected virtual void Visit(Literal l)
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
                case UnsignedInteger8Literal u:
                    Visit(u);
                    break;
                case UnsignedInteger16Literal u:
                    Visit(u);
                    break;
                case UnsignedInteger32Literal u:
                    Visit(u);
                    break;
                case UnsignedInteger64Literal u:
                    Visit(u);
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
            };
        }

        public virtual void Visit(UnexpectedTokenError u)
        {
            throw new NotImplementedException();
        }
    }
}