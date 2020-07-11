namespace Caylang.Assembler.AST
{
    public enum Type
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
    
    public class StructFieldDefinition : ASTLeaf
    {
        public StructFieldDefinition(int line) : base(line) {}
        
        public Type Field { get; set; }
    }
}
