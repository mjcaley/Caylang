using Caylang.Assembler.ParseTree;
using Yoakke.Lexer;

namespace Caylang.Assembler
{
    public interface IVisitor
    {
        void Visit(Module node);
        void Visit(Definition node);
        void Visit(Struct node);
        void Visit(Types node);
        void Visit(ArgsParameter node);
        void Visit(LocalsParameter node);
        void Visit(FunctionParameters node);
        void Visit(Function node);
        void Visit(Statements node);
        void Visit(NullaryInstruction node);
        void Visit(UnaryInstruction node);
        void Visit(Label node);
        void Visit(Number node);

        void Visit(Keyword node);
        void Visit(Type node);
        void Visit(Instruction node);
        void Visit(Sign node);
        void Visit(Identifier node);
        void Visit(Integer node);
        void Visit(Float node);
        void Visit(String node);
    }

    public class DefaultVisitor : IVisitor
    {
        public void Visit(Node node)
        {
            switch (node)
            {
                case Branch b:
                    Visit(b);
                    break;
                case Leaf l:
                    Visit(l);
                    break;
            }
        }

        public virtual void Visit(Branch node)
        {
            foreach (var child in node.Children)
            {
                switch (child)
                {
                    case Module m:
                        Visit(m);
                        break;
                    case Definition d:
                        Visit(d);
                        break;
                    case Struct s:
                        Visit(s);
                        break;
                    case Types t:
                        Visit(t);
                        break;
                    case ArgsParameter a:
                        Visit(a);
                        break;
                    case LocalsParameter l:
                        Visit(l);
                        break;
                    case FunctionParameters f:
                        Visit(f);
                        break;
                    case Function f:
                        Visit(f);
                        break;
                    case Statements s:
                        Visit(s);
                        break;
                    case NullaryInstruction n:
                        Visit(n);
                        break;
                    case UnaryInstruction u:
                        Visit(u);
                        break;
                    case Label l:
                        Visit(l);
                        break;
                    case Number n:
                        Visit(n);
                        break;
                    case Keyword k:
                        Visit(k);
                        break;
                    case Type t:
                        Visit(t);
                        break;
                    case Instruction i:
                        Visit(i);
                        break;
                    case Sign s:
                        Visit(s);
                        break;
                    case Identifier i:
                        Visit(i);
                        break;
                    case Integer i:
                        Visit(i);
                        break;
                    case Float f:
                        Visit(f);
                        break;
                    case String s:
                        Visit(s);
                        break;
                }
            }
        }

        public virtual void Visit(Module node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(Definition node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(Struct node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(Types node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(ArgsParameter node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(LocalsParameter node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(FunctionParameters node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(Function node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(Statements node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(NullaryInstruction node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(UnaryInstruction node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(Label node)
        {
            Visit((Branch)node);
        }

        public virtual void Visit(Number node)
        {
            Visit((Branch)node);
        }

        #region Leaves
        public virtual void Visit(Leaf node) { }
        public virtual void Visit(Keyword node)
        {
            Visit((Leaf)node);
        }

        public virtual void Visit(Type node)
        {
            Visit((Leaf)node);
        }

        public virtual void Visit(Instruction node)
        {
            Visit((Leaf)node);
        }

        public virtual void Visit(Sign node)
        {
            Visit((Leaf)node);
        }

        public virtual void Visit(Identifier node)
        {
            Visit((Leaf)node);
        }

        public virtual void Visit(Integer node)
        {
            Visit((Leaf)node);
        }

        public virtual void Visit(Float node)
        {
            Visit((Leaf)node);
        }

        public virtual void Visit(String node)
        {
            Visit((Leaf)node);
        }
        #endregion

        public virtual void Visit(IToken<TokenType> token) { }
    }
}
