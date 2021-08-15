using System.Collections.Generic;
using System.Collections.Immutable;
using Yoakke.Lexer;
using Yoakke.Parser;
using Yoakke.Parser.Attributes;
using Caylang.Assembler.ParseTree;

namespace Caylang.Assembler
{
    using Token = IToken<TokenType>;

    [Parser(typeof(TokenType))]
    public partial class AsmParser
    {
        [Rule("module : block*")]
        public static Module Module(IReadOnlyList<Branch> children)
            => new() { Children = children.ToImmutableArray<Node>() };

        [Rule("block : struct")]
        [Rule("block : definition")]
        [Rule("block : function")]
        public static Branch Block(Branch child) => child;

        [Rule("definition : DefineKw identifier type literal")]
        public static Definition Definition(Token defineKeyword, Identifier identifier, Type type, Literal literal)
        {
            var defineNode = new Keyword { Token = defineKeyword };

            return new() { Children = ImmutableArray.Create<Node>(defineNode, identifier, type, literal) };
        }

        [Rule("struct : StructKw identifier types")]
        public static Struct Struct(Token structKeyword, Identifier identifier, Types types)
        {
            var structNode = new Keyword { Token = structKeyword };

            return new() { Children = ImmutableArray.Create<Node>(structNode, identifier, types) };
        }

        [Rule("types : (type (',' type)*)")]
        public static Types Types(Punctuated<Type, Token> elements)
            => new() { Children = elements.Values.ToImmutableArray<Node>() };

        [Rule("type : TypeKw")]
        public static Type Type(Token token)
            => new() { Token = token };

        [Rule("args : ArgsKw '=' DecIntegerLit")]
        public static ArgsParameter ArgsParam(Token keyword, Token _, Token num)
            => new()
            {
                Children = ImmutableArray.Create<Node>(
                    new Keyword { Token = keyword },
                    new Integer { Token = num })
            };

        [Rule("locals : LocalsKw '=' DecIntegerLit")]
        public static LocalsParameter LocalsParam(Token keyword, Token _, Token num)
            => new()
            {
                Children = ImmutableArray.Create<Node>(
                    new Keyword { Token = keyword },
                    new Integer { Token = num })
            };

        [Rule("params : locals ',' args")]
        [Rule("params : args ',' locals")]
        public static FunctionParameters FunctionParams(Parameter left, Token _, Parameter right)
        {
            switch (left)
			{
                case LocalsParameter:
                    return new() { Children = ImmutableArray.Create<Node>(left, right) };
                default:
                    return new() { Children = ImmutableArray.Create<Node>(right, left) };
            }
        }

        [Rule("function : FuncKw identifier params statement+")]
        public static Function Function(Token keyword, Identifier identifier, FunctionParameters parameters, IReadOnlyList<Statement> statements)
            => new()
            {
                Children = ImmutableArray.Create<Node>(
                    new Keyword { Token = keyword },
                    identifier,
                    parameters,
                    new Statements { Children = statements.ToImmutableArray<Node>() })
            };

        [Rule("statement : nullary_instruction")]
        [Rule("statement : unary_instruction")]
        [Rule("statement : label")]
        public static Statement Statement(Statement statement)
            => statement;

        [Rule("nullary_instruction : InstructionKw type?")]
        public static NullaryInstruction NullaryInstruction(Token instructionToken, Type? type)
        {
            var builder = ImmutableArray.CreateBuilder<Node>();
            builder.Add(new Instruction { Token = instructionToken });
            if (type is not null)
            {
                builder.Add(type);
            }

            return new() { Children = builder.ToImmutable() };
        }

        [Rule("unary_instruction : InstructionKw type? literal?")]
        public static NullaryInstruction NullaryInstruction(Token instructionToken, Type? type, Literal? literal)
        {
            var builder = ImmutableArray.CreateBuilder<Node>();
            builder.Add(new Instruction { Token = instructionToken });
            if (type is not null)
            {
                builder.Add(type);
            }
            if (literal is not null)
			{
                builder.Add(literal);
			}

            return new() { Children = builder.ToImmutable() };
        }

        [Rule("label : identifier ':'")]
        public static Label Label(Identifier identifier, Token _)
            => new() { Children = ImmutableArray.Create<Node>(identifier) };

        [Rule("literal : number")]
        [Rule("literal : string")]
        [Rule("literal : identifier")]
        public static Literal Literal(Node child)
            => new() { Children = ImmutableArray.Create<Node>(child) };

        [Rule("number : (Plus | Minus)? integer")]
        [Rule("number : (Plus | Minus)? float")]
        public static Number Number(Token? signToken, Node number)
        {
            var builder = ImmutableArray.CreateBuilder<Node>();
            if (signToken is not null)
            {
                builder.Add(new Sign { Token = signToken });
            }
            builder.Add(number);

            return new() { Children = builder.ToImmutable() };
        }

        [Rule("integer : BinIntegerLit")]
        [Rule("integer : DecIntegerLit")]
        [Rule("integer : HexIntegerLit")]
        public static Integer Integer(Token token) => new() { Token = token };

        [Rule("float : FloatLit")]
        public static Float Float(Token token) => new() { Token = token };

        [Rule("identifier : Identifier")]
        public static Identifier Identifier(Token token) => new() { Token = token };

        [Rule("string : StringLit")]
        public static String String(Token token) => new() { Token = token };
    }
}
