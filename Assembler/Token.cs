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

        // Literals
        Integer,
        Float,
        String,
        Identifier,

        // Assembler tokens
        Func,
        Param,
        Define,
        Equal,
        Colon,

        EndOfFile
    }

    public class Token
    {
        public Token(TokenType type, string value, int line)
        {
            Type = type;
            Line = line;
            Value = value;
        }

        public Token(TokenType type, int line) : this(type, "", line) { }

        public TokenType Type { get; }

        public int Line { get; }

        public string Value { get; }
    }
}
