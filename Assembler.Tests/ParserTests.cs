using Xunit;
using Caylang.Assembler.ParseTree;

namespace Caylang.Assembler.Tests
{
    public class ParserTests
    {
        [Fact]
        public void ModuleNode()
        {
            var testData = @"
                struct Structure
                    u64, addr

                define Definition i32 42

                func Function locals=0, args=0
                    ret
            ";
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseModule();

            Assert.True(result.IsOk);
            var node = Assert.IsType<Module>(result.Ok.Value);
            Assert.NotEmpty(node.Children);
            Assert.Collection(node.Children,
                c => Assert.IsType<Struct>(c),
                c => Assert.IsType<Definition>(c),
                c => Assert.IsType<Function>(c));
        }

        [Fact]
        public void DefinitionNode()
        {
            var testData = @"define Definition i32 42";
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseDefinition();

            Assert.True(result.IsOk);
            var node = Assert.IsType<Definition>(result.Ok.Value);
        }

        [Fact]
        public void StructNode()
        {
            var testData = @"
                struct Vector
                    u64, addr
            ";
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseStruct();

            Assert.True(result.IsOk);
            var node = Assert.IsType<Struct>(result.Ok.Value);
        }

        [Theory]
        [InlineData("i8")]
        [InlineData("i8, i32")]
        public void TypeListNode(string testData)
        {
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseTypes();

            Assert.True(result.IsOk);
            var node = Assert.IsType<Types>(result.Ok.Value);
            Assert.NotEmpty(node.Children);
            Assert.All(node.Children, child => Assert.IsType<Type>(child));
        }

        [Fact]
        public void FunctionNode()
        {
            var lexer = new AsmLexer(@"
                func Function locals=4, args=2
                    ret
            ");
            var parser = new AsmParser(lexer);
            var result = parser.ParseFunction();

            Assert.True(result.IsOk);
            var node = Assert.IsType<Function>(result.Ok.Value);
        }

        [Fact]
        public void NullaryInstructionNodeWithoutType()
        {
            var lexer = new AsmLexer("halt");
            var parser = new AsmParser(lexer);
            var result = parser.ParseNullaryInstruction();

            Assert.True(result.IsOk);
            Assert.IsType<NullaryInstruction>(result.Ok.Value);
            Assert.Single(result.Ok.Value.Children);
            Assert.IsType<Instruction>(result.Ok.Value.Children[0]);
        }

        [Fact]
        public void NullaryInstructionNodeWithType()
        {
            var lexer = new AsmLexer("add i8");
            var parser = new AsmParser(lexer);
            var result = parser.ParseNullaryInstruction();

            Assert.True(result.IsOk);
            var node = Assert.IsType<NullaryInstruction>(result.Ok.Value);
            Assert.Collection(node.Children,
                c => Assert.IsType<Instruction>(c),
                c => Assert.IsType<Type>(c));
        }

        [Theory]
        [InlineData("i8")]
        [InlineData("i16")]
        [InlineData("i32")]
        [InlineData("i64")]
        [InlineData("u8")]
        [InlineData("u16")]
        [InlineData("u32")]
        [InlineData("u64")]
        [InlineData("f32")]
        [InlineData("f64")]
        [InlineData("addr")]
        [InlineData("str")]
        [InlineData("void")]
        public void TypeNode(string testData)
        {
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseType();

            Assert.True(result.IsOk);
            var type = Assert.IsType<Type>(result.Ok.Value);
            Assert.Equal(testData, type.Token.Text);
        }

        [Theory]
        [InlineData("42")]
        [InlineData("4.2")]
        public void LiteralNumberNode(string testData)
        {
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseLiteral();

            Assert.True(result.IsOk);
            var literal = Assert.IsType<Literal>(result.Ok.Value);
            Assert.IsType<Number>(literal.Children[0]);
        }

        [Fact]
        public void LiteralStringNode()
        {
            var lexer = new AsmLexer(@"""Forty-two""");
            var parser = new AsmParser(lexer);
            var result = parser.ParseLiteral();

            Assert.True(result.IsOk);
            var literal = Assert.IsType<Literal>(result.Ok.Value);
            Assert.IsType<String>(literal.Children[0]);
        }


        [Fact]
        public void LiteralIdentifierNode()
        {
            var lexer = new AsmLexer("forty_two");
            var parser = new AsmParser(lexer);
            var result = parser.ParseLiteral();

            Assert.True(result.IsOk);
            var literal = Assert.IsType<Literal>(result.Ok.Value);
            Assert.IsType<Identifier>(literal.Children[0]);
        }

        [Theory]
        [InlineData("0", TokenType.DecIntegerLit)]
        [InlineData("42", TokenType.DecIntegerLit)]
        [InlineData("0x2a", TokenType.HexIntegerLit)]
        [InlineData("0b101010", TokenType.BinIntegerLit)]
        public void IntegerNode(string testData, TokenType tokenType)
        {
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseInteger();

            Assert.True(result.IsOk);
            var literal = Assert.IsType<Integer>(result.Ok.Value);
            Assert.Equal(testData, literal.Token.Text);
            Assert.Equal(tokenType, literal.Token.Kind);
        }

        [Theory]
        [InlineData("42.0")]
        [InlineData("42.")]
        public void FloatNode(string testData)
        {
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseFloat();

            Assert.True(result.IsOk);
            var literal = Assert.IsType<Float>(result.Ok.Value);
            Assert.Equal(testData, literal.Token.Text);
            Assert.Equal(TokenType.FloatLit, literal.Token!.Kind);
        }

        [Fact]
        public void StringNode()
        {
            var testData = @"""Test data""";
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseString();

            Assert.True(result.IsOk);
            var literal = Assert.IsType<String>(result.Ok.Value);
            Assert.Equal(testData, literal.Token.Text);
            Assert.Equal(TokenType.StringLit, literal.Token.Kind);
        }

        [Fact]
        public void IdentifierNode()
        {
            var testData = @"forty_two";
            var lexer = new AsmLexer(testData);
            var parser = new AsmParser(lexer);
            var result = parser.ParseIdentifier();

            Assert.True(result.IsOk);
            var literal = Assert.IsType<Identifier>(result.Ok.Value);
            Assert.Equal(testData, literal.Token.Text);
            Assert.Equal(TokenType.Identifier, literal.Token.Kind);
        }
    }
}
