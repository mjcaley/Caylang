using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Operand
	{
		public Operand(Literal value, int line) => (Value, Line) = (value, line);

		public Literal Value { get; }
		public int Line { get; }
	}
}
