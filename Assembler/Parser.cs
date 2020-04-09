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

        public List<ParserException> Errors { get; } = new List<ParserException>();

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

        private void SkipTo(params TokenType[] type)
        {
            while (Current != null)
            {
                if (!type.Contains(Current.Type))
                {
                    Advance();
                }
            }
        }

        public Tree Start()
        {
            return null;
        }

        public List<InstructionStatement> ParseInstructions()
        {
            var instructions = new List<InstructionStatement>();
            while (Current != null)
            {
                try
                {
                    instructions.Add(ParseInstruction());
                }
                catch (ParserException e)
                {
                    Errors.Add(e);
                    SkipTo(TokenType.EndOfFile, TokenType.Func, TokenType.Define);
                }
            }

            return instructions;
        }

        public InstructionStatement ParseInstruction()
        {
            switch (Current?.Type)
            {
                case TokenType.Halt:
                case TokenType.Noop:
                case TokenType.Pop:
                case TokenType.Add:
                case TokenType.Subtract:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Modulo:
                case TokenType.TestEqual:
                case TokenType.TestNotEqual:
                case TokenType.TestGreaterThan:
                case TokenType.TestLessThan:
                case TokenType.Return:
                    return ParseNullaryInstruction();
                case TokenType.LoadConst:
                case TokenType.LoadLocal:
                case TokenType.StoreLocal:
                case TokenType.Jump:
                case TokenType.JumpTrue:
                case TokenType.JumpFalse:
                case TokenType.NewStruct:
                case TokenType.NewArray:
                case TokenType.CallFunc:
                case TokenType.LoadField:
                case TokenType.StoreField:
                    return ParseUnaryInstruction();
                default:
                    throw new UnexpectedTokenException(Current);
            }
        }
        
        private NullaryInstruction ParseNullaryInstruction()
        {
            var line = Current?.Line ?? 0;

            var instruction = Current?.Type switch
            {
                TokenType.Halt => Instruction.Halt,
                TokenType.Noop => Instruction.Noop,
                TokenType.Pop => Instruction.Pop,
                TokenType.Add => Instruction.Add,
                TokenType.Subtract => Instruction.Sub,
                TokenType.Multiply => Instruction.Mul,
                TokenType.Divide => Instruction.Div,
                TokenType.Modulo => Instruction.Mod,
                TokenType.TestEqual => Instruction.TestEQ,
                TokenType.TestNotEqual => Instruction.TestNE,
                TokenType.TestGreaterThan => Instruction.TestGT,
                TokenType.TestLessThan => Instruction.TestLT,
                TokenType.Return => Instruction.Ret,
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            InstructionType type = ParseInstructionType();
            
            return new NullaryInstruction(instruction, type, line);
        }

        private UnaryInstruction ParseUnaryInstruction()
        {
            var line = Current?.Line ?? 0;

            var instruction = Current?.Type switch
            {
                TokenType.LoadConst => Instruction.LdConst,
                TokenType.LoadLocal => Instruction.LdLocal,
                TokenType.StoreLocal => Instruction.StLocal,
                TokenType.Jump => Instruction.Jmp,
                TokenType.JumpTrue => Instruction.JmpT,
                TokenType.JumpFalse => Instruction.JmpF,
                TokenType.NewStruct => Instruction.NewStruct,
                TokenType.NewArray => Instruction.NewArray,
                TokenType.CallFunc => Instruction.CallFunc,
                TokenType.LoadField => Instruction.LdField,
                TokenType.StoreField => Instruction.StField,
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            var type = ParseInstructionType();
            var operand = ParseOperand();
            
            return new UnaryInstruction(instruction, type, operand, line);
        }

        public InstructionType ParseInstructionType()
        {
            var type = Current?.Type switch
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
                TokenType.AddressType => InstructionType.Address,
                _ => InstructionType.Void
            };
            if (type != InstructionType.Void)
            {
                Advance();
            }

            return type;
        }

        public Operand ParseOperand()
        {
            var line = Current?.Line ?? 0;

            var literal = ParseLiteral();

            if (literal is IdentifierLiteral)
            {
                return new Operand(literal, OperandType.Unknown, line);
            }
            else if (literal is StringLiteral)
            {
                return new Operand(literal, OperandType.StringType, line);
            }
            else
            {
                var type = ParseOperandType();
                return new Operand(literal, type, line);
            }
        }

        public OperandType ParseOperandType()
        {
            var operandType = Current?.Type switch
            {
                TokenType.i8Type => OperandType.Integer8,
                TokenType.u8Type => OperandType.UInteger8,
                TokenType.i16Type => OperandType.Integer16,
                TokenType.u16Type => OperandType.UInteger16,
                TokenType.i32Type => OperandType.Integer32,
                TokenType.u32Type => OperandType.UInteger32,
                TokenType.i64Type => OperandType.Integer64,
                TokenType.u64Type => OperandType.UInteger64,
                TokenType.f32Type => OperandType.FloatingPoint32,
                TokenType.f64Type => OperandType.FloatingPoint64,
                TokenType.AddressType => OperandType.Address,
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            return operandType;
        }

        public Literal ParseLiteral()
        {
            Literal literal = Current?.Type switch
            {
                TokenType.Identifier => new IdentifierLiteral(Current.Value),
                TokenType.IntegerLiteral => new IntegerLiteral(Current.Value),
                TokenType.FloatLiteral => new FloatLiteral(Current.Value),
                TokenType.StringLiteral => new StringLiteral(Current.Value),
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            return literal;
        }
    }
}
