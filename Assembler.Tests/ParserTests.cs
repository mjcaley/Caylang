using Caylang.Assembler.ParseTree;
using Pidgin;
using Xunit;

namespace Caylang.Assembler.Tests
{
    public class ParserTests
    {
        [Fact]
        public void PidginCanConsumeLexerTokens()
        {
            var tokens = Lexer.LexString("42 i32\n");
            var result = Parser.CaylangToken(TokenType.IntegerLiteral).ParseOrThrow(tokens);

            Assert.Equal(tokens[0], result);
        }

        [Theory]
        [InlineData("42 i8", 42)]
        [InlineData("-42 i8", -42)]
        public void ParsesInteger8Literal(string testInput, sbyte expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<Integer8Literal>(result.Value);
            Assert.Equal(expected, literal.Value);
        }

        [Theory]
        [InlineData("42 i16", 42)]
        [InlineData("-42 i16", -42)]
        public void ParsesInteger16Literal(string testInput, short expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<Integer16Literal>(result.Value);
            Assert.Equal(expected, literal.Value);
        }

        [Theory]
        [InlineData("42 i32", 42)]
        [InlineData("-42 i32", -42)]
        public void ParsesInteger32Literal(string testInput, int expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<Integer32Literal>(result.Value);
            Assert.Equal(expected, literal.Value);
        }

        [Theory]
        [InlineData("42 i64", 42)]
        [InlineData("-42 i64", -42)]
        public void ParsesInteger64Literal(string testInput, long expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<Integer64Literal>(result.Value);
            Assert.Equal(expected, literal.Value);
        }

        [Fact]
        public void ParsesUnsignedInteger8Literal()
        {
            var tokens = Lexer.LexString("42 u8");
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<UnsignedInteger8Literal>(result.Value);
            Assert.Equal((byte)42, literal.Value);
        }

        [Fact]
        public void ParsesUnsignedInteger16Literal()
        {
            var tokens = Lexer.LexString("42 u16");
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<UnsignedInteger16Literal>(result.Value);
            Assert.Equal((ushort)42, literal.Value);
        }

        [Fact]
        public void ParsesUnsignedInteger32Literal()
        {
            var tokens = Lexer.LexString("42 u32");
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<UnsignedInteger32Literal>(result.Value);
            Assert.Equal(42u, literal.Value);
        }

        [Fact]
        public void ParsesUnsignedInteger64Literal()
        {
            var tokens = Lexer.LexString("42 u64");
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<UnsignedInteger64Literal>(result.Value);
            Assert.Equal(42UL, literal.Value);
        }

        [Theory]
        [InlineData("42.42 f32", 42.42)]
        [InlineData("-42.42 f32", -42.42)]
        public void ParsesFloat32Literal(string testInput, float expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<Float32Literal>(result.Value);
            Assert.Equal(expected, literal.Value);
        }

        [Theory]
        [InlineData("42.42 f64", 42.42)]
        [InlineData("-42.42 f64", -42.42)]
        public void ParsesFloat64Literal(string testInput, double expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<Float64Literal>(result.Value);
            Assert.Equal(expected, literal.Value);
        }

        [Theory]
        [InlineData("\"String literal\"")]
        [InlineData("\"String literal\" str")]
        public void ParsesStringLiteral(string testInput)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<StringLiteral>(result.Value);
            Assert.Equal("String literal", literal.Value);
        }

        [Fact]
        public void ParsesIdentifierLiteral()
        {
            var tokens = Lexer.LexString("identifier123");
            var result = Parser.Literal.Parse(tokens);

            Assert.True(result.Success);
            var literal = Assert.IsType<IdentifierLiteral>(result.Value);
            Assert.Equal("identifier123", literal.Value);
        }

        [Fact]
        public void ParsesLabelStatement()
        {
            var tokens = Lexer.LexString("label:");
            var result = Parser.Statement.Parse(tokens);
            
            Assert.True(result.Success);
            var literal = Assert.IsType<Label>(result.Value);
            Assert.Equal("label", literal.Name);
        }

        [Theory]
        [InlineData("halt", Instruction.Halt)]
        [InlineData("nop", Instruction.Noop)]
        [InlineData("pop", Instruction.Pop)]
        [InlineData("ret", Instruction.Return)]
        public void ParsesNullaryInstruction(string testInput, Instruction expectedInstruction)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Statement.Parse(tokens);
            
            Assert.True(result.Success);
            var statement = Assert.IsType<NullaryInstruction>(result.Value);
            Assert.Equal(expectedInstruction, statement.Instruction);
        }

        [Theory]
        [InlineData("add", Instruction.Add)]
        [InlineData("sub", Instruction.Subtract)]
        [InlineData("mul", Instruction.Multiply)]
        [InlineData("div", Instruction.Divide)]
        [InlineData("mod", Instruction.Modulo)]
        public void ParsesArithmeticInstruction(string testInput, Instruction expectedInstruction)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Statement.Parse(tokens);

            Assert.True(result.Success);
            var statement = Assert.IsType<NullaryInstruction>(result.Value);
            Assert.Equal(expectedInstruction, statement.Instruction);
        }

        [Theory]
        [InlineData("jmp function", Instruction.Jump)]
        [InlineData("jmpt function", Instruction.JumpTrue)]
        [InlineData("jmpf function", Instruction.JumpFalse)]
        public void ParsesJumpInstruction(string testInput, Instruction expectedInstruction)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Statement.Parse(tokens);

            Assert.True(result.Success);
            var statement = Assert.IsType<UnaryInstruction>(result.Value);
            Assert.Equal(expectedInstruction, statement.Instruction);
            Assert.IsType<IdentifierLiteral>(statement.Operand);
        }

        [Fact]
        public void ParsesCallFuncInstruction()
        {
            var tokens = Lexer.LexString("callfunc function");
            var result = Parser.Statement.Parse(tokens);

            Assert.True(result.Success);
            var statement = Assert.IsType<UnaryInstruction>(result.Value);
            Assert.IsType<IdentifierLiteral>(statement.Operand);
        }

        [Fact]
        public void ParsesCallVirtualInstruction()
        {
            var tokens = Lexer.LexString("callvirt");
            var result = Parser.Statement.Parse(tokens);

            Assert.True(result.Success);
            var statement = Assert.IsType<NullaryInstruction>(result.Value);
            Assert.Equal(Instruction.CallVirtual, statement.Instruction);
        }

        [Theory]
        [InlineData("ldlocal 42", Instruction.LoadLocal)]
        [InlineData("stlocal 42", Instruction.StoreLocal)]
        public void ParsesLocalInstructions(string testInput, Instruction expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Statement.Parse(tokens);
            
            Assert.True(result.Success);
            var statement = Assert.IsType<UnaryInstruction>(result.Value);
            Assert.Equal(expected, statement.Instruction);
        }

        [Fact]
        public void ParsesLoadConstInstruction()
        {
            var tokens = Lexer.LexString("ldconst 42 i32");
            var result = Parser.Statement.Parse(tokens);
            
            Assert.True(result.Success);
            var statement = Assert.IsType<UnaryInstruction>(result.Value);
            Assert.Equal(Instruction.LoadConst, statement.Instruction);
        }

        [Fact]
        public void ParsesNewStructInstruction()
        {
            var tokens = Lexer.LexString("newstruct StructDef");
            var result = Parser.Statement.Parse(tokens);

            Assert.True(result.Success);
            var statement = Assert.IsType<UnaryInstruction>(result.Value);
            Assert.Equal(Instruction.NewStruct, statement.Instruction);
            Assert.IsType<IdentifierLiteral>(statement.Operand);
        }

        [Theory]
        [InlineData("ldfield 42", Instruction.LoadField)]
        [InlineData("stfield 42", Instruction.StoreField)]
        public void ParsesFieldInstructions(string testInput, Instruction expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Statement.Parse(tokens);
            
            Assert.True(result.Success);
            var statement = Assert.IsType<UnaryInstruction>(result.Value);
            Assert.Equal(expected, statement.Instruction);
        }

        [Fact]
        public void ParsesNewArrayInstruction()
        {
            var tokens = Lexer.LexString("newarray StructDef");
            var result = Parser.Statement.Parse(tokens);

            Assert.True(result.Success);
            var statement = Assert.IsType<UnaryInstruction>(result.Value);
            Assert.Equal(Instruction.NewArray, statement.Instruction);
            Assert.IsType<IdentifierLiteral>(statement.Operand);
        }

        [Theory]
        [InlineData("ldelem", Instruction.LoadElement)]
        [InlineData("stelem", Instruction.StoreElement)]
        public void ParsesElementInstructions(string testInput, Instruction expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Statement.Parse(tokens);
            
            Assert.True(result.Success);
            var statement = Assert.IsType<NullaryInstruction>(result.Value);
            Assert.Equal(expected, statement.Instruction);
        }

        [Theory]
        [InlineData("testeq", Instruction.TestEqual)]
        [InlineData("testne", Instruction.TestNotEqual)]
        [InlineData("testgt", Instruction.TestGreaterThan)]
        [InlineData("testlt", Instruction.TestLessThan)]
        public void ParsesTestInstruction(string testInput, Instruction expected)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Statement.Parse(tokens);

            Assert.True(result.Success);
            var statement = Assert.IsType<NullaryInstruction>(result.Value);
            Assert.Equal(expected, statement.Instruction);
        }

        [Theory]
        [InlineData("func functionName args=1 locals=2\nret\n")]
        [InlineData("func functionName locals=2 args=1\nret\n")]
        public void ParsesFunction(string testInput)
        {
            var tokens = Lexer.LexString(testInput);
            var result = Parser.Definition.Parse(tokens);

            Assert.True(result.Success);
            var definition = Assert.IsType<FunctionDefinition>(result.Value);
            Assert.Equal("functionName", definition.Name);
            Assert.Equal(1, definition.Parameters.Arguments);
            Assert.Equal(2, definition.Parameters.Locals);
        }

        [Fact]
        public void ParsesVariable()
        {
            var tokens = Lexer.LexString("define number = 1 i8");
            var result = Parser.Definition.Parse(tokens);

            Assert.True(result.Success);
            var definition = Assert.IsType<ConstantDefinition>(result.Value);
            Assert.Equal("number", definition.Name);
        }

        [Fact]
        public void ParsesStruct()
        {
            var tokens = Lexer.LexString("struct object\n\tu32,\n\taddr\n");
            var result = Parser.Definition.Parse(tokens);

            Assert.True(result.Success);
            var definition = Assert.IsType<StructDefinition>(result.Value);
            Assert.Equal("object", definition.Name);
        }
    }
}
