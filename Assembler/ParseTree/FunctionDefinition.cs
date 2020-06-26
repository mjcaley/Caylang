using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
    public class FunctionDefinition : Definition
    {
        public FunctionDefinition(string name, FunctionParameters parameters, IEnumerable<Statement> statements, int line)
            : base(line)
            => (Name, Parameters, Statements) = (name, parameters, statements);

        public string Name { get; }

        public FunctionParameters Parameters { get; }

        public IEnumerable<Statement> Statements { get; }

        public override string ToString()
        {
            return $"{GetType()} Name: {Name}, Locals: {Parameters.Locals}, Arguments: {Parameters.Arguments}";
        }
    }
}
