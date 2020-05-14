using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
    public class Statement : ParseNode
    {
        public Statement() : base() { }

        public Statement(IEnumerable<ParseNode> children) : base(children) { }

        public Statement(ParseNode child) : base(new[] { child }) { }
    }
}
