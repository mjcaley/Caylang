using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Definition : ParseNode
	{
		public Definition(Token name, IEnumerable<ParseNode> children) : base(children) => Name = name;
		
		public Definition(Token name, params ParseNode[] children) : base(children) => Name = name;

		public Token Name { get; }
		public UnaryExpression? Value => Children.First() as UnaryExpression;
	}
}
