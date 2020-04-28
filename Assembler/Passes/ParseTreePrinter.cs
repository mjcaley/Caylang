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

		private void Print(int line, Type type, string value)
		{
			Console.WriteLine($"{MakeTab()}[{type.Name} : {line}] {value}");
		}
		private void Print(Type type, string value)
		{
			Console.WriteLine($"{MakeTab()}[{type.Name}] {value}");
		}

		public void Visit(Tree t)
		{
			Print(t.GetType(), $"");
			Increment();
			foreach (var definition in t.Definitions)
			{
				Visit(definition.Value);
			}
			foreach (var function in t.Functions)
			{
				Visit(function);
			}
			Decrement();
		}

		public void Visit(Function f)
		{
			Print(f.Line, f.GetType(), $"Name: {f.Name}, Locals: {f.Locals}, Arguments: {f.Arguments}");
			Increment();
			foreach (var statement in f.Statements)
			{
				switch (statement)
				{
					case LabelStatement l:
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
			Decrement();
		}

		public void Visit(Definition d)
		{
			Print(d.Line, d.GetType(), $"Name: {d.Name}");
			Increment();
			Visit(d.Value);
			Decrement();
		}

		public void Visit(LabelStatement l)
		{
			Print(l.Line, l.GetType(), $"Label: {l.Label}");
		}

		public void Visit(NullaryInstruction n)
		{
			Print(n.Line, n.GetType(), $"Instruction: {n.Instruction}, ReturnType: {n.ReturnType}");
		}

		public void Visit(UnaryInstruction u)
		{
			Print(u.Line, u.GetType(), $"Instruction: {u.Instruction}, ReturnType: {u.ReturnType}");
			Increment();
			Visit(u.First);
			Decrement();
		}

		public void Visit(Operand o)
		{
			Print(o.Line, o.GetType(), $"Type: {o.Type}, Value: {o.Value}");
		}

		public void Visit(IntegerLiteral i)
		{
			Print(i.GetType(), $"Value: {i.Value}");
		}

		public void Visit(FloatLiteral f)
		{
			Print(f.GetType(), $"Value: {f.Value}");
		}

		public void Visit(StringLiteral s)
		{
			Print(s.GetType(), $"Value: {s.Value}");
		}

		public void Visit(IdentifierLiteral i)
		{
			Print(i.GetType(), $"Value: {i.Value}");
		}
	}
}
