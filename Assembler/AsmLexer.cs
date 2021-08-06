using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;

namespace Caylang.Assembler
{
	[Lexer("AsmLexer")]
	public enum TokenType2
	{
		[Error] Error,
		[End] End,
		
		[Ignore]
		[Regex(Regexes.Whitespace)]
		[Regex(@"#[^\n]*")] Ignored,

		[Token("define")] DefineKw,
		[Token("struct")] StructKw,
		[Token("func")] FuncKw,

		[Token("locals")] LocalsKw,
		[Token("args")] ArgsKw,

		[Token("i8")]
		[Token("i16")]
		[Token("i32")]
		[Token("i64")]
		[Token("u8")]
		[Token("u16")]
		[Token("u32")]
		[Token("u64")]
		[Token("f32")]
		[Token("f64")]
		[Token("addr")]
		[Token("str")]
		[Token("void")]
		TypeKw,

		[Token("halt")]
		[Token("noop")]
		[Token("pop")]
		[Token("ret")]
		[Token("add")]
		[Token("sub")]
		[Token("mul")]
		[Token("div")]
		[Token("mod")]
		[Token("ldconst")]
		[Token("ldlocal")]
		[Token("stlocal")]
		[Token("testeq")]
		[Token("testne")]
		[Token("testlt")]
		[Token("testgt")]
		[Token("jmp")]
		[Token("jmpt")]
		[Token("jmpf")]
		[Token("callfunc")]
		[Token("callvirt")]
		[Token("newstruct")]
		[Token("ldfield")]
		[Token("stfield")]
		[Token("newarray")]
		[Token("ldelem")]
		[Token("stelem")]
		InstructionKw,

		[Token("-")] Minus,
		[Token("+")] Plus,

		[Regex(@"[1-9][0-9]*")] DecIntegerLit,
		[Regex(Regexes.HexLiteral)] HexIntegerLit,
		[Regex(@"0b[01]+")] BinIntegerLit,
		[Regex(@"(0|([1-9][0-9]*)).[0-9]*")] FloatLit,
		[Regex(@"""[^""]*""")] StringLit,

		[Regex(Regexes.Identifier)] Identifier
	}
}
