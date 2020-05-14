using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Tree : ParseNode
	{
		public Tree(IEnumerable<ParseNode> definitions, IEnumerable<ParseNode> functions)
			: this(definitions, functions, Array.Empty<ParseNode>()) { }

		public Tree(
			IEnumerable<ParseNode> definitions,
			IEnumerable<ParseNode> functions,
			IEnumerable<ParseNode> other)
			: base(definitions.Union(functions).Union(other)) { }

		public IEnumerable<Definition> Definitions => Children.OfType<Definition>(); 
		public IEnumerable<FunctionNode> Functions => Children.OfType<FunctionNode>();
	}
}
