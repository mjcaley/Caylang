using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Function
	{
		public Function(string name, int locals, int arguments, int line) =>
			(Name, Locals, Arguments, Line) = (name, locals, arguments, line);

		public string Name { get; }
		public int Locals { get; }
		public int Arguments { get; }
		public int Line { get; }
	}
}
