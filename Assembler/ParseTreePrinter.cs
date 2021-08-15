using System;
using System.Text;
using Caylang.Assembler.ParseTree;

namespace Caylang.Assembler
{
	class ParseTreePrinter : DefaultVisitor, IVisitor
	{
		private int indentLevel = 0;
		private const int indentLength = 4;

		private void Indent() => indentLevel++;

		private void Dedent() => indentLevel--;

		private void PrintLine(string value)
		{
			var indentPrefix = new StringBuilder();
			for (var i = 0; i < indentLength; i++)
			{
				for (var j = 0; j < indentLevel; j++)
				{
					indentPrefix.Append(' ');
				}
			}

			Console.WriteLine($"{indentPrefix}{value}");
		}

        public override void Visit(Branch node)
		{
			PrintLine($"{node.GetType()}");
			Indent();
			base.Visit(node);
			Dedent();
		}

		public override void Visit(Leaf node)
		{
			PrintLine($"{node.GetType()} - {node.Token.Kind} : '{node.Token.Text}'");
			base.Visit(node);
		}
	}
}
