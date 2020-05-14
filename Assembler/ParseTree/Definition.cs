using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Definition : ParseNode
	{
		public Definition(Token name, ParseNode value) : base(value) => Name = name;

		public Token Name { get; }
		public Literal? Value => Children.First() as Literal;
	}
}
