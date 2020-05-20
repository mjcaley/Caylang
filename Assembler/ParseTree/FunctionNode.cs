using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class FunctionNode : ParseNode
	{
		public FunctionNode(Token name, Token locals, Token arguments, IEnumerable<ParseNode> statements) : base(statements) => (Name, Locals, Arguments) = (name, locals, arguments);

		public FunctionNode(Token name, Token locals, Token arguments, params ParseNode[] statements) : base(statements) => (Name, Locals, Arguments) = (name, locals, arguments);

		public Token Name { get; }

		public Token Locals { get; }

		public Token Arguments { get; }

		public IEnumerable<Statement> Statements => Children.OfType<Statement>();
	}
}
