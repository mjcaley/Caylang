using Caylang.Assembler.ParseTree;
using CayLang.Assembler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Caylang.Assembler
{
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

	public class Parser
	{
        private readonly IEnumerator<Token> tokens;

        private Token? _current;
        public Token? Current => _current;

        public Token? Next => tokens.Current;

        public Parser(IEnumerator<Token> tokens)
        {
            this.tokens = tokens;
            Initialize();
        }

        public Parser(IEnumerable<Token> tokens)
		{
            this.tokens = tokens.GetEnumerator();
            Initialize();
        }

        private void Initialize()
        {
            tokens.MoveNext();
            Advance();
        }

        private void Advance()
        {
            _current = Next;
            tokens.MoveNext();
        }

        public Tree Start()
        {
            return null;
        }
    }
}
