using System.Collections.Generic;
using System.Collections.Immutable;
using Yoakke.Lexer;
using Yoakke.SyntaxTree;
using Yoakke.SyntaxTree.Attributes;

namespace Caylang.Assembler
{
	[SyntaxTree]
	public partial class ParseNode
	{
		public string Kind { get; }

		public bool IsLeaf { get; }
		public IToken<TokenType>? Token { get; }

		public bool HasChildren => !IsLeaf;
		public ImmutableArray<ParseNode> Children { get; } = ImmutableArray<ParseNode>.Empty;

		public ParseNode(string kind, IToken<TokenType> token)
		{
			Kind = kind;
			Token = token;
		}

		public ParseNode(string kind, ImmutableArray<ParseNode> children)
		{
			Kind = kind;
			IsLeaf = false;
			Children = children;
		}

		public ParseNode(string kind, IEnumerable<ParseNode> children)
			: this(kind, ImmutableArray.ToImmutableArray(children)) { }

		public ParseNode(string kind, params ParseNode[] children)
			: this(kind, ImmutableArray.ToImmutableArray(children)) { }

		public ParseNode(string kind, ParseNode child)
			: this(kind, ImmutableArray.Create(child)) { }
	}
}
