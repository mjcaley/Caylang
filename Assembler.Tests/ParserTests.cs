using System;
using System.Collections.Generic;
using System.Text;
using Caylang.Assembler;
using CayLang.Assembler;
using Xunit;
using Xunit.Sdk;

namespace CayLang.Assembler.Tests
{
    public class ParserTests
    {
        [Fact]
        public void SignedIntegerConvertsToValue()
        {
            var testInput = new Token[] { new Token(TokenType.IntegerLiteral, "42", 1) };
            var parser = new Parser(testInput);
            var result = parser.SignedInteger();

            Assert.Equal(42, result);
        }

        [Fact]
        public void UnsignedIntegerConvertsToValue()
        {
            var testInput = new Token[] { new Token(TokenType.IntegerLiteral, "42", 1) };
            var parser = new Parser(testInput);
            var result = parser.UnsignedInteger();

            Assert.Equal(42UL, result);
        }

        [Fact]
        public void FloatingPoint32ConvertsToValue()
        {
            var testInput = new Token[] { new Token(TokenType.FloatLiteral, "42.0", 1) };
            var parser = new Parser(testInput);
            var result = parser.FloatingPoint32();

            Assert.Equal(42.0f, result);
        }

        [Fact]
        public void FloatingPoint64ConvertsToValue()
        {
            var testInput = new Token[] { new Token(TokenType.FloatLiteral, "42.0", 1) };
            var parser = new Parser(testInput);
            var result = parser.FloatingPoint64();

            Assert.Equal(42.0d, result);
        }
    }
}
