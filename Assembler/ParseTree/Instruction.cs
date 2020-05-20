using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class NullaryInstruction : Statement
	{
		public NullaryInstruction(Token instruction, Token returnType)
			=> (Instruction, ReturnType) = (instruction, returnType);

		public NullaryInstruction(Token instruction, Token returnType, IEnumerable<ParseNode> children)
			: base(children)
			=> (Instruction, ReturnType) = (instruction, returnType);
		
		public NullaryInstruction(Token instruction, Token returnType, params ParseNode[] children)
					: base(children)
					=> (Instruction, ReturnType) = (instruction, returnType);

		public Token Instruction { get; }
		public Token ReturnType { get; }
	}

	public class UnaryInstruction : Statement
	{
		public UnaryInstruction(Token instruction, Token returnType, ParseNode operand)
			: base(operand)
			=> (Instruction, ReturnType) = (instruction, returnType);

		public UnaryInstruction(Token instruction, Token returnType, IEnumerable<ParseNode> children)
			: base(children)
			=> (Instruction, ReturnType) = (instruction, returnType);

		public UnaryInstruction(Token instruction, Token returnType, params ParseNode[] children)
			: base(children)
			=> (Instruction, ReturnType) = (instruction, returnType);

		public Token Instruction { get; }
		public Token ReturnType { get; }

		public Literal? First => Children.First() as Literal;
	}
}
