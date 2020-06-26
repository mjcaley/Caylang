using Caylang.Assembler.Passes;
using Pidgin;
using System;

namespace Caylang.Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokens = Lexer.LexString(@"
struct Byte
    u8

func Increment args=0 locals=1
    ldlocal 0
    ldconst 1 i8
    add
    ret
");
            var tree = Parser.Start.ParseOrThrow(tokens);

            ParseTree.IVisitor printer = new ParseTreePrinter();
            printer.Visit(tree);
        }
    }
}
