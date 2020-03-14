﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using CayLang.Assembler;

namespace Caylang.Assembler
{
    public class LexerException : Exception { }

    public class LexerDataException : LexerException { }

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
            Negative,
            Digit,
            Decimal,
            Binary,
            Hexadecimal,
            Float,
            String,
            Keyword,
            Identifier,
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

        private void Append()
        {
            Lexeme += Current;
            Advance();
        }

        public Token SkipWhitespace()
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

            return null;
        }

        public Token Start()
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
                    Mode = LexerMode.Negative;
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
                    Mode = LexerMode.Decimal;
                    break;
                case '.':
                    Append();
                    Mode = LexerMode.Keyword;
                    break;
                case '"':
                    Advance();
                    Mode = LexerMode.String;
                    break;
                case '_':
                case char _ when char.IsLetter(Current):
                    Append();
                    Mode = LexerMode.Identifier;
                    break;
                default:
                    Advance();
                    break;
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
