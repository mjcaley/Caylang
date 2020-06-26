namespace Caylang.Assembler.ParseTree
{
    public class ParseNode
    {
        public ParseNode(int line) => Line = line;
        
        public int Line { get; }
    }
}