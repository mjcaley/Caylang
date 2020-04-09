using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public abstract class Literal { }

	#region AST literals

	public class ASTLiteral : Literal
	{
		public ASTLiteral(string value) => Value = value;

		public string Value { get; }
	}

	public class IntegerLiteral : ASTLiteral
	{
		public IntegerLiteral(string value) : base(value) { }
	}

	public class FloatLiteral : ASTLiteral
	{
		public FloatLiteral(string value) : base(value) { }
	}
	#endregion

	public class VoidLiteral : Literal
	{
		public VoidLiteral() { }
	}

	public class ValueLiteral<T> : Literal
	{
		public ValueLiteral(T value) => Value = value;

		public T Value { get; }
	}

	public class Int8Literal : ValueLiteral<sbyte>
	{
		public Int8Literal(sbyte value) : base(value) { }
	}

	public class UInt8Literal : ValueLiteral<byte>
	{
		public UInt8Literal(byte value) : base(value) { }
	}

	public class Int16Literal : ValueLiteral<short>
	{
		public Int16Literal(short value) : base(value) { }
	}

	public class UInt16Literal : ValueLiteral<ushort>
	{
		public UInt16Literal(ushort value) : base(value) { }
	}

	public class Int32Literal : ValueLiteral<int>
	{
		public Int32Literal(int value) : base(value) { }
	}

	public class UInt32Literal : ValueLiteral<uint>
	{
		public UInt32Literal(uint value) : base(value) { }
	}

	public class Int64Literal : ValueLiteral<long>
	{
		public Int64Literal(long value) : base(value) { }
	}

	public class UInt64Literal : ValueLiteral<ulong>
	{
		public UInt64Literal(ulong value) : base(value) { }
	}

	public class Float32Literal : ValueLiteral<float>
	{
		public Float32Literal(float value) : base(value) { }
	}

	public class Float64Literal : ValueLiteral<double>
	{
		public Float64Literal(double value) : base(value) { }
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
