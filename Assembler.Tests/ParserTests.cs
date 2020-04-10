using System;
using System.Collections.Generic;
using Caylang.Assembler;
using Caylang.Assembler.ParseTree;
using Xunit;

namespace CayLang.Assembler.Tests
{
    public class ParserTests
    {
        private static Token Eof { get; } = new Token(TokenType.EndOfFile, 10);

        [Fact]
        public void ParserStartsWithInitializedErrors()
        {
            var parser = new Parser(Array.Empty<Token>());
            
            Assert.Empty(parser.Errors);
        }

        [Fact]
        public void StartRule()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Define, 1),
                new Token(TokenType.Identifier, 2, "constant"),
                new Token(TokenType.Equal, 3),
                new Token(TokenType.IntegerLiteral, 4, "42"),
                new Token(TokenType.i8Type, 5),
                
                new Token(TokenType.Func, 6),
                new Token(TokenType.Identifier, 7, "function"),
                new Token(TokenType.Locals, 8),
                new Token(TokenType.Equal, 9), 
                new Token(TokenType.IntegerLiteral, 10, "1"),
                new Token(TokenType.Args, 11),
                new Token(TokenType.Equal, 12),
                new Token(TokenType.IntegerLiteral, 13, "0"),
                new Token(TokenType.Return, 14),
                Eof
            });
            var result = parser.Start();
            
            Assert.NotEmpty(result.Definitions);
            Assert.NotEmpty(result.Functions);
        }

        [Fact]
        public void StartRuleLogsError()
        {
            var testInput = new Token(TokenType.IntegerLiteral, 1, "42");
            var parser = new Parser(new[]
            {
                testInput,
                Eof
            });
            var result = parser.Start();

            Assert.IsType<UnexpectedTokenException>(parser.Errors[0]);
            var error = parser.Errors[0] as UnexpectedTokenException;
            Assert.Same(testInput, error?.FoundToken);
        }

        [Fact]
        public void ParsesDefinition()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Define, 1),
                new Token(TokenType.Identifier, 2, "constant"),
                new Token(TokenType.Equal, 3),
                new Token(TokenType.IntegerLiteral, 4, "42"),
                new Token(TokenType.i8Type, 5),
                Eof
            });
            var result = parser.ParseDefinition();
            
            Assert.Equal(1, result.Line);
            Assert.Equal("constant", result.Name);
            Assert.IsType<Operand>(result.Value);
        }

        [Fact]
        public void ParsesFunction()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Func, 1),
                new Token(TokenType.Identifier, 2, "name"),
                new Token(TokenType.Locals, 3),
                new Token(TokenType.Equal, 4),
                new Token(TokenType.IntegerLiteral, 5, "1"),
                new Token(TokenType.Args, 6),
                new Token(TokenType.Equal, 7),
                new Token(TokenType.IntegerLiteral, 8, "2"),
                Eof
            });
            var result = parser.ParseFunction();
            
            Assert.Equal("name", result.Name);
            Assert.Equal(1, result.Locals);
            Assert.Equal(2, result.Arguments);
            Assert.Empty(result.Statements);
        }

        [Fact]
        public void ParseFunctionWithStatements()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Func, 1), 
                new Token(TokenType.Identifier, 2, "name"),
                new Token(TokenType.Locals, 6),
                new Token(TokenType.Equal, 7),
                new Token(TokenType.IntegerLiteral, 8, "0"),
                new Token(TokenType.Args, 3),
                new Token(TokenType.Equal, 4),
                new Token(TokenType.IntegerLiteral, 5, "0"),
                new Token(TokenType.Identifier, 9, "label"),
                new Token(TokenType.Colon, 10),
                new Token(TokenType.Noop, 11), 
                new Token(TokenType.Return, 12), 
                Eof
            });
            var result = parser.ParseFunction();
            
            Assert.Equal(3, result.Statements.Count);
            Assert.IsType<LabelStatement>(result.Statements[0]);
            Assert.IsType<NullaryInstruction>(result.Statements[1]);
            Assert.IsType<NullaryInstruction>(result.Statements[2]);
        }

        [Theory]
        [InlineData("a", "1")]
        [InlineData("1", "a")]
        public void ParseFunctionThrowsIntegerConversionException(string argsInput, string localsInput)
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Func, 1),
                new Token(TokenType.Identifier, 2, "name"),
                new Token(TokenType.Locals, 2),
                new Token(TokenType.Equal, 2),
                new Token(TokenType.IntegerLiteral, 2, localsInput),
                new Token(TokenType.Args, 2),
                new Token(TokenType.Equal, 2),
                new Token(TokenType.IntegerLiteral, 2, argsInput),
                Eof
            });

            Assert.Throws<IntegerConversionException>(() => parser.ParseFunction());
        }

        [Fact]
        public void ParseStatementsParsesLabel()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Identifier, 1, "label"),
                new Token(TokenType.Colon, 2),
                Eof
            });
            var result = parser.ParseStatements();
            
            Assert.NotEmpty(result);
            Assert.IsType<LabelStatement>(result[0]);
        }

        [Fact]
        public void ParseStatementsParsesNullaryInstruction()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Halt, 1),
                Eof
            });
            var result = parser.ParseStatements();
            
            Assert.NotEmpty(result);
            Assert.IsType<NullaryInstruction>(result[0]);
        }

        [Fact]
        public void ParseStatementsParsesUnaryInstruction()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.CallFunc, 1),
                new Token(TokenType.IntegerLiteral, 3, "42"),
                new Token(TokenType.i8Type, 4),
                Eof
            });
            var result = parser.ParseStatements();
            
            Assert.NotEmpty(result);
            Assert.IsType<UnaryInstruction>(result[0]);
        }

        [Fact]
        public void ParseStatementsParsesCollection()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Identifier, 1, "start"),
                new Token(TokenType.Colon, 2),
                new Token(TokenType.Noop, 3),
                new Token(TokenType.Halt, 4) 
            });
            var result = parser.ParseStatements();
            
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void ParseLabel()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Identifier, 1, "label"),
                new Token(TokenType.Colon, 2),
                Eof
            });
            var result = parser.ParseLabel();
            
            Assert.Equal("label", result.Label);
            Assert.Equal(1, result.Line);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParserLabelThrows()
        {
            var parser = new Parser(new[] { Eof });
            
            Assert.Throws<UnexpectedTokenException>(() => parser.ParseLabel());
        }
        
        [Theory]
        [InlineData(TokenType.Halt, Instruction.Halt)]
        [InlineData(TokenType.Noop, Instruction.Noop)]
        [InlineData(TokenType.Pop, Instruction.Pop)]
        [InlineData(TokenType.Add, Instruction.Add)]
        [InlineData(TokenType.Subtract, Instruction.Sub)]
        [InlineData(TokenType.Multiply, Instruction.Mul)]
        [InlineData(TokenType.Divide, Instruction.Div)]
        [InlineData(TokenType.Modulo, Instruction.Mod)]
        [InlineData(TokenType.TestEqual, Instruction.TestEQ)]
        [InlineData(TokenType.TestNotEqual, Instruction.TestNE)]
        [InlineData(TokenType.TestGreaterThan, Instruction.TestGT)]
        [InlineData(TokenType.TestLessThan, Instruction.TestLT)]
        [InlineData(TokenType.Return, Instruction.Ret)]
        public void ParsesNullaryInstruction(TokenType testInput, Instruction expected)
        {
            var parser = new Parser(new[]
            {
                new Token(testInput, 1),
                Eof
            });
            var result = parser.ParseInstruction();
            
            Assert.IsType<NullaryInstruction>(result);
            Assert.Equal(expected, result.Instruction);
            Assert.Equal(1, result.Line);
            Assert.Same(Eof, parser.Current);
        }

        [Theory]
        [InlineData(TokenType.LoadConst, Instruction.LdConst)]
        [InlineData(TokenType.LoadLocal, Instruction.LdLocal)]
        [InlineData(TokenType.StoreLocal, Instruction.StLocal)]
        [InlineData(TokenType.Jump, Instruction.Jmp)]
        [InlineData(TokenType.JumpTrue, Instruction.JmpT)]
        [InlineData(TokenType.JumpFalse, Instruction.JmpF)]
        [InlineData(TokenType.NewStruct, Instruction.NewStruct)]
        [InlineData(TokenType.NewArray, Instruction.NewArray)]
        [InlineData(TokenType.CallFunc, Instruction.CallFunc)]
        [InlineData(TokenType.LoadField, Instruction.LdField)]
        [InlineData(TokenType.StoreField, Instruction.StField)]
        public void ParsesUnaryInstruction(TokenType testInput, Instruction expected)
        {
            var parser = new Parser(new[]
            {
                new Token(testInput, 1),
                new Token(TokenType.Identifier, 2, "ident"),
                Eof
            });
            var result = parser.ParseInstruction();
            
            Assert.IsType<UnaryInstruction>(result);
            Assert.Equal(expected, result.Instruction);
            Assert.Equal(1, result.Line);
            var unary = result as UnaryInstruction;
            Assert.NotNull(unary?.First);
            Assert.Same(Eof, parser.Current);
        }

        [Theory]
        [InlineData(TokenType.i8Type, InstructionType.Integer8)]
        [InlineData(TokenType.u8Type, InstructionType.UInteger8)]
        [InlineData(TokenType.i16Type, InstructionType.Integer16)]
        [InlineData(TokenType.u16Type, InstructionType.UInteger16)]
        [InlineData(TokenType.i32Type, InstructionType.Integer32)]
        [InlineData(TokenType.u32Type, InstructionType.UInteger32)]
        [InlineData(TokenType.i64Type, InstructionType.Integer64)]
        [InlineData(TokenType.u64Type, InstructionType.UInteger64)]
        [InlineData(TokenType.f32Type, InstructionType.FloatingPoint32)]
        [InlineData(TokenType.f64Type, InstructionType.FloatingPoint64)]
        [InlineData(TokenType.AddressType, InstructionType.Address)]
        public void ParseInstructionWithInstructionType(TokenType testInput, InstructionType expected)
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Add, 1),
                new Token(testInput, 2), 
                Eof
            });
            var result = parser.ParseInstruction();

            Assert.Equal(expected, result.ReturnType);
        }

        [Theory]
        [InlineData(TokenType.i8Type, InstructionType.Integer8)]
        [InlineData(TokenType.u8Type, InstructionType.UInteger8)]
        [InlineData(TokenType.i16Type, InstructionType.Integer16)]
        [InlineData(TokenType.u16Type, InstructionType.UInteger16)]
        [InlineData(TokenType.i32Type, InstructionType.Integer32)]
        [InlineData(TokenType.u32Type, InstructionType.UInteger32)]
        [InlineData(TokenType.i64Type, InstructionType.Integer64)]
        [InlineData(TokenType.u64Type, InstructionType.UInteger64)]
        [InlineData(TokenType.f32Type, InstructionType.FloatingPoint32)]
        [InlineData(TokenType.f64Type, InstructionType.FloatingPoint64)]
        [InlineData(TokenType.AddressType, InstructionType.Address)]
        public void ParseInstructionType(TokenType testInput, InstructionType expected)
        {
            var parser = new Parser(new[]
            { 
                new Token(testInput, 1), 
                Eof
            });
            var result = parser.ParseInstructionType();
            
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ParseOperandWithIdentifierLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Identifier, 1, "ident"),
                Eof
            });
            var result = parser.ParseOperand();
            
            Assert.Equal(1, result.Line);
            Assert.Equal(OperandType.Unknown, result.Type);
            Assert.IsType<IdentifierLiteral>(result.Value);
            var literal = result.Value as IdentifierLiteral;
            Assert.Equal("ident", literal?.Value);
        }

        [Fact]
        public void ParseOperandWithStringLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.StringLiteral, 1, "string"),
                Eof
            });
            var result = parser.ParseOperand();
            
            Assert.Equal(1, result.Line);
            Assert.Equal(OperandType.StringType, result.Type);
            Assert.IsType<StringLiteral>(result.Value);
            var literal = result.Value as StringLiteral;
            Assert.Equal("string", literal?.Value);
        }

        [Fact]
        public void ParseOperandWithIntegerLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.IntegerLiteral, 1, "42"),
                new Token(TokenType.i8Type, 2), 
                Eof
            });
            var result = parser.ParseOperand();
            
            Assert.Equal(1, result.Line);
            Assert.Equal(OperandType.Integer8, result.Type);
            Assert.IsType<IntegerLiteral>(result.Value);
            var literal = result.Value as IntegerLiteral;
            Assert.Equal("42", literal?.Value);
        }

        [Fact]
        public void ParseOperandWithFloatLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.FloatLiteral, 1, "4.2"),
                new Token(TokenType.f32Type, 2), 
                Eof
            });
            var result = parser.ParseOperand();
            
            Assert.Equal(1, result.Line);
            Assert.Equal(OperandType.FloatingPoint32, result.Type);
            Assert.IsType<FloatLiteral>(result.Value);
            var literal = result.Value as FloatLiteral;
            Assert.Equal("4.2", literal?.Value);
        }

        [Theory]
        [InlineData(TokenType.i8Type, OperandType.Integer8)]
        [InlineData(TokenType.u8Type, OperandType.UInteger8)]
        [InlineData(TokenType.i16Type, OperandType.Integer16)]
        [InlineData(TokenType.u16Type, OperandType.UInteger16)]
        [InlineData(TokenType.i32Type, OperandType.Integer32)]
        [InlineData(TokenType.u32Type, OperandType.UInteger32)]
        [InlineData(TokenType.i64Type, OperandType.Integer64)]
        [InlineData(TokenType.u64Type, OperandType.UInteger64)]
        [InlineData(TokenType.f32Type, OperandType.FloatingPoint32)]
        [InlineData(TokenType.f64Type, OperandType.FloatingPoint64)]
        [InlineData(TokenType.AddressType, OperandType.Address)]
        public void ParseOperandWithType(TokenType testInput, OperandType expected)
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.IntegerLiteral, 1, "42"),
                new Token(testInput, 2),
                Eof
            });
            var result = parser.ParseOperand();
            
            Assert.Equal(expected, result.Type);
        }

        [Theory]
        [InlineData(TokenType.i8Type, OperandType.Integer8)]
        [InlineData(TokenType.u8Type, OperandType.UInteger8)]
        [InlineData(TokenType.i16Type, OperandType.Integer16)]
        [InlineData(TokenType.u16Type, OperandType.UInteger16)]
        [InlineData(TokenType.i32Type, OperandType.Integer32)]
        [InlineData(TokenType.u32Type, OperandType.UInteger32)]
        [InlineData(TokenType.i64Type, OperandType.Integer64)]
        [InlineData(TokenType.u64Type, OperandType.UInteger64)]
        [InlineData(TokenType.f32Type, OperandType.FloatingPoint32)]
        [InlineData(TokenType.f64Type, OperandType.FloatingPoint64)]
        [InlineData(TokenType.AddressType, OperandType.Address)]
        public void ParseOperandType(TokenType testInput, OperandType expected)
        {
            var parser = new Parser(new[]
            {
                new Token(testInput, 1),
                Eof
            });
            var result = parser.ParseOperandType();
            
            Assert.Equal(expected, result);
            Assert.Same(Eof, parser.Current);
        }
        
        [Fact]
        public void ParsesIdentifierLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Identifier, 1, "identifier"),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<IdentifierLiteral>(result);
            var literal = result as IdentifierLiteral;
            Assert.Equal("identifier", literal?.Value);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesIntegerLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.IntegerLiteral, 1, "42"),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<IntegerLiteral>(result);
            var literal = result as IntegerLiteral;
            Assert.Equal("42", literal?.Value);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesFloatLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.FloatLiteral, 1, "4.2"),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<FloatLiteral>(result);
            var literal = result as FloatLiteral;
            Assert.Equal("4.2", literal?.Value);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesStringLiteral()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.StringLiteral, 1, "string"),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<StringLiteral>(result);
            var literal = result as StringLiteral;
            Assert.Equal("string", literal?.Value);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParseIdentifierLiteralThrowsUnexpectedTokenException()
        {
            var parser = new Parser(new[]
            {
                Eof
            });

            Assert.Throws<UnexpectedTokenException>(() => parser.ParseLiteral());
            Assert.Same(Eof, parser.Current);
        }
    }
}
