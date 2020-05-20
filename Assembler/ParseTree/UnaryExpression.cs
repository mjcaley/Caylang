using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Caylang.Assembler.ParseTree
{
    public class UnaryExpression : ParseNode
    {
        public UnaryExpression(Token? op, IEnumerable<ParseNode> children) : base(children) => Operator = op;

        public UnaryExpression(Token? op, params ParseNode[] children) : base(children) => Operator = op;

        public Token? Operator { get; }
        
        public Literal? Expression => Children.First() as Literal;
    }
}
