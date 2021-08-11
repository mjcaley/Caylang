using Xunit;
using Caylang.Assembler;

namespace Caylang.Assembler.Tests
{
	public class ParserTests
	{
		[Fact]
		public void StartNode()
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
			var result = parser.ParseStart();

			Assert.True(result.IsOk);
			var node = Assert.IsType<ParseTreeBranch>(result.Ok.Value);
			Assert.Equal("start", node.Kind);
			Assert.NotEmpty(node.Children);
			Assert.Collection(node.Children,
				c => Assert.Equal("struct", c.Kind),
				c => Assert.Equal("definition", c.Kind),
				c => Assert.Equal("function", c.Kind));
		}

		[Fact]
		public void DefinitionNode()
		{
			var testData = @"define Definition i32 42";
			var lexer = new AsmLexer(testData);
			var parser = new AsmParser(lexer);
			var result = parser.ParseDefinition();

			Assert.True(result.IsOk);
			var node = Assert.IsType<ParseTreeBranch>(result.Ok.Value);
			Assert.Equal("definition", node.Kind);
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
			var node = Assert.IsType<ParseTreeBranch>(result.Ok.Value);
			Assert.Equal("struct", node.Kind);
		}

		[Theory]
		[InlineData("i8")]
		[InlineData("i8, i32")]
		public void TypeListNode(string testData)
		{
			var lexer = new AsmLexer(testData);
			var parser = new AsmParser(lexer);
			var result = parser.ParseTypeList();

			Assert.True(result.IsOk);
			var node = Assert.IsType<ParseTreeBranch>(result.Ok.Value);
			Assert.Equal("type_list", node.Kind);
			Assert.NotEmpty(node.Children);
			Assert.All(node.Children, child => Assert.Equal("type", child.Kind));
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
			var node = Assert.IsType<ParseTreeBranch>(result.Ok.Value);
			Assert.Equal("function", node.Kind);
		}

		[Fact]
		public void NullaryInstructionNodeWithoutType()
		{
			var lexer = new AsmLexer("halt");
			var parser = new AsmParser(lexer);
			var result = parser.ParseNullaryInstruction();

			Assert.True(result.IsOk);
			Assert.Equal("nullary_instruction", result.Ok.Value.Kind);
			Assert.Single(result.Ok.Value.Children);
			Assert.Equal("instruction", result.Ok.Value.Children[0].Kind);
		}

		[Fact]
		public void NullaryInstructionNodeWithType()
		{
			var lexer = new AsmLexer("add i8");
			var parser = new AsmParser(lexer);
			var result = parser.ParseNullaryInstruction();

			Assert.True(result.IsOk);
			var node = Assert.IsType<ParseTreeBranch>(result.Ok.Value);
			Assert.Equal("nullary_instruction", node.Kind);
			Assert.Collection(node.Children,
				c => Assert.Equal("instruction", c.Kind),
				c => Assert.Equal("type", c.Kind));
			Assert.Equal("instruction", node.Children[0].Kind);
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
			var type = Assert.IsType<ParseTreeLeaf>(result.Ok.Value);
			Assert.Equal("type", type.Kind);
			Assert.Equal(testData, type.Token.Text);
		}

		[Theory]
		[InlineData("42", "number")]
		[InlineData("4.2", "number")]
		[InlineData(@"""Forty-two""", "string")]
		[InlineData("forty_two", "identifier")]
		public void LiteralNode(string testData, string childKind)
		{
			var lexer = new AsmLexer(testData);
			var parser = new AsmParser(lexer);
			var result = parser.ParseLiteral();

			Assert.True(result.IsOk);
			var literal = Assert.IsType<ParseTreeBranch>(result.Ok.Value);
			Assert.Equal("literal", literal.Kind);
			Assert.Equal(childKind, literal.Children[0].Kind);
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
			var literal = Assert.IsType<ParseTreeLeaf>(result.Ok.Value);
			Assert.Equal("integer", literal.Kind);
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
			var literal = Assert.IsType<ParseTreeLeaf>(result.Ok.Value);
			Assert.Equal("float", literal.Kind);
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
			var literal = Assert.IsType<ParseTreeLeaf>(result.Ok.Value);
			Assert.Equal("string", literal.Kind);
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
			var literal = Assert.IsType<ParseTreeLeaf>(result.Ok.Value);
			Assert.Equal("identifier", literal.Kind);
			Assert.Equal(testData, literal.Token.Text);
			Assert.Equal(TokenType.Identifier, literal.Token.Kind);
		}
	}
}
