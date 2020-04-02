using System.Collections.Generic;
using Caylang.Assembler;
using Xunit;

namespace CayLang.Assembler.Tests
{
    public class ParserTests
    {
        [Fact]
        public void HaltSuccess()
        {
            var p = new Parser(
                new Token[]
                {
                    new Token(TokenType.Halt, 1)
                }
            );
            var n = p.Halt();

            Assert.Equal(Caylang.Assembler.ParseTree.Type.Void, n.ReturnType);
            Assert.Equal(Caylang.Assembler.ParseTree.InstructionType.Halt, n.Instruction);
            Assert.Equal(1, n.Line);
            
            Assert.Null(p.Current);
        }

        [Fact]
        public void HaltFail()
        {
            var p = new Parser(
                new Token[]
                {
                    new Token(TokenType.EndOfFile, 1)
                }
            );

            Assert.Throws<UnexpectedTokenException>(() => p.Halt());
        }

        [Fact]
        public void HaltFailNull()
        {
            var p = new Parser(System.Array.Empty<Token>());

            Assert.Throws<UnexpectedTokenException>(() => p.Halt());
        }

        [Fact]
        public void PopSuccess()
        {
            var p = new Parser(
                new[]
                {
                    new Token(TokenType.Pop, 1)
                }
            );
            var n = p.Pop();

            Assert.Equal(Caylang.Assembler.ParseTree.Type.Void, n.ReturnType);
            Assert.Equal(Caylang.Assembler.ParseTree.InstructionType.Pop, n.Instruction);
            Assert.Equal(1, n.Line);

            Assert.Null(p.Current);
        }

        [Fact]
        public void PopFail()
        {
            var p = new Parser(
                new[]
                {
                    new Token(TokenType.EndOfFile, 1)
                }
            );

            Assert.Throws<UnexpectedTokenException>(() => p.Pop());
        }

        [Fact]
        public void PopFailNull()
        {
            var p = new Parser(System.Array.Empty<Token>());

            Assert.Throws<UnexpectedTokenException>(() => p.Pop());
        }

        [Fact]
        public void AddSuccess()
        {
            var p = new Parser(
                new[]
                {
                    new Token(TokenType.Add, 1),
                    new Token(TokenType.i8Type, 1)
                }
            );
            var n = p.Add();

            Assert.Equal(Caylang.Assembler.ParseTree.Type.Integer8, n.ReturnType);
            Assert.Equal(Caylang.Assembler.ParseTree.InstructionType.Add, n.Instruction);
            Assert.Equal(1, n.Line);

            Assert.Null(p.Current);
        }

        [Fact]
        public void AddFailUnexpectedToken()
        {
            var p = new Parser(
                new[]
                {
                    new Token(TokenType.EndOfFile, 1)
                }
            );

            Assert.Throws<UnexpectedTokenException>(() => p.Add());
        }

        [Fact]
        public void AddFailNull()
        {
            var p = new Parser(System.Array.Empty<Token>());

            Assert.Throws<UnexpectedTokenException>(() => p.Add());
        }
    }
}
