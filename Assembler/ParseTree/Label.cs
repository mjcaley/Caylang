using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class LabelStatement : Statement
	{
		public LabelStatement(Token label, IEnumerable<ParseNode> children) : base(children) => Label = label;

		public LabelStatement(Token label, params ParseNode[] children) : base(children) => Label = label;

		public LabelStatement(Token label) => Label = label;

		public Token Label { get; }
	}
}
