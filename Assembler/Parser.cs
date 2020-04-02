﻿using Caylang.Assembler.ParseTree;
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
            var match = Current?.Type switch
            {
                TokenType.i8Type => Type.Integer8,
                TokenType.u8Type => Type.UInteger8,
                TokenType.i16Type => Type.Integer16,
                TokenType.u16Type => Type.UInteger16,
                TokenType.i32Type => Type.Integer32,
                TokenType.u32Type => Type.UInteger32,
                TokenType.i64Type => Type.Integer64,
                TokenType.u64Type => Type.UInteger64,
                TokenType.f32Type => Type.FloatingPoint32,
                TokenType.f64Type => Type.FloatingPoint64,
                _ => throw new UnexpectedTokenException(Current)
            };

            Advance();

            return match;
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
