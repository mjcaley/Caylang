using System;
using System.Text;

namespace Caylang.Assembler
{
	class ParseTreePrinter : Pass
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

		public override void Visit(ParseTreeBranch node)
		{
			PrintLine($"{node.Kind}");
			Indent();
			foreach (var child in node.Children)
			{
				Visit(child);
			}
			Dedent();
		}

		public override void Visit(ParseTreeLeaf node)
		{
			PrintLine($"{node.Kind} - {node.Token.Kind} : '{node.Token.Text}'");
			base.Visit(node);
		}
	}
}
