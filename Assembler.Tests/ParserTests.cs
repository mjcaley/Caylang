using System.Collections.Generic;
using Caylang.Assembler;
using Caylang.Assembler.ParseTree;
using Xunit;

namespace CayLang.Assembler.Tests
{
    public class ParserTests
    {
        [Theory]
        [InlineData(TokenType.Halt, Instruction.Halt)]
        [InlineData(TokenType.Noop, Instruction.Noop)]
        [InlineData(TokenType.Pop, Instruction.Pop)]
        [InlineData(TokenType.Add, Instruction.Add)]
        [InlineData(TokenType.Subtract, Instruction.Sub)]
        [InlineData(TokenType.Multiply, Instruction.Mul)]
        [InlineData(TokenType.Divide, Instruction.Div)]
        [InlineData(TokenType.Modulo, Instruction.Mod)]
        [InlineData(TokenType.TestEqual, Instruction.TestEQ)]
        [InlineData(TokenType.TestNotEqual, Instruction.TestNE)]
        [InlineData(TokenType.TestGreaterThan, Instruction.TestGT)]
        [InlineData(TokenType.TestLessThan, Instruction.TestLT)]
        [InlineData(TokenType.Return, Instruction.Ret)]
        public void ParsesNullaryInstruction(TokenType testInput, Instruction expected)
        {
            var parser = new Parser(new[] {new Token(testInput, 1), });
            var result = parser.ParseInstruction();
            
            Assert.IsType<NullaryInstruction>(result);
            Assert.Equal(expected, result.Instruction);
            Assert.Equal(1, result.Line);
        }

        [Theory]
        [InlineData(TokenType.LoadConst, Instruction.LdConst)]
        [InlineData(TokenType.LoadLocal, Instruction.LdLocal)]
        [InlineData(TokenType.StoreLocal, Instruction.StLocal)]
        [InlineData(TokenType.Jump, Instruction.Jmp)]
        [InlineData(TokenType.JumpTrue, Instruction.JmpT)]
        [InlineData(TokenType.JumpFalse, Instruction.JmpF)]
        [InlineData(TokenType.NewStruct, Instruction.NewStruct)]
        [InlineData(TokenType.NewArray, Instruction.NewArray)]
        [InlineData(TokenType.CallFunc, Instruction.CallFunc)]
        [InlineData(TokenType.LoadField, Instruction.LdField)]
        [InlineData(TokenType.StoreField, Instruction.StField)]
        public void ParsesUnaryInstruction(TokenType testInput, Instruction expected)
        {
            var parser = new Parser(new[]
            {
                new Token(testInput, 1),
                new Token(TokenType.Identifier, 2, "ident")
            });
            var result = parser.ParseInstruction();
            
            Assert.IsType<UnaryInstruction>(result);
            Assert.Equal(expected, result.Instruction);
            Assert.Equal(1, result.Line);
        }
    }
}
