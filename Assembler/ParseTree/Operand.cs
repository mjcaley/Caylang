using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public enum OperandType
	{
		Unknown,
		Integer8,
		UInteger8,
		Integer16,
		UInteger16,
		Integer32,
		UInteger32,
		Integer64,
		UInteger64,
		FloatingPoint32,
		FloatingPoint64,
		Address,
		StringType
	}

	public class Operand
	{
		public Operand(Literal value, OperandType type, int line) => (Value, Type, Line) = (value, type, line);

		public Literal Value { get; }
		public OperandType Type { get; }
		public int Line { get; }
	}
}
