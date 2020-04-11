using System;
using Caylang.Assembler;

namespace Caylang.Assembler
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
        NewArray,

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

        EndOfFile
    }

    public class Token
    {
        public Token(TokenType type, int line) => (Type, Line, Value) = (type, line, string.Empty);
        public Token(TokenType type, int line, string value) => (Type, Line, Value) = (type, line, value);

        public TokenType Type { get; }
        public int Line { get; }
        public string Value { get; }

        public override string ToString()
        {
            return $"Caylang.Assembler.Token(Type={Type.ToString()}, Line={Line.ToString()}, Value={Value})";
        }
    }
}
