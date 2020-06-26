using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
    public enum FieldType
    {
        Integer8,
        Integer16,
        Integer32,
        Integer64,
        UnsignedInteger8,
        UnsignedInteger16,
        UnsignedInteger32,
        UnsignedInteger64,
        FloatingPoint32,
        FloatingPoint64,
        Address
    }
}
