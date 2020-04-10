using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Function
	{
		public Function(string name, int locals, int arguments, int line, List<Statement> statements) =>
			(Name, Locals, Arguments, Line, Statements) = (name, locals, arguments, line, statements);

		public string Name { get; }
		public int Locals { get; }
		public int Arguments { get; }
		public int Line { get; }
		public List<Statement> Statements { get; }
	}
}
