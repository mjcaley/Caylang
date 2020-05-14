using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class LabelStatement : Statement
	{
		public LabelStatement(Token label) => Label = label;

		public Token Label { get; }
	}
}
