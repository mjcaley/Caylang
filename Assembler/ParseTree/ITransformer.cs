using System.Collections;
using System.Collections.Generic;

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

    public class ParseTreeTransformer : ITransformer
    {
        public IEnumerable<ParseNode> Transform(IEnumerable<ParseNode> children)
        {
            var newChildren = new List<ParseNode>();
            foreach (var child in children)
            {
                newChildren.Add(Transform(child));
            }

            return newChildren;
        }

        public virtual ParseNode Transform(ParseNode p) =>
            p switch
            {
                Tree t => Transform(t),
                FunctionNode f => Transform(f),
                Definition d => Transform(d),
                Statement s => Transform(s),
                UnaryExpression u => Transform(u),
                Literal l => Transform(l),
                _ => p
            };

        public virtual Tree Transform(Tree t)
        {
            return new Tree(Transform(t.Children));
        }

        public virtual FunctionNode Transform(FunctionNode f)
        {
            return new FunctionNode(f.Name, f.Locals, f.Arguments, Transform(f.Children));
        }

        public virtual Definition Transform(Definition d)
        {
            return new Definition(d.Name, Transform(d.Children));
        }

        public virtual Statement Transform(Statement s) =>
            s switch
            {
                LabelStatement l => Transform(l),
                NullaryInstruction n => Transform(n),
                UnaryInstruction u => Transform(u),
                _ => s
            };

        public LabelStatement Transform(LabelStatement l)
        {
            return new LabelStatement(l.Label, Transform(l.Children));
        }

        public NullaryInstruction Transform(NullaryInstruction n)
        {
            return new NullaryInstruction(n.Instruction, n.ReturnType, Transform(n.Children));
        }

        public UnaryInstruction Transform(UnaryInstruction u)
        {
            return new UnaryInstruction(u.Instruction, u.ReturnType, Transform(u.Children));
        }

        public UnaryExpression Transform(UnaryExpression u)
        {
            return new UnaryExpression(u.Operator, Transform(u.Children));
        }

        public Literal Transform(Literal l) =>
            l switch
            {
                Integer8Literal i => Transform(i),
                Integer16Literal i => Transform(i),
                Integer32Literal i => Transform(i),
                Integer64Literal i => Transform(i),
                UnsignedInteger8Literal i => Transform(i),
                UnsignedInteger16Literal i => Transform(i),
                UnsignedInteger32Literal i => Transform(i),
                UnsignedInteger64Literal i => Transform(i),
                Float32Literal f => Transform(f),
                Float64Literal f => Transform(f),
                StringLiteral s => Transform(s),
                IdentifierLiteral i => Transform(i),
                _ => l
            };

        public Integer8Literal Transform(Integer8Literal i)
        {
            return new Integer8Literal(i.Atom, Transform(i.Children));
        }

        public Integer16Literal Transform(Integer16Literal i)
        {
            return new Integer16Literal(i.Atom, Transform(i.Children));
        }

        public Integer32Literal Transform(Integer32Literal i)
        {
            return new Integer32Literal(i.Atom, Transform(i.Children));
        }

        public Integer64Literal Transform(Integer64Literal i)
        {
            return new Integer64Literal(i.Atom, Transform(i.Children));
        }

        public UnsignedInteger8Literal Transform(UnsignedInteger8Literal i)
        {
            return new UnsignedInteger8Literal(i.Atom, Transform(i.Children));
        }

        public UnsignedInteger16Literal Transform(UnsignedInteger16Literal i)
        {
            return new UnsignedInteger16Literal(i.Atom, Transform(i.Children));
        }

        public UnsignedInteger32Literal Transform(UnsignedInteger32Literal i)
        {
            return new UnsignedInteger32Literal(i.Atom, Transform(i.Children));
        }

        public UnsignedInteger64Literal Transform(UnsignedInteger64Literal i)
        {
            return new UnsignedInteger64Literal(i.Atom, Transform(i.Children));
        }

        public Float32Literal Transform(Float32Literal f)
        {
            return new Float32Literal(f.Atom, Transform(f.Children));
        }

        public Float64Literal Transform(Float64Literal f)
        {
            return new Float64Literal(f.Atom, Transform(f.Children));
        }

        public StringLiteral Transform(StringLiteral s)
        {
            return new StringLiteral(s.Atom, Transform(s.Children));
        }

        public IdentifierLiteral Transform(IdentifierLiteral i)
        {
            return new IdentifierLiteral(i.Atom, Transform(i.Children));
        }
    }
}