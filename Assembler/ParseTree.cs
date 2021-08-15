using System.Collections.Immutable;
using Yoakke.Lexer;

namespace Caylang.Assembler.ParseTree
{
	public abstract partial record Node { }

	public abstract record Leaf : Node
	{
		public IToken<TokenType> Token { get; init; }
	}

	public abstract record Branch : Node
	{
		public ImmutableArray<Node> Children { get; init; } = ImmutableArray<Node>.Empty;
	}

	public record Module : Branch { }
	public record Definition : Branch { }
	public record Struct : Branch { }
	public record Types : Branch { }
	public abstract record Parameter : Branch { }
	public record ArgsParameter : Parameter { }
	public record LocalsParameter : Parameter { }
	public record FunctionParameters : Branch { }
	public record Function : Branch { }
	public record Statements : Branch { }
	public abstract record Statement : Branch { }
	public record NullaryInstruction : Statement { }
	public record UnaryInstruction : Statement { }
	public record Label : Statement { }
	public record Number : Branch { }
	public record Literal : Branch { }

	public record Keyword : Leaf { }
	public record Type : Leaf { }
	public record Instruction : Leaf { }
	public record Sign : Leaf { }
	public record Identifier : Leaf { }
	public record Integer : Leaf { }
	public record Float : Leaf { }
	public record String : Leaf { }
}
