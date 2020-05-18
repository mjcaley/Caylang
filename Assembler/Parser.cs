using Caylang.Assembler.ParseTree;
using System.Collections.Generic;
using System.Linq;

namespace Caylang.Assembler
{
    public class Parser
    {
        #region Error recovery token types
        private static IEnumerable<TokenType> NullaryInstructionTypes { get; } = new[]
        {
            TokenType.Halt,
            TokenType.Noop,
            TokenType.Pop,
            TokenType.Add,
            TokenType.Subtract,
            TokenType.Multiply,
            TokenType.Divide,
            TokenType.Modulo,
            TokenType.TestEqual,
            TokenType.TestNotEqual,
            TokenType.TestGreaterThan,
            TokenType.TestLessThan,
            TokenType.Return
        };

        private static IEnumerable<TokenType> UnaryInstructionTypes { get; } = new[]
        {
            TokenType.LoadConst,
            TokenType.LoadLocal,
            TokenType.StoreLocal,
            TokenType.Jump,
            TokenType.JumpTrue,
            TokenType.JumpFalse,
            TokenType.NewStruct,
            TokenType.NewArray,
            TokenType.CallFunc,
            TokenType.LoadField,
            TokenType.StoreField
        };

        private static IEnumerable<TokenType> InstructionTypes { get; } =
            NullaryInstructionTypes.Union(UnaryInstructionTypes);

        private static IEnumerable<TokenType> StatementTypes { get; } = new[]
        {
            TokenType.EndOfFile,
            TokenType.Define,
            TokenType.Func
        };

        static private IEnumerable<TokenType> InstructionRecovery { get; } =
            StatementTypes.Union(UnaryInstructionTypes).Union(NullaryInstructionTypes);

        private static IEnumerable<TokenType> ReturnTypes { get; } = new[]
        {
            TokenType.Integer8Type,
            TokenType.Integer16Type,
            TokenType.Integer32Type,
            TokenType.Integer64Type,
            TokenType.UInteger8Type,
            TokenType.UInteger16Type,
            TokenType.UInteger32Type,
            TokenType.UInteger64Type,
            TokenType.Float32Type,
            TokenType.Float64Type,
            TokenType.AddressType
        };
        #endregion

        private readonly IEnumerator<Token> tokens;

        public Parser(IEnumerator<Token> tokens)
        {
            this.tokens = tokens;
            Advance();
            Advance();
        }

        public Parser(IEnumerable<Token> tokens) : this(tokens.GetEnumerator()) { }

        public Token? Current { get; private set; }

        public Token? Next { get; private set; }

        #region Parser utility methods
        private void Advance()
        {
            Current = Next;
            if (tokens.MoveNext())
            {
                Next = tokens.Current;
            }
            else
            {
                Next = null;
            }
        }

        private List<Token?> CollectUntil(IEnumerable<TokenType> types)
        {
            var collected = new List<Token?>();
            while (Current != null && !types.Contains(Current.Type))
            {
                collected.Add(Current);
                Advance();
            }

            return collected;
        }

        private bool Match(TokenType type)
        {
            if (Current != null)
            {
                return Current.Type == type;
            }

            return false;
        }

        private bool Match(IEnumerable<TokenType> types)
        {
            foreach (var type in types)
            {
                if (Match(type))
                {
                    return true;
                }
            }

            return false;
        }

        private Token? Expect(TokenType type)
        {
            if (Current?.Type == type)
            {
                var token = Current;
                Advance();
                
                return token;
            }

            return null;
        }

        private Token? Expect(IEnumerable<TokenType> types)
        {
            foreach (var tokenType in types)
            {
                if (Current?.Type == tokenType)
                {
                    var token = Current;
                    Advance();

                    return token;
                }
            }

            return null;
        }

        bool Expect(TokenType type, out Token token)
        {
            var expected = Expect(type);
            if (expected != null)
            {
                token = expected;
                return true;
            }

            token = new Token(TokenType.Error, 0);
            return false;
        }

        bool Expect(IEnumerable<TokenType> types, out Token token)
        {
            var expected = Expect(types);
            if (expected != null)
            {
                token = expected;
                return true;
            }

            token = new Token(TokenType.Error, 0);
            return false;
        }
        #endregion

        #region Recursive descent methods
        public ParseNode Start()
        {
            var definitions = new List<ParseNode>();
            var functions = new List<ParseNode>();
            var errors = new List<ParseNode>();

            while (!Match(TokenType.EndOfFile))
            {
                if (Match(TokenType.Func))
                {
                    functions.Add(ParseFunction());
                }
                else if (Match(TokenType.Define))
                {
                    definitions.Add(ParseDefinition());
                }
                else
                {
                    errors.Add(new UnexpectedTokenError(CollectUntil(StatementTypes)));
                }
            }

            return new Tree(definitions, functions, errors);
        }

        public ParseNode ParseDefinition()
        {
            if (
                Expect(TokenType.Define, out _) &&
                Expect(TokenType.Identifier, out var name) &&
                Expect(TokenType.Equal, out _)
            )
            {
                var operand = ParseLiteral();
                return new Definition(name, operand);
            }
            
            return new UnexpectedTokenError(Current);
        }

        public ParseNode ParseFunction()
        {
            if (
                Expect(TokenType.Func, out _) &&
                Expect(TokenType.Identifier, out var name) &&

                Expect(TokenType.Locals, out _) &&
                Expect(TokenType.Equal, out _) &&
                Expect(TokenType.IntegerLiteral, out var locals) &&
                
                Expect(TokenType.Args, out _) &&
                Expect(TokenType.Equal, out _) &&
                Expect(TokenType.IntegerLiteral, out var args)
                )
            {
                var statements = ParseStatements();
                
                return new FunctionNode(name, locals, args, statements);
            }

            return new UnexpectedTokenError(Current);
        }
        
        public List<ParseNode> ParseStatements()
        {
            var instructions = new List<ParseNode>();
            
            while (!Match(StatementTypes))
            {
                if (Match(TokenType.Identifier))
                {
                    instructions.Add(ParseLabel());
                }
                else if (Match(InstructionTypes))
                {
                    instructions.Add(ParseInstruction());
                }
                else
                {
                    instructions.Add(
                        new UnexpectedTokenError(CollectUntil(InstructionRecovery))
                        );
                    break;
                }
            }

            return instructions;
        }

        public ParseNode ParseLabel()
        {
                if (Expect(TokenType.Identifier, out var name) && Expect(TokenType.Colon, out _))
                {
                    return new LabelStatement(name);
                }
         
                return new UnexpectedTokenError(Current);
        }

        public ParseNode ParseInstruction()
        {
            if (Match(NullaryInstructionTypes))
            {
                return ParseNullaryInstruction();
            }
            else if (Match(UnaryInstructionTypes))
            {
                return ParseUnaryInstruction();
            }
            
            return new UnexpectedTokenError(
                CollectUntil(InstructionRecovery)
            );
        }

        private ParseNode ParseNullaryInstruction()
        {
            if (Expect(NullaryInstructionTypes, out var instruction))
            {
                var returnType = Expect(ReturnTypes) ?? new Token(TokenType.VoidType, 0);
                
                return new NullaryInstruction(instruction, returnType);
            }

            return new UnexpectedTokenError(Current);
        }

        private ParseNode ParseUnaryInstruction()
        {
            if (Expect(UnaryInstructionTypes, out var instruction))
            {
                var returnType = Expect(ReturnTypes) ?? new Token(TokenType.VoidType, 0);
                var operand = ParseLiteral();
                
                return new UnaryInstruction(instruction, returnType, operand);
            }
            
            return new UnexpectedTokenError(Current);
        }

        public ParseNode ParseLiteral() =>
            Current?.Type switch
            {
                TokenType.StringLiteral => ParseStringLiteral(),
                TokenType.Identifier => ParseIdentifierLiteral(),
                TokenType.Negative => ParseNumberLiteral(),
                TokenType.IntegerLiteral => ParseNumberLiteral(),
                TokenType.FloatLiteral => ParseNumberLiteral(),
                _ => new UnexpectedTokenError(Current)
            };

        private ParseNode ParseNumberLiteral()
        {
            Expect(TokenType.Negative, out var op);

            var numberLiteral = Current?.Type switch
            {
                TokenType.IntegerLiteral => ParseIntegerLiteral(),
                TokenType.FloatLiteral => ParseFloatLiteral(),
                _ => new UnexpectedTokenError(Current)
            };
            
            return new UnaryExpression(op, numberLiteral);
        }

        public ParseNode ParseIntegerLiteral()
        {
            if (Current?.Type == TokenType.IntegerLiteral)
            {
                ParseNode integerLiteral = Next?.Type switch
                {
                    TokenType.Integer8Type => new Integer8Literal(Current),
                    TokenType.Integer16Type => new Integer16Literal(Current),
                    TokenType.Integer32Type => new Integer32Literal(Current),
                    TokenType.Integer64Type => new Integer64Literal(Current),
                    TokenType.UInteger8Type => new UnsignedInteger8Literal(Current),
                    TokenType.UInteger16Type => new UnsignedInteger16Literal(Current),
                    TokenType.UInteger32Type => new UnsignedInteger32Literal(Current),
                    TokenType.UInteger64Type => new UnsignedInteger64Literal(Current),
                    _ => new UnexpectedTokenError(Current, Next)
                };
                Advance();
                Advance();

                return integerLiteral;
            }
            else
            {
                return new UnexpectedTokenError(Current, Next);
            }
        }

        public ParseNode ParseFloatLiteral()
        {
            if (Current?.Type == TokenType.FloatLiteral)
            {
                ParseNode floatLiteral = Next?.Type switch
                {
                    TokenType.Float32Type => new Float32Literal(Current),
                    TokenType.Float64Type => new Float64Literal(Current),
                    _ => new UnexpectedTokenError(Current, Next)
                };
                Advance();
                Advance();

                return floatLiteral;
            }
            
            return new UnexpectedTokenError(Current, Next);
        }

        public ParseNode ParseStringLiteral()
        {
            if (Current?.Type == TokenType.StringLiteral)
            {
                var stringLiteral = new StringLiteral(Current);
                Advance();

                if (Current?.Type == TokenType.StringType)
                {
                    Advance();
                }

                return stringLiteral;
            }
            else
            {
                return new UnexpectedTokenError(Current);
            }
        }
        
        public ParseNode ParseIdentifierLiteral()
        {
            if (Current?.Type == TokenType.Identifier)
            {
                var identifier = new IdentifierLiteral(Current);
                Advance();
                
                return identifier;
            }
            else
            {
                return new UnexpectedTokenError(Current);
            }
        }
        #endregion
    }
}
