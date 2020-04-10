using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public abstract class Statement
	{
		protected Statement(int line) => Line = line;
		
		public int Line { get; }
	}
}
