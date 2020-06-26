using System;

namespace Caylang.Assembler.ParseTree
{
    public class Literal : ParseNode
    {
        public Literal(int line) : base(line) { }
    }

    public abstract class TypedLiteral<T> : Literal
    {
        protected internal TypedLiteral(T value, int line) : base(line) => Value = value;

        public T Value { get; }

        public override string ToString()
        {
            return $"{GetType()} Value: {Value}";
        }
    }

    public class Integer8Literal : TypedLiteral<sbyte>
    {
        public Integer8Literal(sbyte value, int line) : base(value, line) { }
    }

    public class Integer16Literal : TypedLiteral<short>
    {
        public Integer16Literal(short value, int line) : base(value, line) { }
    }

    public class Integer32Literal : TypedLiteral<int>
    {
        public Integer32Literal(int value, int line) : base(value, line) { }
    }

    public class Integer64Literal : TypedLiteral<long>
    {
        public Integer64Literal(long value, int line) : base(value, line) { }
    }

    public class UnsignedInteger8Literal : TypedLiteral<byte>
    {
        public UnsignedInteger8Literal(byte value, int line) : base(value, line) { }
    }

    public class UnsignedInteger16Literal : TypedLiteral<ushort>
    {
        public UnsignedInteger16Literal(ushort value, int line) : base(value, line) { }
    }

    public class UnsignedInteger32Literal : TypedLiteral<uint>
    {
        public UnsignedInteger32Literal(uint value, int line) : base(value, line) { }
    }

    public class UnsignedInteger64Literal : TypedLiteral<ulong>
    {
        public UnsignedInteger64Literal(ulong value, int line) : base(value, line) { }
    }

    public class Float32Literal : TypedLiteral<float>
    {
        public Float32Literal(float value, int line) : base(value, line) { }
    }

    public class Float64Literal : TypedLiteral<double>
    {
        public Float64Literal(double value, int line) : base(value, line) { }
    }

    public class StringLiteral : TypedLiteral<string>
    {
        public StringLiteral(string value, int line) : base(value, line) { }
    }

    public class IdentifierLiteral : TypedLiteral<string>
    {
        public IdentifierLiteral(string name, int line) : base(name, line) { }
    }
}
