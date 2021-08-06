using System.Linq;
using Yoakke.Lexer;
using Yoakke.Parser;
using Yoakke.Parser.Attributes;

namespace Caylang.Assembler
{
    using Token = IToken<TokenType2>;

    [Parser(typeof(TokenType2))]
    public partial class AsmParser
    {
        [Rule("definition : DefineKw Identifier type '=' literal")]
        public static ParseNode2 Definition(Token defineKeyword, Token identifier, ParseNode2 type, Token _, ParseNode2 literal)
        {
            var defineNode = new ParseNode2(ParseNodeKind.Keyword, defineKeyword);
            var identifierNode = new ParseNode2(ParseNodeKind.Literal, identifier);

            return new(ParseNodeKind.Definition, defineNode, identifierNode, type, literal);
        }

        [Rule("struct : StructKw Identifier type_list")]
        public static ParseNode2 Struct(Token structKeyword, Token identifier, ParseNode2 types)
        {
            var structNode = new ParseNode2(ParseNodeKind.Keyword, structKeyword);
            var identifierNode = new ParseNode2(ParseNodeKind.Literal, identifier);

            return new(ParseNodeKind.Struct, structNode, identifierNode, types);
        }

        [Rule("type_list : (type (',' type)*)")]
        public static ParseNode2 TypeList(Punctuated<ParseNode2, Token> elements)
            => new(ParseNodeKind.TypeList, elements.Values.ToList());

        [Rule("type : TypeKw")]
        public static ParseNode2 Type(Token token)
            => new(ParseNodeKind.Type, token);

        [Rule("literal : BinIntegerLit | DecIntegerLit | HexIntegerLit | FloatLit | StringLit | Identifier")]
        public static ParseNode2 Literal(Token token)
            => new(ParseNodeKind.Literal, token);
    }
}
