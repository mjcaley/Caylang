using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using CayLang.Assembler;

namespace Caylang.Assembler
{
    public class LexerException : Exception
    {
        public LexerException() { }

        public LexerException(string message) : base(message) { }

        public LexerException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class LexerDataException : LexerException
    {
        public LexerDataException() { }

        public LexerDataException(string message) : base(message) { }

        public LexerDataException(string message, Exception inner): base(message, inner) { }
    }

    public class Lexer : IDisposable
    {
        public Lexer(TextReader stream)
        {
            _stream = stream;
            Advance();
        }

        public enum LexerMode
        {
            Start,
            SkipWhitespace,
            IsNegative,
            Digit,
            IsDecimal,
            IsBinary,
            IsHexadecimal,
            IsFloat,
            IsString,
            IsKeyword,
            IsIdentifier,
            End
        }

        public LexerMode Mode { get; set; } = LexerMode.Start;

        public string Lexeme { get; set; } = "";
        
        private int _line = 1;
        public int Line => _line;

        private char _current;
        public char Current => _current;

        private readonly TextReader _stream;

        private static char ToChar(int character)
        {
            if (character == -1)
            {
                return '\0';
            }
            else
            {
                return Convert.ToChar(character);
            }
        }

        public char Advance()
        {
            var previous = _current;

            var next = ToChar(_stream.Read());

            if (next == '\r')
            {
                if (ToChar(_stream.Peek()) == '\n')
                {
                    next = ToChar(_stream.Read());
                }
                else
                {
                    next = '\n';
                }
            }

            _current = next;

            if (previous == '\n')
            {
                _line++;
            }

            return previous;
        }

        public void Append()
        {
            Lexeme += Current;
            Advance();
        }

        public string Consume()
        {
            var lexeme = Lexeme;
            Lexeme = "";

            return lexeme;
        }

        public static List<Token> LexString(string data)
        {
            var stream = new StringReader(data);
            return Lex(stream);
        }

        public static List<Token> LexFile(string filename)
        {
            var stream = File.OpenText(filename);
            return Lex(stream);
        }

        private Token NewToken(TokenType type, string value = "")
        {
            return new Token(type, value, Line);
        }

        public void SkipWhitespace()
        {
            switch (Current)
            {
                case char _ when char.IsWhiteSpace(Current):
                    Advance();
                    break;
                default:
                    Mode = LexerMode.Start;
                    break;
            }
        }

        public Token? Start()
        {
            switch (Current)
            {
                case char _ when char.IsWhiteSpace(Current):
                    Mode = LexerMode.SkipWhitespace;
                    break;
                case '=':
                    return NewToken(TokenType.Equal);
                case ':':
                    return NewToken(TokenType.Colon);
                case '-':
                    Append();
                    Mode = LexerMode.IsNegative;
                    break;
                case '0':
                    Append();
                    Mode = LexerMode.Digit;
                    break;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    Append();
                    Mode = LexerMode.IsDecimal;
                    break;
                case '.':
                    Append();
                    Mode = LexerMode.IsKeyword;
                    break;
                case '"':
                    Advance();
                    Mode = LexerMode.IsString;
                    break;
                case '_':
                case char _ when char.IsLetter(Current):
                    Append();
                    Mode = LexerMode.IsIdentifier;
                    break;
                default:
                    Append();
                    return NewToken(TokenType.Error, Consume());
            }

            return null;
        }

        public Token? Negative()
        {
            switch (Current)
            {
                case '0':
                    Append();
                    Mode = LexerMode.Digit;
                    break;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    Append();
                    Mode = LexerMode.IsDecimal;
                    break;
                default:
                    Append();
                    return NewToken(TokenType.Error, Consume());
            }

            return null;
        }

        public Token? Digit()
        {
            switch (Current)
            {
                case 'x':
                    Append();
                    Mode = LexerMode.IsHexadecimal;
                    break;
                case 'b':
                    Append();
                    Mode = LexerMode.IsBinary;
                    break;
                case '.':
                    Append();
                    Mode = LexerMode.IsFloat;
                    break;
                default:
                    Append();
                    Mode = LexerMode.Start;
                    return NewToken(TokenType.IntegerLiteral, Consume());
            }

            return null;
        }

        public Token? IsDecimal()
        {
            switch (Current)
            {
                case var _ when char.IsDigit(Current):
                    Append();
                    break;
                case '.':
                    Append();
                    Mode = LexerMode.IsFloat;
                    break;
                default:
                    Mode = LexerMode.Start;
                    return NewToken(TokenType.IntegerLiteral, Consume());
            }

            return null;
        }

        public Token? IsHexadecimal()
        {
            char[] HexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

            switch (Current)
            {
                case var _ when HexChars.Contains(Current):
                    Append();
                    break;
                default:
                    Mode = LexerMode.Start;
                    return NewToken(TokenType.IntegerLiteral, Consume());
            }

            return null;
        }

        public static List<Token> Lex(TextReader stream)
        {
            using var state = new Lexer(stream);
            var tokens = new List<Token>();

            while (state.Current != '\0')
            {
                switch (state.Mode)
                {
                    case LexerMode.Start:
                        var token = state.Start();
                        if (token != null)
                        {
                            tokens.Add(token);
                        }

                        break;
                    case LexerMode.SkipWhitespace:
                        state.SkipWhitespace();
                        break;
                    case LexerMode.End:
                        break;
                }
            }

            tokens.Add(new Token(TokenType.EndOfFile, state.Line));

            return tokens;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }

                disposedValue = true;
            }
        }

        ~Lexer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
