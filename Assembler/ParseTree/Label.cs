using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class LabelStatement : Statement
	{
		public LabelStatement(string label, int line) : base(line) => Label = label;

		public string Label { get; }
	}
}
