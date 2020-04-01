using System;
using Caylang.Assembler;

namespace CayLang.Assembler
{
    public enum TokenType
    {
        Error,
        
        // Assembler instructions
        Halt,
        Noop,
        
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,

        LoadConst,
        LoadLocal,
        StoreLocal,
        Pop,

        TestEqual,
        TestNotEqual,
        TestGreaterThan,
        TestLessThan,

        Jump,
        JumpTrue,
        JumpFalse,

        CallFunc,
        CallInterface,

        Return,

        NewStruct,
        LoadField,
        StoreField,

        // Types
        PointerType,
        i8Type,
        u8Type,
        i16Type,
        u16Type,
        i32Type,
        u32Type,
        i64Type,
        u64Type,
        AddressType,
        f32Type,
        f64Type,
        StringType,

        // Literals
        IntegerLiteral,
        FloatLiteral,
        StringLiteral,
        Identifier,

        // Assembler tokens
        Func,
        Args,
        Locals,
        Define,
        Equal,
        Colon,
        Negative,

        EndOfFile
    }

    public class Token
    {
        public Token(TokenType type, int line) => (Type, Line) = (type, line);

        public TokenType Type { get; }
        public int Line { get; }
    }

    public class ValueToken<T> : Token
    {
        public ValueToken(TokenType type, int line, T value) : base(type, line) =>
            Value = value;

        public T Value { get; }
    }

    public class IntegerToken : ValueToken<ulong>
    {
        public IntegerToken(TokenType type, int line, ulong value) : base(type, line, value) { }
    }

    public class FloatToken : ValueToken<decimal>
    {
        public FloatToken(TokenType type, int line, decimal value) : base(type, line, value) { }
    }

    public class IdentifierToken : ValueToken<string>
    {
        public IdentifierToken(TokenType type, int line, string value) : base(type, line, value) { }
    }

    public class StringToken : ValueToken<string>
    {
        public StringToken(TokenType type, int line, string value) : base(type, line, value) { }
    }
}
