using Xunit;
using Caylang.Assembler;

namespace Caylang.Assembler.Tests
{
	public class Parser2Tests
	{
		[Theory]
		[InlineData("i8")]
		[InlineData("i8, i32")]
		public void TypeListNode(string testData)
		{
			var lexer = new AsmLexer(testData);
			var parser = new AsmParser(lexer);
			var result = parser.ParseTypeList();

			Assert.True(result.IsOk);
			Assert.Equal(ParseNodeKind.TypeList, result.Ok.Value.Kind);
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
			var type = result.Ok.Value;
			Assert.Equal(ParseNodeKind.Type, type.Kind);
			Assert.Equal(testData, type?.Token.Text);
		}

		[Theory]
		[InlineData("42")]
		[InlineData("0x2a")]
		[InlineData("0b101010")] 
		[InlineData("4.2")]
		[InlineData(@"""Forty-two""")]
		[InlineData("forty_two")]
		public void LiteralNode(string testData)
		{
			var lexer = new AsmLexer(testData);
			var parser = new AsmParser(lexer);
			var result = parser.ParseLiteral();

			Assert.True(result.IsOk);
			var literal = result.Ok.Value;
			Assert.Equal(ParseNodeKind.Literal, literal.Kind);
			Assert.Equal(testData, literal?.Token.Text);
		}
	}
}
