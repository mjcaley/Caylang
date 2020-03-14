using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler
{
    public struct Position
    {
        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }

        public int Column { get; }
    }
}
