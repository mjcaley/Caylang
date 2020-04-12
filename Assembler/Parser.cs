using Caylang.Assembler.ParseTree;
using System.Collections.Generic;
using System.Linq;
using InstructionType = Caylang.Assembler.ParseTree.InstructionType;

namespace Caylang.Assembler
{
    public class Parser
	{
        private readonly IEnumerator<Token> tokens;

        public Token? Current { get; private set; }

        public Token? Next { get; private set; }

        public List<ParserException> Errors { get; } = new List<ParserException>();

        public Parser(IEnumerator<Token> tokens)
        {
            this.tokens = tokens;
            Advance();
            Advance();
        }

        public Parser(IEnumerable<Token> tokens) : this(tokens.GetEnumerator()) { }

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

        private void SkipTo(params TokenType[] types)
        {
            while (Current != null && !types.Contains(Current.Type))
            {
                Advance();
            }
        }

        private bool Match(params TokenType[] types)
        {
            if (Current != null)
            {
                return types.Contains(Current.Type);
            }

            return false;
        }

        private Token Expect(TokenType type)
        {
            if (Current?.Type == type)
            {
                var token = Current;
                Advance();
                
                return token;
            }
            else
            {
                throw new UnexpectedTokenException(Current);
            }
        }

        public Tree Start()
        {
            var tree = new Tree();

            while (!Match(TokenType.EndOfFile))
            {
                if (Match(TokenType.Func))
                {
                    tree.Functions.Add(ParseFunction());
                }
                else if (Match(TokenType.Define))
                {
                    tree.Definitions.Add(ParseDefinition());
                }
                else
                {
                    Errors.Add(new UnexpectedTokenException(Current));
                    SkipTo(TokenType.Func, TokenType.Define, TokenType.EndOfFile);
                }
            }

            return tree;
        }

        public Definition ParseDefinition()
        {
            var line = Current?.Line ?? 0;

            Expect(TokenType.Define);
            var name = Expect(TokenType.Identifier).Value;
            Expect(TokenType.Equal);
            var operand = ParseOperand();
            
            return new Definition(name, operand, line);
        }

        public Function ParseFunction()
        {
            var line = Current?.Line ?? 0;

            Expect(TokenType.Func);
            var name = Expect(TokenType.Identifier).Value;

            Expect(TokenType.Locals);
            Expect(TokenType.Equal);
            int locals;
            {
                var localToken = Expect(TokenType.IntegerLiteral);
                var localResult = int.TryParse(localToken.Value, out locals);
                if (!localResult)
                {
                    throw new IntegerConversionException(localToken);
                }
            }
            
            Expect(TokenType.Args);
            Expect(TokenType.Equal);
            int args;
            {
                var argToken = Expect(TokenType.IntegerLiteral);
                var argsResult = int.TryParse(argToken.Value, out args);
                if (!argsResult)
                {
                    throw new IntegerConversionException(argToken);
                }
            }

            var statements = ParseStatements();
            
            return new Function(name, locals, args, line, statements);
        }
        
        public List<Statement> ParseStatements()
        {
            var instructions = new List<Statement>();
            
            try
            {
                while (!Match(TokenType.EndOfFile, TokenType.Func, TokenType.Define))
                {
                    if (Current?.Type == TokenType.Identifier)
                    {
                        instructions.Add(ParseLabel());
                    }
                    else
                    {
                        instructions.Add(ParseInstruction());
                    }
                }
            }
            catch (ParserException e)
            {
                Errors.Add(e);
                SkipTo(TokenType.EndOfFile, TokenType.Func, TokenType.Define);
            }

            return instructions;
        }

        public LabelStatement ParseLabel()
        {
            if (Current?.Type == TokenType.Identifier && Next?.Type == TokenType.Colon)
            {
                var line = Current.Line;
                var name = Current.Value;
                
                Advance();
                Advance();
                
                return new LabelStatement(name, line);
            }
            else
            {
                throw new UnexpectedTokenException(Current);
            }
        }

        public InstructionStatement ParseInstruction()
        {
            switch (Current?.Type)
            {
                case TokenType.Halt:
                case TokenType.Noop:
                case TokenType.Pop:
                case TokenType.Add:
                case TokenType.Subtract:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Modulo:
                case TokenType.TestEqual:
                case TokenType.TestNotEqual:
                case TokenType.TestGreaterThan:
                case TokenType.TestLessThan:
                case TokenType.Return:
                    return ParseNullaryInstruction();
                case TokenType.LoadConst:
                case TokenType.LoadLocal:
                case TokenType.StoreLocal:
                case TokenType.Jump:
                case TokenType.JumpTrue:
                case TokenType.JumpFalse:
                case TokenType.NewStruct:
                case TokenType.NewArray:
                case TokenType.CallFunc:
                case TokenType.LoadField:
                case TokenType.StoreField:
                    return ParseUnaryInstruction();
                default:
                    throw new UnexpectedTokenException(Current);
            }
        }
        
        private NullaryInstruction ParseNullaryInstruction()
        {
            var line = Current?.Line ?? 0;

            var instruction = Current?.Type switch
            {
                TokenType.Halt => Instruction.Halt,
                TokenType.Noop => Instruction.Noop,
                TokenType.Pop => Instruction.Pop,
                TokenType.Add => Instruction.Add,
                TokenType.Subtract => Instruction.Sub,
                TokenType.Multiply => Instruction.Mul,
                TokenType.Divide => Instruction.Div,
                TokenType.Modulo => Instruction.Mod,
                TokenType.TestEqual => Instruction.TestEQ,
                TokenType.TestNotEqual => Instruction.TestNE,
                TokenType.TestGreaterThan => Instruction.TestGT,
                TokenType.TestLessThan => Instruction.TestLT,
                TokenType.Return => Instruction.Ret,
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            InstructionType type = ParseInstructionType();
            
            return new NullaryInstruction(instruction, type, line);
        }

        private UnaryInstruction ParseUnaryInstruction()
        {
            var line = Current?.Line ?? 0;

            var instruction = Current?.Type switch
            {
                TokenType.LoadConst => Instruction.LdConst,
                TokenType.LoadLocal => Instruction.LdLocal,
                TokenType.StoreLocal => Instruction.StLocal,
                TokenType.Jump => Instruction.Jmp,
                TokenType.JumpTrue => Instruction.JmpT,
                TokenType.JumpFalse => Instruction.JmpF,
                TokenType.NewStruct => Instruction.NewStruct,
                TokenType.NewArray => Instruction.NewArray,
                TokenType.CallFunc => Instruction.CallFunc,
                TokenType.LoadField => Instruction.LdField,
                TokenType.StoreField => Instruction.StField,
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            var type = ParseInstructionType();
            var operand = ParseOperand();
            
            return new UnaryInstruction(instruction, type, operand, line);
        }

        public InstructionType ParseInstructionType()
        {
            var type = Current?.Type switch
            {
                TokenType.i8Type => InstructionType.Integer8,
                TokenType.u8Type => InstructionType.UInteger8,
                TokenType.i16Type => InstructionType.Integer16,
                TokenType.u16Type => InstructionType.UInteger16,
                TokenType.i32Type => InstructionType.Integer32,
                TokenType.u32Type => InstructionType.UInteger32,
                TokenType.i64Type => InstructionType.Integer64,
                TokenType.u64Type => InstructionType.UInteger64,
                TokenType.f32Type => InstructionType.FloatingPoint32,
                TokenType.f64Type => InstructionType.FloatingPoint64,
                TokenType.AddressType => InstructionType.Address,
                _ => InstructionType.Void
            };
            if (type != InstructionType.Void)
            {
                Advance();
            }

            return type;
        }

        public Operand ParseOperand()
        {
            var line = Current?.Line ?? 0;

            var literal = ParseLiteral();

            if (literal is IdentifierLiteral)
            {
                return new Operand(literal, OperandType.Unknown, line);
            }
            else if (literal is StringLiteral)
            {
                return new Operand(literal, OperandType.StringType, line);
            }
            else
            {
                var type = ParseOperandType();
                return new Operand(literal, type, line);
            }
        }

        public OperandType ParseOperandType()
        {
            var operandType = Current?.Type switch
            {
                TokenType.i8Type => OperandType.Integer8,
                TokenType.u8Type => OperandType.UInteger8,
                TokenType.i16Type => OperandType.Integer16,
                TokenType.u16Type => OperandType.UInteger16,
                TokenType.i32Type => OperandType.Integer32,
                TokenType.u32Type => OperandType.UInteger32,
                TokenType.i64Type => OperandType.Integer64,
                TokenType.u64Type => OperandType.UInteger64,
                TokenType.f32Type => OperandType.FloatingPoint32,
                TokenType.f64Type => OperandType.FloatingPoint64,
                TokenType.AddressType => OperandType.Address,
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            return operandType;
        }

        public Literal ParseLiteral()
        {
            Literal literal = Current?.Type switch
            {
                TokenType.Identifier => new IdentifierLiteral(Current.Value),
                TokenType.IntegerLiteral => new IntegerLiteral(Current.Value),
                TokenType.FloatLiteral => new FloatLiteral(Current.Value),
                TokenType.StringLiteral => new StringLiteral(Current.Value),
                _ => throw new UnexpectedTokenException(Current)
            };
            Advance();

            return literal;
        }
    }
}
