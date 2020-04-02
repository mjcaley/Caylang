using Caylang.Assembler.ParseTree;
using CayLang.Assembler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Type = Caylang.Assembler.ParseTree.Type;

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
            Initialize();
        }

        public Parser(IEnumerable<Token> tokens) : this(tokens.GetEnumerator()) { }

        private void Initialize()
        {
            Advance();
            Advance();
        }

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

        public NullaryInstruction Halt()
        {
            if (Current?.Type == TokenType.Halt)
            {
                var value = new NullaryInstruction(InstructionType.Halt, ParseTree.Type.Void, Current.Line);
                Advance();

                return value;
            }
            else
            {
                throw new UnexpectedTokenException(Current);
            }
        }

        public NullaryInstruction Pop()
        {
            if (Current?.Type == TokenType.Pop)
            {
                var value = new NullaryInstruction(InstructionType.Pop, ParseTree.Type.Void, Current.Line);
                Advance();

                return value;
            }
            else
            {
                throw new UnexpectedTokenException(Current);
            }
        }

        public ParseTree.Type NumericType()
        {
            switch (Current?.Type)
            {
                case TokenType.i8Type:
                    Advance();
                    return Type.Integer8;
                case TokenType.u8Type:
                    Advance();
                    return Type.UInteger8;
                case TokenType.i16Type:
                    Advance();
                    return Type.Integer16;
                case TokenType.u16Type:
                    Advance();
                    return Type.UInteger16;
                case TokenType.i32Type:
                    Advance();
                    return Type.Integer32;
                case TokenType.u32Type:
                    Advance();
                    return Type.UInteger32;
                case TokenType.i64Type:
                    Advance();
                    return Type.Integer64;
                case TokenType.u64Type:
                    Advance();
                    return Type.UInteger64;
                case TokenType.f32Type:
                    Advance();
                    return Type.FloatingPoint32;
                case TokenType.f64Type:
                    Advance();
                    return Type.FloatingPoint64;
                default:
                    throw new UnexpectedTokenException(Current);
            }
        }

        public NullaryInstruction Add()
        {
            if (Current?.Type == TokenType.Add)
            {
                var instructionLine = Current.Line;
                Advance();
                var returnType = NumericType();

                return new NullaryInstruction(InstructionType.Add, returnType, instructionLine); ;
            }
            else
            {
                throw new UnexpectedTokenException(Current);
            }
        }
    }
}
