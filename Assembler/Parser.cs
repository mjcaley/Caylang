using Caylang.Assembler.ParseTree;
using CayLang.Assembler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using InstructionType = Caylang.Assembler.ParseTree.InstructionType;

namespace Caylang.Assembler
{
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class UnexpectedTokenException : ParserException
    {
        public UnexpectedTokenException(Token? found) : base(_message) => FoundToken = found;

        public UnexpectedTokenException(Token? found, Exception innerException) : base(_message, innerException) => FoundToken = found;

        private const string _message = "Unexpected token";

        public Token? FoundToken { get; }
    }

	public class Parser
	{
        private readonly IEnumerator<Token> tokens;

        public Token? Current { get; private set; }

        public Token? Next { get; private set; }

        public Parser(IEnumerator<Token> tokens)
        {
            this.tokens = tokens;
            Advance();
            Advance();
        }

        public Parser(IEnumerable<Token> tokens) : this(tokens.GetEnumerator()) { }

        private void Advance()
        {
            Current = Next;
            if (tokens.MoveNext())
            {
                Next = tokens.Current;
            }
            else
            {
                Next = null;
            }
        }

        public Tree Start()
        {
            return null;
        }

        private NullaryInstruction VoidInstruction(TokenType t, Instruction i)
        {
            if (Current?.Type == t)
            {
                var value = new NullaryInstruction(i, ParseTree.InstructionType.Void, Current.Line);
                Advance();

                return value;
            }
            else
            {
                throw new UnexpectedTokenException(Current);
            }
        }

        public NullaryInstruction Halt()
        {
            return VoidInstruction(TokenType.Halt, Instruction.Halt);
        }

        public NullaryInstruction Pop()
        {
            return VoidInstruction(TokenType.Pop, Instruction.Pop);
        }

        public ParseTree.InstructionType NumericType()
        {
            var match = Current?.Type switch
            {
                TokenType.i8Type => InstructionType.Integer8,
                TokenType.u8Type => InstructionType.UInteger8,
                TokenType.i16Type => InstructionType.Integer16,
                TokenType.u16Type => InstructionType.UInteger16,
                TokenType.i32Type => InstructionType.Integer32,
                TokenType.u32Type => InstructionType.UInteger32,
                TokenType.i64Type => InstructionType.Integer64,
                TokenType.u64Type => InstructionType.UInteger64,
                TokenType.f32Type => InstructionType.FloatingPoint32,
                TokenType.f64Type => InstructionType.FloatingPoint64,
                _ => throw new UnexpectedTokenException(Current)
            };

            Advance();

            return match;
        }

        private NullaryInstruction ArithmeticInstruction(TokenType t, Instruction i)
        {
            if (Current?.Type == t)
            {
                var instructionLine = Current.Line;
                Advance();
                var returnType = NumericType();

                return new NullaryInstruction(i, returnType, instructionLine); ;
            }
            else
            {
                throw new UnexpectedTokenException(Current);
            }
        }

        public NullaryInstruction Add()
        {
            return ArithmeticInstruction(TokenType.Add, Instruction.Add);
        }

        public NullaryInstruction Sub()
        {
            return ArithmeticInstruction(TokenType.Subtract, Instruction.Sub);
        }

        public NullaryInstruction Mul()
        {
            return ArithmeticInstruction(TokenType.Multiply, Instruction.Mul);
        }

        public NullaryInstruction Div()
        {
            return ArithmeticInstruction(TokenType.Divide, Instruction.Div);
        }

        public NullaryInstruction Mod()
        {
            return ArithmeticInstruction(TokenType.Modulo, Instruction.Mod);
        }
    }
}
