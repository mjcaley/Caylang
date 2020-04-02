using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Tree
	{
		public Tree() { }

		public List<Definition> Definitions { get; } = new List<Definition>();
		public List<Function> Functions { get; } = new List<Function>();
	}
}
