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
        [Rule("module : block*")]
        public static ParseTreeBranch Module(IReadOnlyList<ParseTree> children)
            => new("module", children);

        [Rule("block : struct")]
        [Rule("block : definition")]
        [Rule("block : function")]
        public static ParseTree Block(ParseTree child) => child;

        [Rule("definition : DefineKw identifier type literal")]
        public static ParseTreeBranch Definition(Token defineKeyword, ParseTree identifier, ParseTree type, ParseTree literal)
        {
            var defineNode = new ParseTreeLeaf("keyword", defineKeyword);

            return new("definition", defineNode, identifier, type, literal);
        }

        [Rule("struct : StructKw identifier type_list")]
        public static ParseTreeBranch Struct(Token structKeyword, ParseTree identifier, ParseTree types)
        {
            var structNode = new ParseTreeLeaf("keyword", structKeyword);

            return new("struct", structNode, identifier, types);
        }

        [Rule("type_list : (type (',' type)*)")]
        public static ParseTreeBranch TypeList(Punctuated<ParseTreeLeaf, Token> elements)
            => new("type_list", elements.Values.ToImmutableArray());

        [Rule("type : TypeKw")]
        public static ParseTreeLeaf Type(Token token)
            => new("type", token);

        [Rule("args : ArgsKw '=' DecIntegerLit")]
        public static ParseTreeBranch ArgsParam(Token keyword, Token _, Token num)
            => new("args", new ParseTreeLeaf("keyword", keyword), new ParseTreeLeaf("integer", num));

        [Rule("locals : LocalsKw '=' DecIntegerLit")]
        public static ParseTreeBranch LocalsParam(Token keyword, Token _, Token num)
            => new("locals", new ParseTreeLeaf("keyword", keyword), new ParseTreeLeaf("integer", num));

        [Rule("params : locals ',' args")]
        [Rule("params : args ',' locals")]
        public static ParseTreeBranch FunctionParams(ParseTree left, Token _, ParseTree right)
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
        public static ParseTreeBranch Function(Token keyword, ParseTree identifier, ParseTree parameters, IReadOnlyList<ParseTree> statements)
            => new("function", new ParseTreeLeaf("keyword", keyword), identifier, parameters, new ParseTreeBranch("statements", statements));

        [Rule("nullary_instruction : InstructionKw type?")]
        public static ParseTreeBranch NullaryInstruction(Token instructionToken, ParseTree? type)
        {
            var builder = ImmutableArray.CreateBuilder<ParseTree>();
            builder.Add(new ParseTreeLeaf("instruction", instructionToken));
            if (type is not null)
            {
                builder.Add(type);
            }

            return new("nullary_instruction", builder.ToImmutable());
        }

        [Rule("statement : nullary_instruction | unary_instruction | label")]
        public static ParseTreeBranch Statement(ParseTree statement)
            => new("statement", statement);

        [Rule("unary_instruction : InstructionKw type? literal?")]
        public static ParseTreeBranch NullaryInstruction(Token instructionToken, ParseTree? type, ParseTree? literal)
        {
            var builder = ImmutableArray.CreateBuilder<ParseTree>();
            builder.Add(new ParseTreeLeaf("instruction", instructionToken));
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
        public static ParseTreeBranch Label(ParseTree identifier, Token _)
            => new("label", identifier);

        [Rule("literal : number | string | identifier")]
        public static ParseTreeBranch Literal(ParseTree child)
            => new("literal", child);

        [Rule("number : (Plus | Minus)? (integer | float)")]
        public static ParseTreeBranch Number(Token? signToken, ParseTree numberToken)
        {
            var builder = ImmutableArray.CreateBuilder<ParseTree>();
            if (signToken is not null)
            {
                builder.Add(new ParseTreeLeaf("sign", signToken));
            }
            builder.Add(new ParseTreeBranch("literal", numberToken));

            return new("number", builder.ToImmutable());
        }

        [Rule("integer : BinIntegerLit | DecIntegerLit | HexIntegerLit")]
        public static ParseTreeLeaf Integer(Token token) => new("integer", token);

        [Rule("float : FloatLit")]
        public static ParseTreeLeaf Float(Token token) => new("float", token);

        [Rule("identifier : Identifier")]
        public static ParseTreeLeaf Identifier(Token token) => new("identifier", token);

        [Rule("string : StringLit")]
        public static ParseTreeLeaf String(Token token) => new("string", token);
    }
}
