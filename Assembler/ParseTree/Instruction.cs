using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public enum InstructionType
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

	public enum Instruction
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
		LdLocal,
		StLocal,
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
		public InstructionStatement(Instruction instruction, int line) => (Instruction, Line) = (instruction, line);

		public Instruction Instruction { get; }
		public int Line { get; }
	}

	public class NullaryInstruction : InstructionStatement
	{
		public NullaryInstruction(Instruction instruction, InstructionType returnType, int line) : base(instruction, line) =>
			ReturnType = returnType;

		public InstructionType ReturnType { get; }
	}

	public class UnaryInstruction : InstructionStatement
	{
		public UnaryInstruction(Instruction instruction, InstructionType returnType, Operand first, int line) : base(instruction, line) =>
			(ReturnType, First) = (returnType, first);

		public InstructionType ReturnType { get; }
		public Operand First { get; }
	}
}
