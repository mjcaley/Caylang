using System.Collections.Generic;
using System.Linq;

namespace Caylang.Assembler.ParseTree
{
    public class UnexpectedTokenError : ParseNode
    {
        public UnexpectedTokenError(List<Token?> terminals) => Terminals = terminals;
        
        public UnexpectedTokenError(IEnumerable<Token?> terminals) => Terminals = terminals.ToList();

        public UnexpectedTokenError(params Token?[] terminals) : this(terminals as IEnumerable<Token?>) { }


        public List<Token?> Terminals { get; }
    }
}
