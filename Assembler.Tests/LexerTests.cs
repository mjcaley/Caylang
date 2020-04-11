using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using Caylang.Assembler;
using Xunit;

namespace Caylang.Assembler.Tests
{
    public class LexerTests
    {
        [Fact]
        public void BeginsInStartMode()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);

            Assert.Equal(lexer.Start, lexer.Mode);
        }

        [Fact]
        public void BeginsWithEmptyLexeme()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);

            Assert.NotNull(lexer.Lexeme);
            Assert.Equal("", lexer.Lexeme.ToString());
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

            Assert.Empty(lexer.Lexeme.ToString());
            var l = lexer.Append();
            Assert.Same(lexer, l);
            Assert.Equal("1", lexer.Lexeme.ToString());
        }

        [Fact]
        public void AppendUsesArgument()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);

            Assert.Empty(lexer.Lexeme.ToString());
            var l = lexer.Append('4');
            Assert.Same(lexer, l);
            Assert.Equal("4", lexer.Lexeme.ToString());
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
            Assert.Empty(lexer.Lexeme.ToString());
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
            Assert.Empty(lexer.Lexeme.ToString());
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
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.SkipWhitespace;
            var token = lexer.SkipWhitespace();

            Assert.Null(token);
            Assert.Equal('1', lexer.Current);
            Assert.Equal(lexer.SkipWhitespace, lexer.Mode);
        }

        [Fact]
        public void SkipTest()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);
            var l = lexer.Skip();

            Assert.Same(lexer, l);
            Assert.Equal('2', lexer.Current);
            Assert.Empty(lexer.Lexeme.ToString());
        }

        [Fact]
        public void TransitionTest()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);
            var l = lexer.Transition(lexer.End);

            Assert.Same(lexer, l);
            Assert.Equal(lexer.End, lexer.Mode);
        }

        [Fact]
        public void TransitionToNull()
        {
            using var testInput = new StringReader("123");
            var lexer = new Lexer(testInput);
            var l = lexer.Transition(null);

            Assert.NotNull(l);
            Assert.Null(lexer.Mode);
        }

        [Fact]
        public void SkipWhitespaceTransitionsToStart()
        {
            using var testInput = new StringReader("1");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.SkipWhitespace;
            lexer.SkipWhitespace();

            Assert.Equal(lexer.Start, lexer.Mode);
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
            Assert.Equal(lexer.SkipWhitespace, lexer.Mode);
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
            Assert.Equal(lexer.Start, lexer.Mode);
            Assert.NotEqual(input?[0], lexer.Current);
        }

        [Fact]
        public void StartTransitionsToFloatAndReplacesLexeme()
        {
            using var testInput = new StringReader(".");
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal("0.", lexer.Lexeme.ToString());
            Assert.Equal(lexer.FloatNumber, lexer.Mode);
        }

        [Fact]
        public void StartAppendsAndTransitionsToNumberBase()
        {
            using var testInput = new StringReader("0");
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal("0", lexer.Lexeme.ToString());
            Assert.Equal(lexer.NumberBase, lexer.Mode);
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
        public void StartAppendsAndTransitionsToDecNumber(string input)
        {
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal(input, lexer.Lexeme.ToString());
            Assert.Equal(lexer.DecNumber, lexer.Mode);
        }

        [Theory]
        [InlineData("_")]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("c")]
        [InlineData("d")]
        [InlineData("e")]
        [InlineData("f")]
        public void StartAppendsAndTransitionsToKeyword(string input)
        {
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Equal(input, lexer.Lexeme.ToString());
            Assert.Equal(lexer.Keyword, lexer.Mode);
        }

        [Fact]
        public void StartDiscardsAndTransitions()
        {
            using var testInput = new StringReader("\"1");
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.Null(token);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal('1', lexer.Current);
            Assert.Equal(lexer.StringValue, lexer.Mode);
        }

        [Fact]
        public void StartEmitsErrorToken()
        {
            using var testInput = new StringReader("*");
            var lexer = new Lexer(testInput);
            var token = lexer.Start();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
        }

        [Fact]
        public void StartTransitionsToEnd()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);
            var token = lexer.Start();
            
            Assert.Null(token);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.End, lexer.Mode);
        }
        #endregion

        #region NumberBase
        [Fact]
        public void NumberBaseTransitionsToHexNumber()
        {
            using var testInput = new StringReader("0x");
            var lexer = new Lexer(testInput);
            lexer.Advance();
            var token = lexer.NumberBase();

            Assert.Null(token);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.HexNumber, lexer.Mode);
        }

        [Fact]
        public void NumberBaseTransitionsToBinNumber()
        {
            using var testInput = new StringReader("0b");
            var lexer = new Lexer(testInput);
            lexer.Advance();
            var token = lexer.NumberBase();

            Assert.Null(token);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.BinNumber, lexer.Mode);
        }

        [Fact]
        public void NumberBaseTransitionsToFloatNumber()
        {
            using var testInput = new StringReader("0.");
            var lexer = new Lexer(testInput);
            lexer.Append();
            var token = lexer.NumberBase();

            Assert.Null(token);
            Assert.Equal("0.", lexer.Lexeme.ToString());
            Assert.Equal(lexer.FloatNumber, lexer.Mode);
        }

        [Fact]
        public void NumberBaseEmitsIntegerTokenOnZero()
        {
            using var testInput = new StringReader("0");
            var lexer = new Lexer(testInput);
            lexer.Append();
            var token = lexer.NumberBase();
            
            Assert.NotNull(token);
            Assert.Equal(TokenType.IntegerLiteral, token.Type);
            Assert.Equal("0", token.Value);
            Assert.Equal(lexer.Start, lexer.Mode);
            Assert.NotEqual('0', lexer.Current);
            Assert.Empty(lexer.Lexeme.ToString());
        }

        [Theory]
        [InlineData('1')]
        [InlineData('2')]
        [InlineData('3')]
        [InlineData('4')]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        public void NumberBaseEmitsErrorTokenOnUnexpectedInput(char input)
        {
            using var testInput = new StringReader("0" + input);
            var lexer = new Lexer(testInput);
            lexer.Append();
            var token = lexer.NumberBase();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }
        #endregion

        #region DecNumber rule
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
        public void DecNumberAppendsDigit(string input)
        {
            using var testInput = new StringReader("1" + input);
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.DecNumber;
            lexer.Append();
            var token = lexer.DecNumber();

            Assert.Null(token);
            Assert.Equal("1" + input, lexer.Lexeme.ToString());
            Assert.Equal(lexer.DecNumber, lexer.Mode);
        }

        [Fact]
        public void DecNumberAppendsDotAndTransitionsToFloat()
        {
            const string input = "0.";
            using var testInput = new StringReader(input);
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.DecNumber;
            lexer.Append();
            var token = lexer.DecNumber();

            Assert.Null(token);
            Assert.Equal(input, lexer.Lexeme.ToString());
            Assert.Equal(lexer.FloatNumber, lexer.Mode);
        }

        [Fact]
        public void DecNumerEmitsIntegerTokenAndTransitionsToStart()
        {
            using var testInput = new StringReader("1 ");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.DecNumber;
            lexer.Append();
            var token = lexer.DecNumber();

            Assert.NotNull(token);
            Assert.Equal(TokenType.IntegerLiteral, token?.Type);
            Assert.Equal("1", token?.Value);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }
        #endregion

        #region HexNumber rule

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
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("c")]
        [InlineData("d")]
        [InlineData("e")]
        [InlineData("f")]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("C")]
        [InlineData("D")]
        [InlineData("E")]
        [InlineData("F")]
        public void HexNumberAppendsToLexeme(string data)
        {
            using var testInput = new StringReader(data);
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.HexNumber;
            var token = lexer.HexNumber();

            Assert.Null(token);
            Assert.Equal(data, lexer.Lexeme.ToString());
            Assert.Equal(lexer.HexNumber, lexer.Mode);
        }

        [Fact]
        public void HexNumberEmitsIntegerTokenWhenNoMatch()
        {
            using var testInput = new StringReader("2a ");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.HexNumber;
            lexer.Append();
            lexer.Append();
            var token = lexer.HexNumber();

            Assert.NotNull(token);
            Assert.Equal(TokenType.IntegerLiteral, token?.Type);
            Assert.Equal("42", token?.Value);
            Assert.Equal(lexer.Start, lexer.Mode);
            Assert.Empty(lexer.Lexeme.ToString());
        }
        #endregion

        #region BinNumber rule

        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        public void BinNumberAppendsToLexeme(char data)
        {
            using var testInput = new StringReader("0b" + data);
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.BinNumber;
            lexer.Append();
            lexer.Append();
            var token = lexer.BinNumber();

            Assert.Null(token);
            Assert.Equal("0b" + data, lexer.Lexeme.ToString());
            Assert.Equal(lexer.BinNumber, lexer.Mode);
        }

        [Fact]
        public void BinNumberEmitsIntegerTokenWhenNoMatch()
        {
            using var testInput = new StringReader("101 ");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.BinNumber;
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.BinNumber();

            Assert.NotNull(token);
            Assert.Equal(TokenType.IntegerLiteral, token?.Type);
            Assert.Equal("5", token?.Value);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }
        #endregion

        #region FloatNumber rule
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
        public void FloatNumberAppendsToLexeme(char data)
        {
            using var testInput = new StringReader("0." + data);
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.FloatNumber;
            lexer.Append();
            lexer.Append();
            var token = lexer.FloatNumber();

            Assert.Null(token);
            Assert.Equal("0." + data, lexer.Lexeme.ToString());
            Assert.Equal(lexer.FloatNumber, lexer.Mode);
        }

        [Fact]
        public void FloatNumberEmitsFloatTokenWhenNoMatch()
        {
            using var testInput = new StringReader("0.0 ");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.FloatNumber;
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.FloatNumber();

            Assert.NotNull(token);
            Assert.Equal(TokenType.FloatLiteral, token?.Type);
            Assert.Equal("0.0", token?.Value);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }
        #endregion

        #region StringValue rule
        [Fact]
        public void StringValueAppendsToLexeme()
        {
            using var testInput = new StringReader("\"a\"");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.StringValue;
            lexer.Advance();
            var token = lexer.StringValue();

            Assert.Null(token);
            Assert.Equal("a", lexer.Lexeme.ToString());
            Assert.Equal(lexer.StringValue, lexer.Mode);
        }

        [Fact]
        public void StringValueEmitsStringTokenOnDoubleQuote()
        {
            using var testInput = new StringReader("\"a\"");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.StringValue;
            lexer.Advance();
            lexer.Append();
            var token = lexer.StringValue();

            Assert.NotNull(token);
            Assert.Equal(TokenType.StringLiteral, token?.Type);
            Assert.Equal("a", token?.Value);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }

        [Fact]
        public void StringValueEmitsStringTokenOnEmptyString()
        {
            using var testInput = new StringReader("\"\"");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.StringValue;
            lexer.Advance();
            var token = lexer.StringValue();

            Assert.NotNull(token);
            Assert.Equal(TokenType.StringLiteral, token?.Type);
            Assert.Equal("", token?.Value);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }

        [Fact]
        public void StringValueEmitsErrorTokenOnNewline()
        {
            using var testInput = new StringReader("\"\n");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.StringValue;
            lexer.Advance();
            var token = lexer.StringValue();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
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
        public void StringValueEscapeCharactersProduceControlCharacters(string escape, string expected)
        {
            using var testInput = new StringReader("\"\\" + escape);
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.StringValue;
            lexer.Advance();
            var token = lexer.StringValue();

            Assert.Null(token);
            Assert.Equal(expected, lexer.Lexeme.ToString());
            Assert.Equal(lexer.StringValue, lexer.Mode);
        }

        [Fact]
        public void StringValueInvalidEscapeCharacterEmitsErrorToken()
        {
            using var testInput = new StringReader("\"\\q");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.StringValue;
            lexer.Advance();
            var token = lexer.StringValue();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Error, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }
        #endregion

        #region Keyword rule
        [Fact]
        public void KeywordAppendsToLexeme()
        {
            using var testInput = new StringReader(".f");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.Keyword;
            lexer.Append();
            var token = lexer.Keyword();

            Assert.Null(token);
            Assert.Equal(".f", lexer.Lexeme.ToString());
            Assert.Equal(lexer.Keyword, lexer.Mode);
        }

        [Fact]
        public void KeywordEmitsFuncTokenAndTransitionsToStart()
        {
            using var testInput = new StringReader("func ");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.Keyword;
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.Keyword();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Func, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }

        [Fact]
        public void KeywordEmitsDefineTokenAndTransitionsToStart()
        {
            using var testInput = new StringReader("define ");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.Keyword;
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            lexer.Append();
            var token = lexer.Keyword();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Define, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }

        [Fact]
        public void KeywordEmitsIdentifierTokenAndTransitionsToStart()
        {
            using var testInput = new StringReader("a ");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.Keyword;
            lexer.Append();
            var token = lexer.Keyword();

            Assert.NotNull(token);
            Assert.Equal(TokenType.Identifier, token?.Type);
            Assert.Equal("a", token?.Value);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(' ', lexer.Current);
            Assert.Equal(lexer.Start, lexer.Mode);
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
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.Keyword;
            Token? token = null;
            while (token is null)
            {
                token ??= lexer.Keyword();
            }

            Assert.NotNull(token);
            Assert.Equal(type, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Equal(lexer.Start, lexer.Mode);
        }
        #endregion

        #region End rule
        [Fact]
        public void EndEmitsEndOfFileToken()
        {
            using var testInput = new StringReader("");
            var lexer = new Lexer(testInput);
            lexer.Mode = lexer.End;
            var token = lexer.End();

            Assert.NotNull(token);
            Assert.Equal(TokenType.EndOfFile, token?.Type);
            Assert.Empty(lexer.Lexeme.ToString());
            Assert.Null(lexer.Mode);
        }
        #endregion
    }
}
