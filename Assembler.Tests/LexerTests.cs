﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            Assert.Equal(input?[0], lexer.Current);
            Assert.Equal(Lexer.LexerMode.SkipWhitespace, lexer.Mode);
        }

        [Theory]
        [InlineData("=", TokenType.Equal)]
        [InlineData(":", TokenType.Colon)]
        public void StartReturnsToken(string input, TokenType type)
        {
            using var lexer = new Lexer(new StringReader(input));
            var token = lexer.Start();

            Assert.Equal(type, token?.Type);
            Assert.Equal(1, token?.Line);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Theory]
        [InlineData("-", Lexer.LexerMode.IsNegative)]
        [InlineData("0", Lexer.LexerMode.Digit)]
        [InlineData("1", Lexer.LexerMode.IsDecimal)]
        [InlineData("2", Lexer.LexerMode.IsDecimal)]
        [InlineData("3", Lexer.LexerMode.IsDecimal)]
        [InlineData("4", Lexer.LexerMode.IsDecimal)]
        [InlineData("5", Lexer.LexerMode.IsDecimal)]
        [InlineData("6", Lexer.LexerMode.IsDecimal)]
        [InlineData("7", Lexer.LexerMode.IsDecimal)]
        [InlineData("8", Lexer.LexerMode.IsDecimal)]
        [InlineData("9", Lexer.LexerMode.IsDecimal)]
        [InlineData("_", Lexer.LexerMode.IsWord)]
        [InlineData("a", Lexer.LexerMode.IsWord)]
        [InlineData("b", Lexer.LexerMode.IsWord)]
        [InlineData("c", Lexer.LexerMode.IsWord)]
        public void StartAppendsAndTransitions(string input, Lexer.LexerMode mode)
        {
            using var lexer = new Lexer(new StringReader(input));
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal(input, lexer.Lexeme);
            Assert.Equal(lexer.Mode, mode);
        }

        [Fact]
        public void StartDiscardsAndTransitions()
        {
            using var lexer = new Lexer(new StringReader("\"1"));
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal('1', lexer.Current);
            Assert.Equal(Lexer.LexerMode.IsString, lexer.Mode);
        }

        [Fact]
        public void StartEmitsErrorToken()
        {
            using var lexer = new Lexer(new StringReader("*"));
            var token = lexer.Start();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Equal("*", token?.Value);
            Assert.Empty(lexer.Lexeme);
        }
        #endregion

        #region IsNegative rule
        [Fact]
        public void NegativeTransitionsToDigit()
        {
            using var lexer = new Lexer(new StringReader("-0"))
            {
                Mode = Lexer.LexerMode.IsNegative
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
                Mode = Lexer.LexerMode.IsNegative
            };
            lexer.Append();
            var token = lexer.Negative();

            Assert.Null(token);
            Assert.Equal("-" + data, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsDecimal, lexer.Mode);
        }

        [Fact]
        public void NegativeEmitsErrorToken()
        {
            const string input = "-a";
            using var lexer = new Lexer(new StringReader(input))
            {
                Mode = Lexer.LexerMode.IsNegative
            };
            lexer.Append();
            var token = lexer.Negative();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Equal(input, token?.Value);
            Assert.Empty(lexer.Lexeme);
        }
        #endregion

        #region Digit rule
        [Theory]
        [InlineData("x", Lexer.LexerMode.IsHexadecimal)]
        [InlineData("b", Lexer.LexerMode.IsBinary)]
        [InlineData(".", Lexer.LexerMode.IsFloat)]
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

            Assert.Equal(TokenType.IntegerLiteral, token?.Type);
            Assert.Equal("1", token?.Value);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
            Assert.Empty(lexer.Lexeme);
        }
        #endregion

        #region IsDecimal rule

        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        [InlineData("7")]
        [InlineData("8")]
        [InlineData("9")]
        public void DecimalAppendsDigit(string input)
        {
            using var lexer = new Lexer(new StringReader("1" + input))
            {
                Mode = Lexer.LexerMode.IsDecimal
            };
            lexer.Append();
            var token = lexer.IsDecimal();

            Assert.Null(token);
            Assert.Equal("1" + input, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsDecimal, lexer.Mode);
        }

        [Fact]
        public void DecimalAppendsDotAndTransitionsToFloat()
        {
            const string input = "0.";
            using var lexer = new Lexer(new StringReader(input))
            {
                Mode = Lexer.LexerMode.IsDecimal
            };
            lexer.Append();
            var token = lexer.IsDecimal();

            Assert.Null(token);
            Assert.Equal(input, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsFloat, lexer.Mode);
        }

        [Fact]
        public void DecimalEmitsIntegerTokenAndTransitionsToStart()
        {
            using var lexer = new Lexer(new StringReader("1 "))
            {
                Mode = Lexer.LexerMode.IsDecimal
            };
            lexer.Append();
            var token = lexer.IsDecimal();

            Assert.NotNull(token);
            Assert.Equal(TokenType.IntegerLiteral, token?.Type);
            Assert.Equal("1", token?.Value);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
        #endregion

        #region IsHexadecimal rule

        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        [InlineData('2')]
        [InlineData('3')]
        [InlineData('4')]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        [InlineData('a')]
        [InlineData('b')]
        [InlineData('c')]
        [InlineData('d')]
        [InlineData('e')]
        [InlineData('f')]
        public void IsHexadecimalAppendsToLexeme(char data)
        {
            using var lexer = new Lexer(new StringReader("0x" + data))
            {
                Mode = Lexer.LexerMode.IsHexadecimal
            };
            lexer.Append();
            lexer.Append();
            var token = lexer.IsHexadecimal();

            Assert.Null(token);
            Assert.Equal("0x" + data, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsHexadecimal, lexer.Mode);
        }

        [Fact]
        public void IsHexadecimalEmitsIntegerTokenWhenNoMatch()
        {
            using var lexer = new Lexer(new StringReader("0x1 "))
            {
                Mode = Lexer.LexerMode.IsHexadecimal
            };
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.IsHexadecimal();

            Assert.NotNull(token);
            Assert.Equal(TokenType.IntegerLiteral, token?.Type);
            Assert.Equal("0x1", token?.Value);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
            Assert.Empty(lexer.Lexeme);
        }
        #endregion

        #region IsBinary rule

        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        public void IsBinaryAppendsToLexeme(char data)
        {
            using var lexer = new Lexer(new StringReader("0b" + data))
            {
                Mode = Lexer.LexerMode.IsBinary
            };
            lexer.Append();
            lexer.Append();
            var token = lexer.IsBinary();

            Assert.Null(token);
            Assert.Equal("0b" + data, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsBinary, lexer.Mode);
        }

        [Fact]
        public void IsBinaryEmitsIntegerTokenWhenNoMatch()
        {
            using var lexer = new Lexer(new StringReader("0b1 "))
            {
                Mode = Lexer.LexerMode.IsBinary
            };
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.IsBinary();

            Assert.NotNull(token);
            Assert.Equal(TokenType.IntegerLiteral, token?.Type);
            Assert.Equal("0b1", token?.Value);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
        #endregion

        #region IsFloat rule
        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        [InlineData('2')]
        [InlineData('3')]
        [InlineData('4')]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        public void IsFloatAppendsToLexeme(char data)
        {
            using var lexer = new Lexer(new StringReader("0." + data))
            {
                Mode = Lexer.LexerMode.IsFloat
            };
            lexer.Append();
            lexer.Append();
            var token = lexer.IsFloat();

            Assert.Null(token);
            Assert.Equal("0." + data, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsFloat, lexer.Mode);
        }

        [Fact]
        public void IsFloatEmitsFloatTokenWhenNoMatch()
        {
            using var lexer = new Lexer(new StringReader("0.0 "))
            {
                Mode = Lexer.LexerMode.IsFloat
            };
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.IsFloat();

            Assert.NotNull(token);
            Assert.Equal(TokenType.FloatLiteral, token?.Type);
            Assert.Equal("0.0", token?.Value);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
        #endregion

        #region IsString rule
        [Fact]
        public void IsStringAppendsToLexeme()
        {
            using var lexer = new Lexer(new StringReader("\"a\""))
            {
                Mode = Lexer.LexerMode.IsString
            };
            lexer.Advance();
            var token = lexer.IsString();

            Assert.Null(token);
            Assert.Equal("a", lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsString, lexer.Mode);
        }

        [Fact]
        public void IsStringEmitsStringTokenOnDoubleQuote()
        {
            using var lexer = new Lexer(new StringReader("\"a\""))
            {
                Mode = Lexer.LexerMode.IsString
            };
            lexer.Advance();
            lexer.Append();
            var token = lexer.IsString();

            Assert.NotNull(token);
            Assert.Equal(TokenType.StringLiteral, token?.Type);
            Assert.Equal("a", token?.Value);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Fact]
        public void IsStringEmitsStringTokenOnEmptyString()
        {
            using var lexer = new Lexer(new StringReader("\"\""))
            {
                Mode = Lexer.LexerMode.IsString
            };
            lexer.Advance();
            var token = lexer.IsString();

            Assert.NotNull(token);
            Assert.Equal(TokenType.StringLiteral, token?.Type);
            Assert.Equal("", token?.Value);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Fact]
        public void IsStringEmitsErrorTokenOnNewline()
        {
            using var lexer = new Lexer(new StringReader("\"\n"))
            {
                Mode = Lexer.LexerMode.IsString
            };
            lexer.Advance();
            var token = lexer.IsString();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Theory]
        [InlineData("a", "\a")]
        [InlineData("b", "\b")]
        [InlineData("f", "\f")]
        [InlineData("n", "\n")]
        [InlineData("r", "\r")]
        [InlineData("t", "\t")]
        [InlineData("v", "\v")]
        [InlineData("\"", "\"")]
        public void IsStringEscapeCharactersProduceControlCharacters(string escape, string expected)
        {
            using var lexer = new Lexer(new StringReader("\"\\" + escape))
            {
                Mode = Lexer.LexerMode.IsString
            };
            lexer.Advance();
            var token = lexer.IsString();

            Assert.Null(token);
            Assert.Equal(expected, lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsString, lexer.Mode);
        }

        [Fact]
        public void IsStringInvalidEscapeCharacterEmitsErrorToken()
        {
            using var lexer = new Lexer(new StringReader("\"\\q"))
            {
                Mode = Lexer.LexerMode.IsString
            };
            lexer.Advance();
            var token = lexer.IsString();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
        #endregion

        #region IsWord rule

        [Fact]
        public void IsWordAppendsToLexeme()
        {
            using var lexer = new Lexer(new StringReader(".f"))
            {
                Mode = Lexer.LexerMode.IsWord
            };
            lexer.Append();
            var token = lexer.IsWord();

            Assert.Null(token);
            Assert.Equal(".f", lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.IsWord, lexer.Mode);
        }

        [Fact]
        public void IsWordEmitsFuncTokenAndTransitionsToStart()
        {
            using var lexer = new Lexer(new StringReader("func "))
            {
                Mode = Lexer.LexerMode.IsWord
            };
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.IsWord();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Func, token?.Type);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Fact]
        public void IsWordEmitsDefineTokenAndTransitionsToStart()
        {
            using var lexer = new Lexer(new StringReader("define "))
            {
                Mode = Lexer.LexerMode.IsWord
            };
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.IsWord();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Define, token?.Type);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Fact]
        public void IsWordEmitsIdentifierTokenAndTransitionsToStart()
        {
            using var lexer = new Lexer(new StringReader("a "))
            {
                Mode = Lexer.LexerMode.IsWord
            };
            lexer.Append();
            var token = lexer.IsWord();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Identifier, token?.Type);
            Assert.Equal("a", token?.Value);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(' ', lexer.Current);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
        #endregion
    }
}
