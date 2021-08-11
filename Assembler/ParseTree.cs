using System.Collections.Generic;
using System.Collections.Immutable;
using Yoakke.Lexer;
using Yoakke.SyntaxTree.Attributes;

namespace Caylang.Assembler
{
	[SyntaxTree]
	public abstract partial record ParseTree
	{
		public ParseTree(string kind) => Kind = kind;

		public string Kind { get; }
	}

	public partial record ParseTreeLeaf : ParseTree
	{
		public IToken<TokenType> Token { get; }

		public ParseTreeLeaf(string kind, IToken<TokenType> token) : base(kind)
			=> Token = token;
	}

	public partial record ParseTreeBranch : ParseTree
	{
		public ImmutableArray<ParseTree> Children { get; } = ImmutableArray<ParseTree>.Empty;

		public ParseTreeBranch(string kind, ImmutableArray<ParseTree> children) : base(kind)
			=> Children = children;

		public ParseTreeBranch(string kind, IEnumerable<ParseTree> children) : base(kind)
			=> Children = ImmutableArray.ToImmutableArray(children);

		public ParseTreeBranch(string kind, params ParseTree[] children) : base(kind)
			=> Children = ImmutableArray.ToImmutableArray(children);

		public ParseTreeBranch(string kind, ParseTree child) : base(kind)
			=> Children = ImmutableArray.Create(child);
	}
}
