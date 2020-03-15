using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Caylang.Assembler;
using Xunit;

namespace CayLang.Assembler.Tests
{
    public class LexerTests
    {
        [Fact]
        public void BeginsInStartMode()
        {
            using var lexer = new Lexer(new StringReader(""));

            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Fact]
        public void BeginsWithEmptyLexeme()
        {
            using var lexer = new Lexer(new StringReader(""));

            Assert.Equal("", lexer.Lexeme);
        }

        [Fact]
        public void BeginsWithLine1()
        {
            using var lexer = new Lexer(new StringReader(""));

            Assert.Equal(1, lexer.Line);
        }

        [Fact]
        public void NullCharacterWithEmptyString()
        {
            using var lexer = new Lexer(new StringReader(""));

            Assert.Equal('\0', lexer.Current);
        }

        [Fact]
        public void FirstCharacterOfStringAssignedToCurrent()
        {
            using var lexer = new Lexer(new StringReader("a"));

            Assert.Equal('a', lexer.Current);
        }

        [Fact]
        public void AdvanceReturnsPreviousValue()
        {
            using var lexer = new Lexer(new StringReader("123"));
            var result = lexer.Advance();

            Assert.Equal('1', result);
        }

        [Fact]
        public void AppendTest()
        {
            using var lexer = new Lexer(new StringReader("123"));

            Assert.Empty(lexer.Lexeme);
            lexer.Append();
            Assert.Equal("1", lexer.Lexeme);
        }

        [Fact]
        public void ConsumeTest()
        {
            var input = "1";
            using var lexer = new Lexer(new StringReader(input));
            lexer.Append();
            var result = lexer.Consume();

            Assert.Equal(input, result);
            Assert.Empty(lexer.Lexeme);
        }

        [Fact]
        public void AdvanceAssignsNextValueToCurrent()
        {
            using var lexer = new Lexer(new StringReader("123"));
            lexer.Advance();

            Assert.Equal('2', lexer.Current);
        }

        [Fact]
        public void AdvanceNewlineIncrementsLine()
        {
            using var lexer = new Lexer(new StringReader("\n"));
            lexer.Advance();

            Assert.Equal(2, lexer.Line);
        }

        [Fact]
        public void AdvanceCarriageReturnIsNewline()
        {
            using var lexer = new Lexer(new StringReader("\r"));
            
            Assert.Equal('\n', lexer.Current);
        }

        [Fact]
        public void AdvanceCarriageReturnIncrementsLine()
        {
            using var lexer = new Lexer(new StringReader("\r"));
            lexer.Advance();

            Assert.Equal(2, lexer.Line);
        }

        [Fact]
        public void AdvanceConsumesBothCarriageReturnNewLine(){
            using var lexer = new Lexer(new StringReader("\r\n"));

            Assert.Equal('\n', lexer.Current);
        }

        #region SkipWhitespace rule
        [Theory]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\v")]
        public void SkipWhitespaceAdvances(string data)
        {
            using var lexer = new Lexer(new StringReader(data + "1"))
            {
                Mode = Lexer.LexerMode.SkipWhitespace
            };
            lexer.SkipWhitespace();

            Assert.Equal('1', lexer.Current);
            Assert.Equal(Lexer.LexerMode.SkipWhitespace, lexer.Mode);
        }

        [Fact]
        public void SkipWhitespaceTransitionsToStart()
        {
            using var lexer = new Lexer(new StringReader("1"))
            {
                Mode = Lexer.LexerMode.SkipWhitespace
            };
            lexer.SkipWhitespace();

            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
		#endregion

		#region Start rule
		[Theory]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("\v")]
        public void StartTransitionsToSkipWhitespace(string input)
        {
            using var lexer = new Lexer(new StringReader(input));
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal(input[0], lexer.Current);
            Assert.Equal(Lexer.LexerMode.SkipWhitespace, lexer.Mode);
        }

        [Theory]
        [InlineData("=", TokenType.Equal)]
        [InlineData(":", TokenType.Colon)]
        public void StartReturnsToken(string input, TokenType type)
        {
            using var lexer = new Lexer(new StringReader(input));
            var token = lexer.Start();

            Assert.Equal(type, token.Type);
            Assert.Equal(1, token.Line);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Theory]
        [InlineData("-", Lexer.LexerMode.Negative)]
        [InlineData("0", Lexer.LexerMode.Digit)]
        [InlineData("1", Lexer.LexerMode.Decimal)]
        [InlineData("2", Lexer.LexerMode.Decimal)]
        [InlineData("3", Lexer.LexerMode.Decimal)]
        [InlineData("4", Lexer.LexerMode.Decimal)]
        [InlineData("5", Lexer.LexerMode.Decimal)]
        [InlineData("6", Lexer.LexerMode.Decimal)]
        [InlineData("7", Lexer.LexerMode.Decimal)]
        [InlineData("8", Lexer.LexerMode.Decimal)]
        [InlineData("9", Lexer.LexerMode.Decimal)]
        [InlineData(".", Lexer.LexerMode.Keyword)]
        [InlineData("_", Lexer.LexerMode.Identifier)]
        [InlineData("a", Lexer.LexerMode.Identifier)]
        [InlineData("b", Lexer.LexerMode.Identifier)]
        [InlineData("c", Lexer.LexerMode.Identifier)]
        public void StartAppendsAndTransitions(string input, Lexer.LexerMode mode)
        {
            using var lexer = new Lexer(new StringReader(input));
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal(input, lexer.Lexeme);
            Assert.Equal(lexer.Mode, mode);
        }

        [Fact]
        public void StartsDiscardsAndTransitions()
        {
            using var lexer = new Lexer(new StringReader("\"1"));
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal('1', lexer.Current);
            Assert.Equal(Lexer.LexerMode.String, lexer.Mode);
        }
        #endregion

        #region Negative rule
        [Fact]
        public void NegativeTransitionsToDigit()
        {
            using var lexer = new Lexer(new StringReader("-0"))
            {
                Mode = Lexer.LexerMode.Negative
            };
            lexer.Append();
            var token = lexer.Negative();

            Assert.Null(token);
            Assert.Equal("-0", lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Digit, lexer.Mode);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        [InlineData("7")]
        [InlineData("8")]
        [InlineData("9")]
        public void NegativeTransitionsToDecimal(string data)
        {
            using var lexer = new Lexer(new StringReader("-" + data))
            {
                Mode = Lexer.LexerMode.Negative
            };
            lexer.Append();
            var token = lexer.Negative();

            Assert.Null(token);
            Assert.Equal("-" + data, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Decimal, lexer.Mode);
        }
        #endregion

        #region Digit rule
        [Theory]
        [InlineData("x", Lexer.LexerMode.Hexadecimal)]
        [InlineData("b", Lexer.LexerMode.Binary)]
        [InlineData(".", Lexer.LexerMode.Float)]
        public void DigitAppendAndTransition(string data, Lexer.LexerMode mode)
        {
            var input = "0" + data;
            using var lexer = new Lexer(new StringReader(input))
            {
                Mode = Lexer.LexerMode.Digit
            };
            lexer.Append();
            var token = lexer.Digit();

            Assert.Null(token);
            Assert.Equal(mode, lexer.Mode);
            Assert.Equal(input, lexer.Lexeme);
        }

        [Fact]
        public void DigitEmitsIntegerAndTransitionsToStart()
        {
            using var lexer = new Lexer(new StringReader("1"))
            {
                Mode = Lexer.LexerMode.Digit
            };
            var token = lexer.Digit();

            Assert.Equal(TokenType.Integer, token.Type);
            Assert.Equal("1", token.Value);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
		#endregion
	}
}
