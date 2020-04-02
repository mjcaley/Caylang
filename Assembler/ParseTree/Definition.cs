using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Definition
	{
		public Definition(string name, Operand value) => (Name, Value) = (name, value);

		public string Name { get; }
		public Operand Value { get; }
	}
}
