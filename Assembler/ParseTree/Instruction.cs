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
		Address
	}

	public enum Instruction
	{
		Halt,
		Noop,
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

	public abstract class InstructionStatement : Statement
	{
		protected InstructionStatement(Instruction instruction, InstructionType returnType, int line) :
			base(line) =>
			(Instruction, ReturnType) = (instruction, returnType);

		public Instruction Instruction { get; }
		public InstructionType ReturnType { get; }
	}

	public class NullaryInstruction : InstructionStatement
	{
		public NullaryInstruction(Instruction instruction, InstructionType returnType, int line) :
			base(instruction, returnType, line) { }
	}

	public class UnaryInstruction : InstructionStatement
	{
		public UnaryInstruction(Instruction instruction, InstructionType returnType, Operand first, int line) :
			base(instruction, returnType, line) => First = first;

		public Operand First { get; }
	}
}
