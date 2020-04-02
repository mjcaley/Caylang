using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public enum Type
	{
		Void,
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
	}

	public enum InstructionType
	{
		Halt,
		Pop,
		Add,
		Sub,
		Mul,
		Div,
		Mod,
		TestEQ,
		TestNE,
		TestGT,
		TestLT,
		Ret,

		LdConst,
		StConst,
		Jmp,
		JmpT,
		JmpF,
		NewStruct,
		NewArray,
		CallFunc,
		LdField,
		StField
	}

	public class InstructionStatement
	{
		public InstructionStatement(InstructionType instruction, int line) => (Instruction, Line) = (instruction, line);

		public InstructionType Instruction { get; }
		public int Line { get; }
	}

	public class NullaryInstruction : InstructionStatement
	{
		public NullaryInstruction(InstructionType instruction, Type returnType, int line) : base(instruction, line) =>
			ReturnType = returnType;

		public Type ReturnType { get; }
	}

	public class UnaryInstruction : InstructionStatement
	{
		public UnaryInstruction(InstructionType instruction, Type returnType, Operand first, int line) : base(instruction, line) =>
			(ReturnType, First) = (returnType, first);

		public Type ReturnType { get; }
		public Operand First { get; }
	}
}
