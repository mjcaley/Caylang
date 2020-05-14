using System;
using System.Collections.Generic;
using System.Text;

namespace Caylang.Assembler.ParseTree
{
	public class Literal : ParseNode
	{
		public Literal(Token atom) => Atom = atom;
		
		public Token Atom { get; }
	}
	
	public class Integer8Literal : Literal
	{
		public Integer8Literal(Token atom) : base(atom) { }

		public byte? Cast
		{
			get
			{
				var converted = byte.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}
	
	public class Integer16Literal : Literal
	{
		public Integer16Literal(Token atom) : base(atom) { }

		public short? Cast
		{
			get
			{
				var converted = short.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}
	
	public class Integer32Literal : Literal
	{
		public Integer32Literal(Token atom) : base(atom) { }

		public int? CastValue
		{
			get
			{
				var converted = int.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}
	
	public class Integer64Literal : Literal
	{
		public Integer64Literal(Token atom) : base(atom) { }

		public long? Cast
		{
			get
			{
				var converted = long.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}
	
	public class UnsignedInteger8Literal : Literal
	{
		public UnsignedInteger8Literal(Token atom) : base(atom) { }

		public byte? Cast
		{
			get
			{
				var converted = byte.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}
	
	public class UnsignedInteger16Literal : Literal
	{
		public UnsignedInteger16Literal(Token atom) : base(atom) { }

		public short? Cast
		{
			get
			{
				var converted = short.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}
	
	public class UnsignedInteger32Literal : Literal
	{
		public UnsignedInteger32Literal(Token atom) : base(atom) { }

		public int? CastValue
		{
			get
			{
				var converted = int.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}
	
	public class UnsignedInteger64Literal : Literal
	{
		public UnsignedInteger64Literal(Token atom) : base(atom) { }

		public long? Cast
		{
			get
			{
				var converted = long.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}

	public class Float32Literal : Literal
	{
		public Float32Literal(Token atom) : base(atom) { }

		public float? CastValue
		{
			get
			{
				var converted = float.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}

	public class Float64Literal : Literal
	{
		public Float64Literal(Token atom) : base(atom) { }

		public decimal? CastValue
		{
			get
			{
				var converted = decimal.TryParse(Atom.Value, out var number);

				if (converted)
				{
					return number;
				}
				
				return null;
			}
		}
	}

	public class StringLiteral : Literal
	{
		public StringLiteral(Token atom) : base(atom) { }
		
		public string Value => Atom.Value;
	}

	public class IdentifierLiteral : Literal
	{
		public IdentifierLiteral(Token atom) : base(atom) { }
		
		public string Value => Atom.Value;
	}
}
