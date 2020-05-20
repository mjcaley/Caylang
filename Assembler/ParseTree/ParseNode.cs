using System;
using System.Collections.Generic;
using System.Linq;

namespace Caylang.Assembler.ParseTree
{
    public abstract class ParseNode
    {
        protected ParseNode() => _children = new List<ParseNode>();

        protected ParseNode(IEnumerable<ParseNode> children) => _children = children.ToList();

        protected ParseNode(params ParseNode[] children) => _children = children.ToList();

        private readonly List<ParseNode> _children;
        public List<ParseNode> Children => _children;
    }
}
