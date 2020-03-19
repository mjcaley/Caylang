using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            IsWord,
            End
        }

        private static readonly ReadOnlyCollection<char> HexChars = new List<char>
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f',
            'A', 'B', 'C', 'D', 'E', 'F'
        }.AsReadOnly();

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

        private Token CreateToken(TokenType type, string value = "")
        {
            return new Token(type, value, Line);
        }

        private Token TransitionAndCreateToken(LexerMode mode, TokenType type, string value = "")
        {
            Mode = mode;
            return CreateToken(type, value);
        }

        private Token? Transition(LexerMode mode)
        {
            Mode = mode;

            return null;
        }

        public void SkipWhitespace()
        {
            switch (Current)
            {
                case var _ when char.IsWhiteSpace(Current):
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
                case var _ when char.IsWhiteSpace(Current):
                    Mode = LexerMode.SkipWhitespace;
                    break;
                case '=':
                    return CreateToken(TokenType.Equal);
                case ':':
                    return CreateToken(TokenType.Colon);
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
                    Mode = LexerMode.IsWord;
                    break;
                case '"':
                    Advance();
                    Mode = LexerMode.IsString;
                    break;
                case '_':
                case var _ when char.IsLetter(Current):
                    Append();
                    Mode = LexerMode.IsWord;
                    break;
                default:
                    Append();
                    return CreateToken(TokenType.Error, Consume());
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
                    return CreateToken(TokenType.Error, Consume());
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
                    return CreateToken(TokenType.IntegerLiteral, Consume());
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
                    return CreateToken(TokenType.IntegerLiteral, Consume());
            }

            return null;
        }

        public Token? IsHexadecimal()
        {
            switch (Current)
            {
                case var _ when HexChars.Contains(Current):
                    Append();
                    break;
                default:
                    Mode = LexerMode.Start;
                    return CreateToken(TokenType.IntegerLiteral, Consume());
            }

            return null;
        }

        public Token? IsBinary()
        {
            switch (Current)
            {
                case '0':
                case '1':
                    Append();
                    break;
                default:
                    Mode = LexerMode.Start;
                    return CreateToken(TokenType.IntegerLiteral, Consume());
            }

            return null;
        }

        public Token? IsFloat()
        {
            switch (Current)
            {
                case var _ when char.IsDigit(Current):
                    Append();
                    break;
                default:
                    Mode = LexerMode.Start;
                    return CreateToken(TokenType.FloatLiteral, Consume());
            }

            return null;
        }

        public Token? IsString()
        {
            switch (Current)
            {
                case '\\':
                    Advance();
                    switch (Current)
                    {
                        case 'a':
                            Advance();
                            Lexeme += '\a';
                            break;
                        case 'b':
                            Advance();
                            Lexeme += '\b';
                            break;
                        case 'f':
                            Advance();
                            Lexeme += '\f';
                            break;
                        case 'n':
                            Advance();
                            Lexeme += '\n';
                            break;
                        case 'r':
                            Advance();
                            Lexeme += '\r';
                            break;
                        case 't':
                            Advance();
                            Lexeme += '\t';
                            break;
                        case 'v':
                            Advance();
                            Lexeme += '\v';
                            break;
                        case '"':
                            Advance();
                            Lexeme += '"';
                            break;
                        default:
                            Mode = LexerMode.Start;
                            return CreateToken(TokenType.Error, Consume());
                    }

                    break;
                case '"':
                    Advance();
                    Mode = LexerMode.Start;
                    return CreateToken(TokenType.StringLiteral, Consume());
                case '\n':
                    Mode = LexerMode.Start;
                    return CreateToken(TokenType.Error, Consume());
                default:
                    Append();
                    break;
            }

            return null;
        }

        public Token? IsWord()
        {
            switch (Current)
            {
                case var _ when char.IsLetterOrDigit(Current):
                    Append();
                    break;
                default:
                    var value = Consume();
                    return value switch
                    {
                        "func" => TransitionAndCreateToken(LexerMode.Start, TokenType.Func),
                        "define" => TransitionAndCreateToken(LexerMode.Start, TokenType.Define),
                        "args" => TransitionAndCreateToken(LexerMode.Start, TokenType.Param, value),
                        "locals" => TransitionAndCreateToken(LexerMode.Start, TokenType.Param, value),
                        "halt" => TransitionAndCreateToken(LexerMode.Start, TokenType.Halt),
                        "nop" => TransitionAndCreateToken(LexerMode.Start, TokenType.Noop),
                        "add" => TransitionAndCreateToken(LexerMode.Start, TokenType.Add),
                        "sub" => TransitionAndCreateToken(LexerMode.Start, TokenType.Subtract),
                        "mul" => TransitionAndCreateToken(LexerMode.Start, TokenType.Multiply),
                        "div" => TransitionAndCreateToken(LexerMode.Start, TokenType.Divide),
                        "mod" => TransitionAndCreateToken(LexerMode.Start, TokenType.Modulo),
                        "ldconst" => TransitionAndCreateToken(LexerMode.Start, TokenType.LoadConst),
                        "ldlocal" => TransitionAndCreateToken(LexerMode.Start, TokenType.LoadLocal),
                        "stconst" => TransitionAndCreateToken(LexerMode.Start, TokenType.StoreLocal),
                        "pop" => TransitionAndCreateToken(LexerMode.Start, TokenType.Pop),
                        "testeq" => TransitionAndCreateToken(LexerMode.Start, TokenType.TestEqual),
                        "testne" => TransitionAndCreateToken(LexerMode.Start, TokenType.TestNotEqual),
                        "testgt" => TransitionAndCreateToken(LexerMode.Start, TokenType.TestGreaterThan),
                        "testlt" => TransitionAndCreateToken(LexerMode.Start, TokenType.TestLessThan),
                        "jmp" => TransitionAndCreateToken(LexerMode.Start, TokenType.Jump),
                        "jmpt" => TransitionAndCreateToken(LexerMode.Start, TokenType.JumpTrue),
                        "jmpf" => TransitionAndCreateToken(LexerMode.Start, TokenType.JumpFalse),
                        "callfunc" => TransitionAndCreateToken(LexerMode.Start, TokenType.CallFunc),
                        "callinterface" => TransitionAndCreateToken(LexerMode.Start, TokenType.CallInterface),
                        "ret" => TransitionAndCreateToken(LexerMode.Start, TokenType.Return),
                        "newstruct" => TransitionAndCreateToken(LexerMode.Start, TokenType.NewStruct),
                        "ldfield" => TransitionAndCreateToken(LexerMode.Start, TokenType.LoadField),
                        "stfield" => TransitionAndCreateToken(LexerMode.Start, TokenType.StoreField),
                        "addr" => TransitionAndCreateToken(LexerMode.Start, TokenType.AddressType),
                        "i8" => TransitionAndCreateToken(LexerMode.Start, TokenType.i8Type),
                        "u8" => TransitionAndCreateToken(LexerMode.Start, TokenType.u8Type),
                        "i16" => TransitionAndCreateToken(LexerMode.Start, TokenType.i16Type),
                        "u16" => TransitionAndCreateToken(LexerMode.Start, TokenType.u16Type),
                        "i32" => TransitionAndCreateToken(LexerMode.Start, TokenType.i32Type),
                        "u32" => TransitionAndCreateToken(LexerMode.Start, TokenType.u32Type),
                        "i64" => TransitionAndCreateToken(LexerMode.Start, TokenType.i64Type),
                        "u64" => TransitionAndCreateToken(LexerMode.Start, TokenType.u64Type),
                        "f32" => TransitionAndCreateToken(LexerMode.Start, TokenType.f32Type),
                        "f64" => TransitionAndCreateToken(LexerMode.Start, TokenType.f64Type),
                        "str" => TransitionAndCreateToken(LexerMode.Start, TokenType.StringType),
                        _ => TransitionAndCreateToken(LexerMode.Start, TokenType.Identifier, value)
                    };
            }

            return null;
        }

        public Token? IsIdentifier()
        {
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
