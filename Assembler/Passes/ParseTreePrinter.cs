using Caylang.Assembler.ParseTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.Passes
{
	public class ParseTreePrinter : IVisitor
	{
		readonly string _tab = "    ";
		int _level = 0;

		private void Increment()
		{
			_level++;
		}

		private void Decrement()
		{
			_level--;
		}

		private string MakeTab()
		{
			var tab = new StringBuilder();
			for (var i = 0; i < _level; i++)
			{
				tab.Append(_tab);
			}

			return tab.ToString();
		}

		private void Print<T>(T type)
		{
			Console.WriteLine($"{MakeTab()}[{type?.ToString()}]");
		}

		private void Print<T>(string property, T value)
		{
			Console.WriteLine($"{MakeTab()}{property} : '{value}'");
		}
		public void Visit(IEnumerable<Definition> d)
		{
			foreach (var node in d)
			{
				Visit(node);
			}
		}
		private void Visit(Definition d)
		{
			switch (d)
			{
				case StructDefinition s:
					Visit(s);
					break;
				case FunctionDefinition f:
					Visit(f);
					break;
				case ConstantDefinition c:
					Visit(c);
					break;
			}
		}

		public void Visit(FunctionDefinition f)
		{
			Print(f);
			Increment();
			foreach (var statement in f.Statements)
            {
				Visit(statement);
            }
			Decrement();
		}

		public void Visit(ConstantDefinition c)
        {
			Print(c);
			Increment();
			Visit(c.Value);
			Decrement();
        }

		public void Visit(StructDefinition s)
        {
			Print(s);
			Increment();
			foreach (var field in s.Fields)
			{
				Print("Field", field);
			}
			Decrement();
        }
		private void Visit(Statement s)
		{
			switch (s)
			{
				case Label l:
					Visit(l);
					break;
				case NullaryInstruction n:
					Visit(n);
					break;
				case UnaryInstruction u:
					Visit(u);
					break;
			}
		}

		public void Visit(Label l)
		{
			Print(l);
		}

		public void Visit(NullaryInstruction n)
		{
			Print(n);
		}

		public void Visit(UnaryInstruction u)
		{
			Print(u);
			Increment();
			Visit(u.Operand);
			Decrement();
		}
		public void Visit(Literal l)
		{
			switch (l)
			{
				case Integer8Literal i:
					Visit(i);
					break;
				case Integer16Literal i:
					Visit(i);
					break;
				case Integer32Literal i:
					Visit(i);
					break;
				case Integer64Literal i:
					Visit(i);
					break;
				case UnsignedInteger8Literal u:
					Visit(u);
					break;
				case UnsignedInteger16Literal u:
					Visit(u);
					break;
				case UnsignedInteger32Literal u:
					Visit(u);
					break;
				case UnsignedInteger64Literal u:
					Visit(u);
					break;
				case Float32Literal f:
					Visit(f);
					break;
				case Float64Literal f:
					Visit(f);
					break;
				case StringLiteral s:
					Visit(s);
					break;
				case IdentifierLiteral i:
					Visit(i);
					break;
			}
		}

		public void Visit(Integer8Literal i)
		{
			Print(i);
		}

		public void Visit(Integer16Literal i)
		{
			Print(i);
		}

		public void Visit(Integer32Literal i)
		{
			Print(i);
		}

		public void Visit(Integer64Literal i)
		{
			Print(i);
		}

		public void Visit(UnsignedInteger8Literal i)
		{
			Print(i);
		}

		public void Visit(UnsignedInteger16Literal i)
		{
			Print(i);
		}

		public void Visit(UnsignedInteger32Literal i)
		{
			Print(i);
		}

		public void Visit(UnsignedInteger64Literal i)
		{
			Print(i);
		}

		public void Visit(Float32Literal f)
		{
			Print(f);
		}

		public void Visit(Float64Literal f)
		{
			Print(f);
		}

		public void Visit(StringLiteral s)
		{
			Print(s);
		}

		public void Visit(IdentifierLiteral i)
		{
			Print(i);
		}
	}
}
