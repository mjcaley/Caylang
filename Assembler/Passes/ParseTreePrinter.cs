using Caylang.Assembler.ParseTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.Passes
{
	public class ParseTreePrinter : ParseTreeVisitor, IVisitor
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

		private void Print(Type type)
		{
			Console.WriteLine($"{MakeTab()}[{type.Name}]");
		}

		private void Print(string property, Token token)
		{
			Console.WriteLine($"{MakeTab()}{property} : [{token.Type}] : Line {token.Line} : '{token.Value}'");
		}

		public void Visit(IEnumerable<ParseNode> nodes)
		{
			if (nodes == null)
			{
				return;
			}

			foreach (var node in nodes)
			{
				Visit(node);
			}
		}

		public override void Visit(Tree t)
		{
			Print(t.GetType());
			Increment();
			foreach (var definition in t.Definitions)
			{
				Visit(definition);
			}
			foreach (var function in t.Functions)
			{
				Visit(function);
			}
			Decrement();
		}

		public override void Visit(FunctionNode f)
		{
			Print(f.GetType());
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

		public override void Visit(Definition d)
		{
			Print(d.GetType());
			Increment();
			Visit(d.Name, "Name");
			Visit(d.Value);
			Decrement();
		}

		public override void Visit(LabelStatement l)
		{
			Print(l.GetType());
			Increment();
			Visit(l.Label, "Label");
			Decrement();
		}

		public override void Visit(NullaryInstruction n)
		{
			Print(n.GetType());
			Increment();
			Visit(n.Instruction, "Instruction");
			Visit(n.ReturnType, "ReturnType");
			Decrement();
		}

		public override void Visit(UnaryInstruction u)
		{
			Print(u.GetType());
			Increment();
			Visit(u.Instruction, "Instruction");
			Visit(u.ReturnType, "ReturnType");
			Visit(u.First);
			Decrement();
		}

		public override void Visit(Integer8Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(Integer16Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(Integer32Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(Integer64Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(UnsignedInteger8Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(UnsignedInteger16Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(UnsignedInteger32Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(UnsignedInteger64Literal i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(Float32Literal f)
		{
			Print(f.GetType());
			Increment();
			Visit(f.Atom, "Atom");
			Decrement();
		}

		public override void Visit(Float64Literal f)
		{
			Print(f.GetType());
			Increment();
			Visit(f.Atom, "Atom");
			Decrement();
		}

		public override void Visit(StringLiteral s)
		{
			Print(s.GetType());
			Increment();
			Visit(s.Atom, "Atom");
			Decrement();
		}

		public override void Visit(IdentifierLiteral i)
		{
			Print(i.GetType());
			Increment();
			Visit(i.Atom, "Atom");
			Decrement();
		}

		public override void Visit(UnexpectedTokenError u)
		{
			Print(u.GetType());
			Increment();
			foreach (var token in u.Children)
			{
				Visit(token);
			}
			Decrement();
		}

		public void Visit(Token token, string property = "")
		{
			Print(property, token);
		}
	}
}
