using System;

namespace Caylang.Assembler
{
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class UnexpectedTokenException : ParserException
    {
        public UnexpectedTokenException(Token? found) : base(_message) => FoundToken = found;

        public UnexpectedTokenException(Token? found, Exception innerException) : base(_message, innerException) => FoundToken = found;

        private const string _message = "Unexpected token";

        public Token? FoundToken { get; }
    }

    public class IntegerConversionException : ParserException
    {
        public IntegerConversionException() => At = null;
        
        public IntegerConversionException(Token? token) => At = token;

        public Token? At { get; }
    }
}
