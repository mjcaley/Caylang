using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public abstract class Literal { }

	public class ValueLiteral<T> : Literal
	{
		public ValueLiteral(T value) => Value = value;

		public T Value { get; }
	}

	public class IntegerLiteral : ValueLiteral<string>
	{
		public IntegerLiteral(string value) : base(value) { }
	}

	public class FloatLiteral : ValueLiteral<string>
	{
		public FloatLiteral(string value) : base(value) { }
	}

	public class StringLiteral : ValueLiteral<string>
	{
		public StringLiteral(string value) : base(value) { }
	}

	public class IdentifierLiteral : ValueLiteral<string>
	{
		public IdentifierLiteral(string value) : base(value) { }
	}
}
