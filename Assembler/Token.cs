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
        CallVirtual,

        Return,

        NewStruct,
        LoadField,
        StoreField,
        NewArray,
        LoadElement,
        StoreElement,

        // Types
        VoidType,
        Integer8Type,
        UInteger8Type,
        Integer16Type,
        UInteger16Type,
        Integer32Type,
        UInteger32Type,
        Integer64Type,
        UInteger64Type,
        AddressType,
        Float32Type,
        Float64Type,
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
        Struct,
        Equal,
        Colon,
        Comma,
        Negative,

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
