using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Tree : ParseNode
	{
		public Tree(IEnumerable<ParseNode> children) : base(children) { }

		public IEnumerable<Definition> Definitions => Children.OfType<Definition>(); 
		public IEnumerable<FunctionNode> Functions => Children.OfType<FunctionNode>();
	}
}
