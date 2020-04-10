using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Definition
	{
		public Definition(string name, Operand value, int line) => (Name, Value, Line) = (name, value, line);

		public string Name { get; }
		public Operand Value { get; }
		public int Line { get; }
	}
}
