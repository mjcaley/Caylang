namespace Caylang.Assembler.AST
{
    public class Literal : ASTLeaf
    {
        public Literal(int line) : base(line) {}
    }
    
    public abstract class ValueLiteral<T> : Literal
    {
        public ValueLiteral(T value, int line) : base(line) => Value = value;
    
        public T Value { get; set; }
    }

    public class Integer8 : ValueLiteral<sbyte>
    {
        public Integer8(sbyte value, int line) : base(value, line) {}
    }

    public class Integer16 : ValueLiteral<short>
    {
        public Integer16(short value, int line) : base(value, line) {}
    }

    public class Integer32 : ValueLiteral<int>
    {
        public Integer32(int value, int line) : base(value, line) {}
    }

    public class Integer64 : ValueLiteral<long>
    {
        public Integer64(long value, int line) : base(value, line) {}
    }

    public class UnsignedInteger8 : ValueLiteral<byte>
    {
        UnsignedInteger8(byte value, int line) : base(value, line) {}
    }

    public class UnsignedInteger16 : ValueLiteral<ushort>
    {
        public UnsignedInteger16(ushort value, int line) : base(value, line) {}
    }

    public class UnsignedInteger32 : ValueLiteral<uint>
    {
        public UnsignedInteger32(uint value, int line) : base(value, line) {}
    }

    public class UnsignedInteger64 : ValueLiteral<ulong>
    {
        public UnsignedInteger64(ulong value, int line) : base(value, line) {}
    }

    public class FloatingPoint32 : ValueLiteral<float>
    {
        public FloatingPoint32(float value, int line) : base(value, line) {}
    }

    public class FloatingPoint64 : ValueLiteral<double>
    {
        public FloatingPoint64(double value, int line) : base(value, line) {}
    }

    public class StringLiteral : ValueLiteral<string>
    {
        public StringLiteral(string value, int line) : base(value, line) {}
    }

    public class Identifier : ValueLiteral<string>
    {
        public Identifier(string value, int line) : base(value, line) {}
    }
}
