using System;
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
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);

            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }

        [Fact]
        public void BeginsWithEmptyLexeme()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);

            Assert.Equal("", lexer.Lexeme);
        }

        [Fact]
        public void BeginsWithLine1()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);

            Assert.Equal(1, lexer.Line);
        }

        [Fact]
        public void NullCharacterWithEmptyString()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);

            Assert.Equal('\0', lexer.Current);
        }

        [Fact]
        public void FirstCharacterOfStringAssignedToCurrent()
        {
            using var testInput = new StringReader("a");
            var lexer = new Lexer(testInput);

            Assert.Equal('a', lexer.Current);
        }

        [Fact]
        public void AdvanceReturnsPreviousValue()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);
            var result = lexer.Advance();

            Assert.Equal('1', result);
        }

        [Fact]
        public void AppendTest()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);

            Assert.Empty(lexer.Lexeme);
            var l = lexer.Append();
            Assert.Same(lexer, l);
            Assert.Equal("1", lexer.Lexeme);
        }

        [Fact]
        public void AppendUsesArgument()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);

            Assert.Empty(lexer.Lexeme);
            var l = lexer.Append('4');
            Assert.Same(lexer, l);
            Assert.Equal("4", lexer.Lexeme);
        }

        [Fact]
        public void ConsumeTest()
        {
            var input = "1";
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
            lexer.Append();
            var result = lexer.Consume();

            Assert.Equal(input, result);
            Assert.Empty(lexer.Lexeme);
        }

        [Fact]
        public void DiscardTest()
        {
            var input = "1";
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
            lexer.Append();
            var l = lexer.Discard();

            Assert.Same(lexer, l);
            Assert.Empty(lexer.Lexeme);
            Assert.NotEqual('1', lexer.Current);
        }

        [Fact]
        public void AdvanceAssignsNextValueToCurrent()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);
            lexer.Advance();

            Assert.Equal('2', lexer.Current);
        }

        [Fact]
        public void AdvanceNewlineIncrementsLine()
        {
            using var testInput = new StringReader("\n");
            var lexer = new Lexer(testInput);
            lexer.Advance();

            Assert.Equal(2, lexer.Line);
        }

        [Fact]
        public void AdvanceCarriageReturnIsNewline()
        {
            using var testInput = new StringReader("\r");
            var lexer = new Lexer(testInput);
            
            Assert.Equal('\n', lexer.Current);
        }

        [Fact]
        public void AdvanceCarriageReturnIncrementsLine()
        {
            using var testInput = new StringReader("\r");
            var lexer = new Lexer(testInput);
            lexer.Advance();

            Assert.Equal(2, lexer.Line);
        }

        [Fact]
        public void AdvanceConsumesBothCarriageReturnNewLine()
        {
            using var testInput = new StringReader("\r\n");
            var lexer = new Lexer(testInput);

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
            using var testInput = new StringReader(data + "1");
            var lexer = new Lexer(testInput)
            {
                Mode = Lexer.LexerMode.SkipWhitespace
            };
            var token = lexer.SkipWhitespace();

            Assert.Null(token);
            Assert.Equal('1', lexer.Current);
            Assert.Equal(Lexer.LexerMode.SkipWhitespace, lexer.Mode);
        }

        [Fact]
        public void SkipTest()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);
            var l = lexer.Skip();

            Assert.Same(lexer, l);
            Assert.Equal('2', lexer.Current);
            Assert.Empty(lexer.Lexeme);
        }

        [Fact]
        public void TransitionTest()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);
            var l = lexer.Transition(Lexer.LexerMode.End);

            Assert.Same(lexer, l);
            Assert.Equal(Lexer.LexerMode.End, lexer.Mode);
        }

        [Fact]
        public void SkipWhitespaceTransitionsToStart()
        {
            using var testInput = new StringReader("1");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
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
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
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
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal(input, lexer.Lexeme);
            Assert.Equal(lexer.Mode, mode);
        }

        [Fact]
        public void StartDiscardsAndTransitions()
        {
            using var testInput = new StringReader("\"1");
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal('1', lexer.Current);
            Assert.Equal(Lexer.LexerMode.IsString, lexer.Mode);
        }

        [Fact]
        public void StartEmitsErrorToken()
        {
            using var testInput = new StringReader("*");
            var lexer = new Lexer(testInput);
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
            using var testInput = new StringReader("-0");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("-" + data);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("1");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("1" + input);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("1 ");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("0x" + data);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("0x1 ");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("0b" + data);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("0b1 ");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("0." + data);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("0.0 ");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("\"a\"");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("\"a\"");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("\"\"");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("\"\n");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("\"\\" + escape);
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("\"\\q");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader(".f");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("func ");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("define ");
            var lexer = new Lexer(testInput)
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
            using var testInput = new StringReader("a ");
            var lexer = new Lexer(testInput)
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

        [Theory]
        [InlineData("func", TokenType.Func)]
        [InlineData("define", TokenType.Define)]
        [InlineData("args", TokenType.Args)]
        [InlineData("locals", TokenType.Locals)]
        [InlineData("halt", TokenType.Halt)]
        [InlineData("nop", TokenType.Noop)]
        [InlineData("add", TokenType.Add)]
        [InlineData("sub", TokenType.Subtract)]
        [InlineData("mul", TokenType.Multiply)]
        [InlineData("div", TokenType.Divide)]
        [InlineData("mod", TokenType.Modulo)]
        [InlineData("ldconst", TokenType.LoadConst)]
        [InlineData("ldlocal", TokenType.LoadLocal)]
        [InlineData("stlocal", TokenType.StoreLocal)]
        [InlineData("pop", TokenType.Pop)]
        [InlineData("testeq", TokenType.TestEqual)]
        [InlineData("testne", TokenType.TestNotEqual)]
        [InlineData("testgt", TokenType.TestGreaterThan)]
        [InlineData("testlt", TokenType.TestLessThan)]
        [InlineData("jmp", TokenType.Jump)]
        [InlineData("jmpt", TokenType.JumpTrue)]
        [InlineData("jmpf", TokenType.JumpFalse)]
        [InlineData("callfunc", TokenType.CallFunc)]
        [InlineData("callinterface", TokenType.CallInterface)]
        [InlineData("ret", TokenType.Return)]
        [InlineData("newstruct", TokenType.NewStruct)]
        [InlineData("ldfield", TokenType.LoadField)]
        [InlineData("stfield", TokenType.StoreField)]
        [InlineData("addr", TokenType.AddressType)]
        [InlineData("i8", TokenType.i8Type)]
        [InlineData("u8", TokenType.u8Type)]
        [InlineData("i16", TokenType.i16Type)]
        [InlineData("u16", TokenType.u16Type)]
        [InlineData("i32", TokenType.i32Type)]
        [InlineData("u32", TokenType.u32Type)]
        [InlineData("i64", TokenType.i64Type)]
        [InlineData("u64", TokenType.u64Type)]
        [InlineData("f32", TokenType.f32Type)]
        [InlineData("f64", TokenType.f64Type)]
        [InlineData("str", TokenType.StringType)]
        public void IsWordEmitsKeywordToken(string input, TokenType type)
        {
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput)
            {
                Mode = Lexer.LexerMode.IsWord
            };
            Token? token = null;
            while (token is null)
            {
                token ??= lexer.IsWord();
            }

            Assert.NotNull(token);
            Assert.Equal(type, token?.Type);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.Start, lexer.Mode);
        }
        #endregion

        #region End rule
        [Fact]
        public void EndEmitsEndOfFileToken()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput)
            {
                Mode = Lexer.LexerMode.End
            };
            var token = lexer.End();

            Assert.NotNull(token);
            Assert.Equal(TokenType.EndOfFile, token?.Type);
            Assert.Empty(lexer.Lexeme);
            Assert.Equal(Lexer.LexerMode.None, lexer.Mode);
        }
        #endregion
    }
}
