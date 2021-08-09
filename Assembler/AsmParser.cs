using System.Collections.Generic;
using System.Collections.Immutable;
using Yoakke.Lexer;
using Yoakke.Parser;
using Yoakke.Parser.Attributes;

namespace Caylang.Assembler
{
    using Token = IToken<TokenType>;

    [Parser(typeof(TokenType))]
    public partial class AsmParser
    {
        [Rule("start : block*")]
        public static ParseNode Start(IReadOnlyList<ParseNode> children)
            => new("start", children);

        [Rule("block : struct")]
        [Rule("block : definition")]
        [Rule("block : function")]
        public static ParseNode Block(ParseNode child) => child;

        [Rule("definition : DefineKw identifier type literal")]
        public static ParseNode Definition(Token defineKeyword, ParseNode identifier, ParseNode type, ParseNode literal)
        {
            var defineNode = new ParseNode("keyword", defineKeyword);

            return new("definition", defineNode, identifier, type, literal);
        }

        [Rule("struct : StructKw identifier type_list")]
        public static ParseNode Struct(Token structKeyword, ParseNode identifier, ParseNode types)
        {
            var structNode = new ParseNode("keyword", structKeyword);

            return new("struct", structNode, identifier, types);
        }

        [Rule("type_list : (type (',' type)*)")]
        public static ParseNode TypeList(Punctuated<ParseNode, Token> elements)
            => new("type_list", elements.Values.ToImmutableArray());

        [Rule("type : TypeKw")]
        public static ParseNode Type(Token token)
            => new("type", token);

        [Rule("args : ArgsKw '=' DecIntegerLit")]
        public static ParseNode ArgsParam(Token keyword, Token _, Token num)
            => new("args", new("keyword", keyword), new("integer", num));

        [Rule("locals : LocalsKw '=' DecIntegerLit")]
        public static ParseNode LocalsParam(Token keyword, Token _, Token num)
            => new("locals", new("keyword", keyword), new("integer", num));

        [Rule("params : locals ',' args")]
        [Rule("params : args ',' locals")]
        public static ParseNode FunctionParams(ParseNode left, Token _, ParseNode right)
        {
            if (left.Kind == "locals")
            {
                return new("params", left, right);
            }
            else
            {
                return new("params", right, left);
            }
        }

        [Rule("function : FuncKw identifier params statement+")]
        public static ParseNode Function(Token keyword, ParseNode identifier, ParseNode parameters, IReadOnlyList<ParseNode> statements)
            => new("function", new("keyword", keyword), identifier, parameters, new("statements", statements));

        [Rule("nullary_instruction : InstructionKw type?")]
        public static ParseNode NullaryInstruction(Token instructionToken, ParseNode? type)
        {
            var builder = ImmutableArray.CreateBuilder<ParseNode>();
            builder.Add(new ParseNode("instruction", instructionToken));
            if (type is not null)
            {
                builder.Add(type);
            }

            return new("nullary_instruction", builder.ToImmutable());
        }

        [Rule("statement : nullary_instruction | unary_instruction | label")]
        public static ParseNode Statement(ParseNode statement)
            => new("statement", statement);

        [Rule("unary_instruction : InstructionKw type? literal?")]
        public static ParseNode NullaryInstruction(Token instructionToken, ParseNode? type, ParseNode? literal)
        {
            var builder = ImmutableArray.CreateBuilder<ParseNode>();
            builder.Add(new ParseNode("instruction", instructionToken));
            if (type is not null)
            {
                builder.Add(type);
            }
            if (literal is not null)
            {
                builder.Add(literal);
            }

            return new("unary_instruction", builder.ToImmutable());
        }

        [Rule("label : identifier ':'")]
        public static ParseNode Label(ParseNode identifier, Token _)
            => new("label", identifier);

        [Rule("literal : number | string | identifier")]
        public static ParseNode Literal(ParseNode child)
            => new("literal", child);

        [Rule("number : (Plus | Minus)? (integer | float)")]
        public static ParseNode Number(Token? signToken, ParseNode numberToken)
        {
            var builder = ImmutableArray.CreateBuilder<ParseNode>();
            if (signToken is not null)
            {
                builder.Add(new("sign", signToken));
            }
            builder.Add(new("literal", numberToken));

            return new("number", builder.ToImmutable());
        }

        [Rule("integer : BinIntegerLit | DecIntegerLit | HexIntegerLit")]
        public static ParseNode Integer(Token token) => new("integer", token);

        [Rule("float : FloatLit")]
        public static ParseNode Float(Token token) => new("float", token);

        [Rule("identifier : Identifier")]
        public static ParseNode Identifier(Token token) => new("identifier", token);

        [Rule("string : StringLit")]
        public static ParseNode String(Token token) => new("string", token);
    }
}
