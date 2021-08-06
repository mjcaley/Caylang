using System.Collections.Generic;
using System.Linq;
using Yoakke.Lexer;

namespace Caylang.Assembler
{
	public enum ParseNodeKind
	{
		Literal,
		Type,
		Keyword,
		Struct,
		Definition,
		TypeList
	}

	public class ParseNode2
	{
		public ParseNodeKind Kind { get; }

		public bool IsLeaf { get; }
		public IToken<TokenType2>? Token { get; }

		public bool HasChildren => !IsLeaf;
		private readonly List<ParseNode2> _children = new List<ParseNode2>();
		public IEnumerable<ParseNode2> Children => _children;

		public ParseNode2(ParseNodeKind kind, IToken<TokenType2> token)
		{
			Kind = kind;
			IsLeaf = true;
			Token = token;
		}

		public ParseNode2(ParseNodeKind kind, List<ParseNode2> children)
		{
			Kind = kind;
			IsLeaf = false;
			_children = children;
		}

		public ParseNode2(ParseNodeKind kind, ICollection<ParseNode2> children)
			: this(kind, children.ToList()) { }

		public ParseNode2(ParseNodeKind kind, params ParseNode2[] children)
			: this(kind, children.ToList()) { }

		public ParseNode2(ParseNodeKind kind, ParseNode2 child)
			: this(kind, new List<ParseNode2>() { child }) { }
	}
}
