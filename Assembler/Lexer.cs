using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CayLang.Assembler;
using Microsoft.VisualBasic.CompilerServices;

namespace Caylang.Assembler
{
    public static class SafeAddTo
    {
        public static void AddTo(this Token token, List<Token> list)
        {
            list?.Add(token);
        }
    }

    public class Lexer
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
            End,
            None
        }

        public LexerMode Mode { get; set; } = LexerMode.Start;

        public StringBuilder Lexeme { get; set; } = new StringBuilder();
        
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

        public Lexer Append()
        {
            Lexeme.Append(Current);
            Advance();

            return this;
        }

        public Lexer Append(char character)
        {
            Lexeme.Append(character);
            Advance();

            return this;
        }

        public string Consume()
        {
            var lexeme = Lexeme.ToString();
            Lexeme.Clear();

            return lexeme;
        }

        public Lexer Skip()
        {
            Advance();

            return this;
        }

        public Lexer Discard()
        {
            Lexeme.Clear();

            return this;
        }

        public Token? Emit()
        {
            return null;
        }

        public Token? Emit(TokenType type)
        {
            return new Token(type, Consume(), Line);
        }

        public Lexer Transition(LexerMode mode)
        {
            Mode = mode;

            return this;
        }

        public Lexer ReplaceLexeme(Func<string, string> with)
        {
            var replacement = with(Lexeme.ToString());
            Lexeme.Clear();
            Lexeme.Append(replacement);

            return this;
        }


        public Token? SkipWhitespace() =>
            Current switch
            {
                var _ when char.IsWhiteSpace(Current) => Skip().Emit(),
                _ => Transition(LexerMode.Start).Emit()
            };

        public Token? Start() =>
            Current switch
            {
                var _ when char.IsWhiteSpace(Current) => Transition(LexerMode.SkipWhitespace).Emit(),
                '=' => Discard().Emit(TokenType.Equal),
                ':' => Discard().Emit(TokenType.Colon),
                '-' => Append().Transition(LexerMode.IsNegative).Emit(),
                '0' => Append().Transition(LexerMode.Digit).Emit(),
                var x when 
                    x >= '1' && 
                    x <= '9' => Append().Transition(LexerMode.IsDecimal).Emit(),
                '"' => Skip().Transition(LexerMode.IsString).Emit(),
                var x when x == '_' || char.IsLetter(x) => Append().Transition(LexerMode.IsWord).Emit(),
                _ => Append().Emit(TokenType.Error)
            };

        public Token? Negative() =>
            Current switch
            {
                '0' => Append().Transition(LexerMode.Digit).Emit(),
                var x when 
                    x >= '1' && 
                    x <= '9' => Append().Transition(LexerMode.IsDecimal).Emit(),
                _ => Append().Emit(TokenType.Error)
            };

        public Token? Digit() =>
            Current switch
            {
                'x' => Skip().Discard().Transition(LexerMode.IsHexadecimal).Emit(),
                'b' => Skip().Discard().Transition(LexerMode.IsBinary).Emit(),
                '.' => Append().Transition(LexerMode.IsFloat).Emit(),
                _ => Append().Transition(LexerMode.Start).Emit(TokenType.IntegerLiteral)
            };

        public Token? IsDecimal() =>
            Current switch
            {
                var _ when char.IsDigit(Current) => Append().Emit(),
                '.' => Append().Transition(LexerMode.IsFloat).Emit(),
                _ => Transition(LexerMode.Start).Emit(TokenType.IntegerLiteral)
            };

        public Token? IsHexadecimal() =>
            Current switch
            {
                var x when
                    (x >= '0' && x <= '9') ||
                    (x >= 'a' && x <= 'f') ||
                    (x >= 'A' && x <= 'F') => Append().Emit(),
                _ => ReplaceLexeme((lexeme) =>
                {
                    var integer = Convert.ToInt64(lexeme, 16);
                    return integer.ToString(CultureInfo.InvariantCulture);
                }).Transition(LexerMode.Start).Emit(TokenType.IntegerLiteral)
            };

        public Token? IsBinary()
        {
            return Current switch
            {
                var x when x == '0' || x == '1' => Append().Emit(),
                _ => ReplaceLexeme((lexeme) =>
                {
                    var integer = Convert.ToInt64(lexeme, 2);
                    return integer.ToString(CultureInfo.InvariantCulture);
                }).Transition(LexerMode.Start).Emit(TokenType.IntegerLiteral)
            };
        }

        public Token? IsFloat() =>
            Current switch
            {
                var _ when char.IsDigit(Current) => Append().Emit(),
                _ => Transition(LexerMode.Start).Emit(TokenType.FloatLiteral)
            };

        public Token? IsString() =>
            Current switch
            {
                var x when
                    x == '\\' &&
                    Skip().Current != '\0' => Current switch
                    {
                        'a' => Append('\a').Emit(),
                        'b' => Append('\b').Emit(),
                        'f' => Append('\f').Emit(),
                        'n' => Append('\n').Emit(),
                        'r' => Append('\r').Emit(),
                        't' => Append('\t').Emit(),
                        'v' => Append('\v').Emit(),
                        '"' => Append('"').Emit(),
                        '0' => Append('\0').Emit(),
                        _ => Transition(LexerMode.Start).Emit(TokenType.Error),
                    },
                '"' => Skip().Transition(LexerMode.Start).Emit(TokenType.StringLiteral),
                '\n' => Transition(LexerMode.Start).Emit(TokenType.Error),
                _ => Append().Emit()
            };

        public Token? IsWord() =>
            Current switch
            {
                var x when char.IsLetterOrDigit(x) => Append().Emit(),
                _ => Lexeme.ToString() switch
                {
                    "func" => Transition(LexerMode.Start).Discard().Emit(TokenType.Func),
                    "define" => Transition(LexerMode.Start).Discard().Emit(TokenType.Define),
                    "args" => Transition(LexerMode.Start).Discard().Emit(TokenType.Args),
                    "locals" => Transition(LexerMode.Start).Discard().Emit(TokenType.Locals),
                    "halt" => Transition(LexerMode.Start).Discard().Emit(TokenType.Halt),
                    "nop" => Transition(LexerMode.Start).Discard().Emit(TokenType.Noop),
                    "add" => Transition(LexerMode.Start).Discard().Emit(TokenType.Add),
                    "sub" => Transition(LexerMode.Start).Discard().Emit(TokenType.Subtract),
                    "mul" => Transition(LexerMode.Start).Discard().Emit(TokenType.Multiply),
                    "div" => Transition(LexerMode.Start).Discard().Emit(TokenType.Divide),
                    "mod" => Transition(LexerMode.Start).Discard().Emit(TokenType.Modulo),
                    "ldconst" => Transition(LexerMode.Start).Discard().Emit(TokenType.LoadConst),
                    "ldlocal" => Transition(LexerMode.Start).Discard().Emit(TokenType.LoadLocal),
                    "stlocal" => Transition(LexerMode.Start).Discard().Emit(TokenType.StoreLocal),
                    "pop" => Transition(LexerMode.Start).Discard().Emit(TokenType.Pop),
                    "testeq" => Transition(LexerMode.Start).Discard().Emit(TokenType.TestEqual),
                    "testne" => Transition(LexerMode.Start).Discard().Emit(TokenType.TestNotEqual),
                    "testgt" => Transition(LexerMode.Start).Discard().Emit(TokenType.TestGreaterThan),
                    "testlt" => Transition(LexerMode.Start).Discard().Emit(TokenType.TestLessThan),
                    "jmp" => Transition(LexerMode.Start).Discard().Emit(TokenType.Jump),
                    "jmpt" => Transition(LexerMode.Start).Discard().Emit(TokenType.JumpTrue),
                    "jmpf" => Transition(LexerMode.Start).Discard().Emit(TokenType.JumpFalse),
                    "callfunc" => Transition(LexerMode.Start).Discard().Emit(TokenType.CallFunc),
                    "callinterface" => Transition(LexerMode.Start).Discard().Emit(TokenType.CallInterface),
                    "ret" => Transition(LexerMode.Start).Discard().Emit(TokenType.Return),
                    "newstruct" => Transition(LexerMode.Start).Discard().Emit(TokenType.NewStruct),
                    "ldfield" => Transition(LexerMode.Start).Discard().Emit(TokenType.LoadField),
                    "stfield" => Transition(LexerMode.Start).Discard().Emit(TokenType.StoreField),
                    "addr" => Transition(LexerMode.Start).Discard().Emit(TokenType.AddressType),
                    "i8" => Transition(LexerMode.Start).Discard().Emit(TokenType.i8Type),
                    "u8" => Transition(LexerMode.Start).Discard().Emit(TokenType.u8Type),
                    "i16" => Transition(LexerMode.Start).Discard().Emit(TokenType.i16Type),
                    "u16" => Transition(LexerMode.Start).Discard().Emit(TokenType.u16Type),
                    "i32" => Transition(LexerMode.Start).Discard().Emit(TokenType.i32Type),
                    "u32" => Transition(LexerMode.Start).Discard().Emit(TokenType.u32Type),
                    "i64" => Transition(LexerMode.Start).Discard().Emit(TokenType.i64Type),
                    "u64" => Transition(LexerMode.Start).Discard().Emit(TokenType.u64Type),
                    "f32" => Transition(LexerMode.Start).Discard().Emit(TokenType.f32Type),
                    "f64" => Transition(LexerMode.Start).Discard().Emit(TokenType.f64Type),
                    "str" => Transition(LexerMode.Start).Discard().Emit(TokenType.StringType),
                    _ => Transition(LexerMode.Start).Emit(TokenType.Identifier)
                }
            };

        public Token? End()
        {
            return Transition(LexerMode.None).Discard().Emit(TokenType.EndOfFile);
        }

        public static List<Token> LexString(string data)
        {
            using var stream = new StringReader(data);
            return Lex(stream);
        }

        public static List<Token> LexFile(string filename)
        {
            using var stream = File.OpenText(filename);
            return Lex(stream);
        }

        public static List<Token> Lex(TextReader stream)
        {
            var state = new Lexer(stream);
            var tokens = new List<Token>();

            while (state.Mode != LexerMode.None)
            {
                var token = state.Mode switch
                {
                    LexerMode.Start => state.Start(),
                    LexerMode.SkipWhitespace => state.SkipWhitespace(),
                    LexerMode.IsNegative => state.Negative(),
                    LexerMode.Digit => state.Digit(),
                    LexerMode.IsDecimal => state.IsDecimal(),
                    LexerMode.IsBinary => state.IsBinary(),
                    LexerMode.IsHexadecimal => state.IsHexadecimal(),
                    LexerMode.IsFloat => state.IsFloat(),
                    LexerMode.IsString => state.IsString(),
                    LexerMode.IsWord => state.IsWord(),
                    LexerMode.End => state.End(),
                    _ => state.Emit()
                };
                token?.AddTo(tokens);
            }

            return tokens;
        }
    }
}
