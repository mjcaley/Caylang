using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
    public class StructDefinition : Definition
    {
        public StructDefinition(string name, IEnumerable<FieldType> fields, int line)
            : base(line)
            => (Name, Fields) = (name, fields);

        public string Name { get; }

        public IEnumerable<FieldType> Fields { get; }
    }
}
