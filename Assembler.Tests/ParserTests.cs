using System;
using System.Collections.Generic;
using Caylang.Assembler;
using Caylang.Assembler.ParseTree;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Caylang.Assembler.Tests
{
    public class ParserTests
    {
        private static Token Eof { get; } = new Token(TokenType.EndOfFile, 10);

        [Fact]
        public void StartRule()
        {
            var parser = new Parser(new[]
            {
                new Token(TokenType.Define, 1),
                new Token(TokenType.Identifier, 2, "constant"),
                new Token(TokenType.Equal, 3),
                new Token(TokenType.IntegerLiteral, 4, "42"),
                new Token(TokenType.Integer8Type, 5),
                
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

            Assert.IsType<Tree>(result);
            var tree = result as Tree;
            Assert.NotEmpty(tree?.Definitions);
            Assert.NotEmpty(tree?.Functions);
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

            Assert.IsType<Tree>(result);
            Assert.Single(result.Children);
            Assert.IsType<UnexpectedTokenError>(result.Children[0]);
        }

        [Fact]
        public void ParsesDefinition()
        {
            var constant = new Token(TokenType.Identifier, 2, "constant");
            
            var parser = new Parser(new[]
            {
                new Token(TokenType.Define, 1),
                constant,
                new Token(TokenType.Equal, 3),
                new Token(TokenType.IntegerLiteral, 4, "42"),
                new Token(TokenType.Integer8Type, 5),
                Eof
            });
            var result = parser.ParseDefinition();

            Assert.IsType<Definition>(result);
            var definition = result as Definition;
            Assert.Equal(definition?.Name, constant);
            Assert.IsType<Integer8Literal>(definition?.Value);
        }

        [Fact]
        public void ParsesFunction()
        {
            var name = new Token(TokenType.Identifier, 2, "name");
            var one = new Token(TokenType.IntegerLiteral, 5, "1");
            var two = new Token(TokenType.IntegerLiteral, 8, "2");
            
            var parser = new Parser(new[]
            {
                new Token(TokenType.Func, 1),
                name,
                new Token(TokenType.Locals, 3),
                new Token(TokenType.Equal, 4),
                one,
                new Token(TokenType.Args, 6),
                new Token(TokenType.Equal, 7),
                two,
                Eof
            });
            var result = parser.ParseFunction();

            Assert.IsType<FunctionNode>(result);
            var function = result as FunctionNode;
            Assert.Equal(name, function?.Name);
            Assert.Equal(one, function?.Locals);
            Assert.Equal(two, function?.Arguments);
            Assert.Empty(function?.Statements);
        }

        [Fact]
        public void ParseFunctionWithStatements()
        {
            var name = new Token(TokenType.Identifier, 2, "name");
            var localsZero = new Token(TokenType.IntegerLiteral, 5, "0");
            var argsZero = new Token(TokenType.IntegerLiteral, 8, "0");

            var parser = new Parser(new[]
            {
                new Token(TokenType.Func, 1), 
                name,
                new Token(TokenType.Locals, 3),
                new Token(TokenType.Equal, 4),
                localsZero,
                new Token(TokenType.Args, 6),
                new Token(TokenType.Equal, 7),
                argsZero,
                new Token(TokenType.Identifier, 9, "label"),
                new Token(TokenType.Colon, 10),
                new Token(TokenType.Noop, 11), 
                new Token(TokenType.Return, 12), 
                Eof
            });
            var result = parser.ParseFunction();
            
            Assert.IsType<FunctionNode>(result);
            var function = result as FunctionNode;
            Assert.Equal(name, function?.Name);
            Assert.Equal(localsZero, function?.Locals);
            Assert.Equal(argsZero, function?.Arguments);
            
            Assert.NotEmpty(function?.Statements);
            Assert.Equal(3, result.Children.Count);
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
                new Token(TokenType.Integer8Type, 4),
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
                new Token(TokenType.Halt, 4),
                Eof
            });
            var result = parser.ParseStatements();
            
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void ParseStatementsReturnsError()
        {
            var current = new Token(TokenType.IntegerLiteral, 1, "42");
            var type = new Token(TokenType.Integer32Type, 2);
            
            var parser = new Parser(new[]
            {
                current,
                type,
                Eof
            });
            var result = parser.ParseStatements();

            Assert.IsType<UnexpectedTokenError>(result[0]);
            Assert.Single(result);
            var error = result[0] as UnexpectedTokenError;
            Assert.Equal(2, error?.Terminals.Count);
            Assert.Equal(current, error?.Terminals[0]);
            Assert.Equal(type, error?.Terminals[1]);
        }

        [Fact]
        public void ParseLabel()
        {
            var name = new Token(TokenType.Identifier, 1, "label");

            var parser = new Parser(new[]
            {
                name,
                new Token(TokenType.Colon, 2),
                Eof
            });
            var result = parser.ParseLabel();

            Assert.IsType<LabelStatement>(result);
            var label = result as LabelStatement;
            Assert.Equal(name, label?.Label);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParserLabelReturnsErrorOnFirstToken()
        {
            var parser = new Parser(new[] { Eof });
            var result = parser.ParseLabel();

            Assert.IsType<UnexpectedTokenError>(result);
        }

        [Fact]
        public void ParserLabelReturnsErrorOnSecondToken()
        {
            var label = new Token(TokenType.Identifier, 1, "label");

            var parser = new Parser(new[] { 
                label,
                Eof
            });
            var result = parser.ParseLabel();

            Assert.IsType<UnexpectedTokenError>(result);
        }

        [Theory]
        [InlineData(TokenType.Halt)]
        [InlineData(TokenType.Noop)]
        [InlineData(TokenType.Pop)]
        [InlineData(TokenType.Add)]
        [InlineData(TokenType.Subtract)]
        [InlineData(TokenType.Multiply)]
        [InlineData(TokenType.Divide)]
        [InlineData(TokenType.Modulo)]
        [InlineData(TokenType.TestEqual)]
        [InlineData(TokenType.TestNotEqual)]
        [InlineData(TokenType.TestGreaterThan)]
        [InlineData(TokenType.TestLessThan)]
        [InlineData(TokenType.Return)]
        public void ParsesNullaryInstruction(TokenType testInput)
        {
            var instruction = new Token(testInput, 1);
            var returnType = new Token(TokenType.Integer8Type, 2);

            var parser = new Parser(new[]
            {
                instruction,
                returnType,
                Eof
            });
            var result = parser.ParseInstruction();
            
            Assert.IsType<NullaryInstruction>(result);
            var statement = result as NullaryInstruction;
            Assert.Equal(instruction, statement?.Instruction);
            Assert.Equal(returnType, statement?.ReturnType);
            Assert.Equal(Eof, parser.Current);
        }

        [Theory]
        [InlineData(TokenType.LoadConst)]
        [InlineData(TokenType.LoadLocal)]
        [InlineData(TokenType.StoreLocal)]
        [InlineData(TokenType.Jump)]
        [InlineData(TokenType.JumpTrue)]
        [InlineData(TokenType.JumpFalse)]
        [InlineData(TokenType.NewStruct)]
        [InlineData(TokenType.NewArray)]
        [InlineData(TokenType.CallFunc)]
        [InlineData(TokenType.LoadField)]
        [InlineData(TokenType.StoreField)]
        public void ParsesUnaryInstruction(TokenType testInput)
        {
            var instruction = new Token(testInput, 1);
            var returnType = new Token(TokenType.UInteger64Type, 2);
            var operand = new Token(TokenType.IntegerLiteral, 3, "0");

            var parser = new Parser(new[]
            {
                instruction,
                returnType,
                operand,
                new Token(TokenType.Integer8Type, 3),
                Eof
            });
            var result = parser.ParseInstruction();

            Assert.IsType<UnaryInstruction>(result);
            var statement = result as UnaryInstruction;
            Assert.Equal(instruction, statement?.Instruction);
            Assert.Equal(returnType, statement?.ReturnType);
            Assert.Single(result.Children);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesUnaryInstructionReturnsError()
        {
            var current = new Token(TokenType.IntegerLiteral, 1, "42");
            var type = new Token(TokenType.Integer32Type, 2);
            
            var parser = new Parser(new[]
            {
                current,
                type,
                Eof
            });
            var result = parser.ParseInstruction();

            Assert.IsType<UnexpectedTokenError>(result);
            var error = result as UnexpectedTokenError;
            Assert.Equal(2, error?.Terminals.Count);
            Assert.Equal(current, error?.Terminals[0]);
            Assert.Equal(type, error?.Terminals[1]);
        }

        [Theory]
        [InlineData(TokenType.Integer8Type)]
        [InlineData(TokenType.UInteger8Type)]
        [InlineData(TokenType.Integer16Type)]
        [InlineData(TokenType.UInteger16Type)]
        [InlineData(TokenType.Integer32Type)]
        [InlineData(TokenType.UInteger32Type)]
        [InlineData(TokenType.Integer64Type)]
        [InlineData(TokenType.UInteger64Type)]
        [InlineData(TokenType.Float32Type)]
        [InlineData(TokenType.Float64Type)]
        [InlineData(TokenType.AddressType)]
        public void ParseInstructionWithInstructionType(TokenType testInput)
        {
            var returnType = new Token(testInput, 2);

            var parser = new Parser(new[]
            {
                new Token(TokenType.Add, 1),
                returnType,
                Eof
            });
            var result = parser.ParseInstruction();

            Assert.IsType<NullaryInstruction>(result);
            var statement = result as NullaryInstruction;
            Assert.Equal(returnType, statement?.ReturnType);
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
        public void ParsesIdentifierLiteralReturnsError()
        {
            var current = new Token(TokenType.IntegerLiteral, 1, "42");
            
            var parser = new Parser(new[]
            {
                current,
                new Token(TokenType.Integer32Type, 5),
                Eof
            });
            var result = parser.ParseStringLiteral();

            Assert.IsType<UnexpectedTokenError>(result);
            var error = result as UnexpectedTokenError;
            Assert.Single(error?.Terminals);
            Assert.Equal(current, error?.Terminals[0]);
        }

        [Fact]
        public void ParsesInteger8Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.Integer8Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<Integer8Literal>(result);
            var literal = result as Integer8Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesInteger16Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.Integer16Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<Integer16Literal>(result);
            var literal = result as Integer16Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesInteger32Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.Integer32Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<Integer32Literal>(result);
            var literal = result as Integer32Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesInteger64Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.Integer64Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<Integer64Literal>(result);
            var literal = result as Integer64Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesUnsignedInteger8Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.UInteger8Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<UnsignedInteger8Literal>(result);
            var literal = result as UnsignedInteger8Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesUnsignedInteger16Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.UInteger16Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<UnsignedInteger16Literal>(result);
            var literal = result as UnsignedInteger16Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesUnsignedInteger32Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.UInteger32Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<UnsignedInteger32Literal>(result);
            var literal = result as UnsignedInteger32Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesUnsignedInteger64Literal()
        {
            var fortyTwo = new Token(TokenType.IntegerLiteral, 1, "42");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.UInteger64Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<UnsignedInteger64Literal>(result);
            var literal = result as UnsignedInteger64Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesIntegerLiteralReturnsError()
        {
            var current = new Token(TokenType.Identifier, 1, "name");
            var next = new Token(TokenType.Equal, 2);
            
            var parser = new Parser(new[]
            {
                current,
                next,
                new Token(TokenType.IntegerLiteral, 4, "1"),
                new Token(TokenType.Integer32Type, 5),
                Eof
            });
            var result = parser.ParseIntegerLiteral();

            Assert.IsType<UnexpectedTokenError>(result);
            var error = result as UnexpectedTokenError;
            Assert.Equal(2, error?.Terminals.Count);
            Assert.Equal(current, error?.Terminals[0]);
            Assert.Equal(next, error?.Terminals[1]);
        }

        [Fact]
        public void ParsesFloat32Literal()
        {
            var fortyTwo = new Token(TokenType.FloatLiteral, 1, "4.2");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.Float32Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<Float32Literal>(result);
            var literal = result as Float32Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesFloat64Literal()
        {
            var fortyTwo = new Token(TokenType.FloatLiteral, 1, "4.2");

            var parser = new Parser(new[]
            {
                fortyTwo,
                new Token(TokenType.Float64Type, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<Float64Literal>(result);
            var literal = result as Float64Literal;
            Assert.Equal(fortyTwo, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesFloatLiteralReturnsError()
        {
            var current = new Token(TokenType.Identifier, 1, "name");
            var next = new Token(TokenType.Equal, 2);
            
            var parser = new Parser(new[]
            {
                current,
                next,
                new Token(TokenType.IntegerLiteral, 4, "1"),
                new Token(TokenType.Integer32Type, 5),
                Eof
            });
            var result = parser.ParseFloatLiteral();

            Assert.IsType<UnexpectedTokenError>(result);
            var error = result as UnexpectedTokenError;
            Assert.Equal(2, error?.Terminals.Count);
            Assert.Equal(current, error?.Terminals[0]);
            Assert.Equal(next, error?.Terminals[1]);
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
        public void ParsesStringLiteralWithType()
        {
            var value = new Token(TokenType.StringLiteral, 1, "string");
            
            var parser = new Parser(new[]
            {
                value,
                new Token(TokenType.StringType, 2),
                Eof
            });
            var result = parser.ParseLiteral();

            Assert.IsType<StringLiteral>(result);
            var literal = result as StringLiteral;
            Assert.Equal(value, literal?.Atom);
            Assert.Same(Eof, parser.Current);
        }

        [Fact]
        public void ParsesStringLiteralReturnsError()
        {
            var current = new Token(TokenType.Identifier, 1, "name");
            
            var parser = new Parser(new[]
            {
                current,
                new Token(TokenType.Equal, 3),
                new Token(TokenType.IntegerLiteral, 4, "1"),
                new Token(TokenType.Integer32Type, 5),
                Eof
            });
            var result = parser.ParseStringLiteral();

            Assert.IsType<UnexpectedTokenError>(result);
            var error = result as UnexpectedTokenError;
            Assert.Single(error?.Terminals);
            Assert.Equal(current, error?.Terminals[0]);
        }
    }
}
