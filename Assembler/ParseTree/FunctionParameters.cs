using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
    public class FunctionParameters
    {
        public FunctionParameters(int locals, int arguments) => (Locals, Arguments) = (locals, arguments);

        public int Locals { get; }

        public int Arguments { get; }
    }
}
