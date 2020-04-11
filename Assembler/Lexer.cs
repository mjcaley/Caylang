using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

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
            Mode = Start;
        }

        public Func<Token?> Mode { get; set; }

        public StringBuilder Lexeme { get; } = new StringBuilder();

        public int Line { get; private set; } = 1;

        public char Current { get; private set; }

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
            var previous = Current;

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

            Current = next;

            if (previous == '\n')
            {
                Line++;
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
            return new Token(type, Line);
        }

        public Token? ConsumeAndEmit(TokenType type)
        {
            return new Token(type, Line, Consume());
        }

        public Lexer Transition(Func<Token?>? mode)
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
                _ => Transition(Start).Emit()
            };

        public Token? Start() =>
            Current switch
            {
                var _ when char.IsWhiteSpace(Current) => Transition(SkipWhitespace).Emit(),
                '=' => Skip().Emit(TokenType.Equal),
                ':' => Skip().Emit(TokenType.Colon),
                '-' => Append().Transition(NumberBase).Emit(),
                '.' => ReplaceLexeme((_) => "0").Append().Transition(FloatNumber).Emit(),
                '0' => Append().Transition(NumberBase).Emit(),
                var x when 
                    x >= '1' && 
                    x <= '9' => Append().Transition(DecNumber).Emit(),
                '"' => Skip().Transition(StringValue).Emit(),
                var x when x == '_' || char.IsLetter(x) => Append().Transition(Keyword).Emit(),
                '\0' => Transition(End).Emit(),
                _ => Skip().Emit(TokenType.Error)
            };

        public Token? NumberBase() =>
            Current switch
            {
                'x' => Skip().Discard().Transition(HexNumber).Emit(),
                'b' => Skip().Discard().Transition(BinNumber).Emit(),
                '.' => Append().Transition(FloatNumber).Emit(),
                '\0' => Transition(Start).ConsumeAndEmit(TokenType.IntegerLiteral),
                var x when
                    char.IsWhiteSpace(x) ||
                    char.IsControl(x) => Transition(Start).ConsumeAndEmit(TokenType.IntegerLiteral),
                _ => Append().Discard().Transition(Start).Emit(TokenType.Error)
            };

        public Token? BinNumber() =>
            Current switch
            {
                var x when x == '0' || x == '1' => Append().Emit(),
                _ => ReplaceLexeme((lexeme) =>
                {
                    var integer = Convert.ToInt64(lexeme, 2);
                    return integer.ToString(CultureInfo.InvariantCulture);
                }).Transition(Start).ConsumeAndEmit(TokenType.IntegerLiteral)
            };

        public Token? HexNumber() =>
            Current switch
            {
                var x when
                (x >= '0' && x <= '9') ||
                (x >= 'a' && x <= 'f') ||
                (x >= 'A' && x <= 'F') => Append().Emit(),
                _ => ReplaceLexeme((lexeme) =>
                {
                    var integer = Convert.ToUInt64(lexeme, 16);
                    return integer.ToString(CultureInfo.InvariantCulture);
                }).Transition(Start).ConsumeAndEmit(TokenType.IntegerLiteral)
            };

        public Token? DecNumber() =>
            Current switch
            {
                var _ when char.IsDigit(Current) => Append().Emit(),
                '.' => Append().Transition(FloatNumber).Emit(),
                _ => Transition(Start).ConsumeAndEmit(TokenType.IntegerLiteral)
            };

        public Token? FloatNumber() =>
            Current switch
            {
                var _ when char.IsDigit(Current) => Append().Emit(),
                _ => Transition(Start).ConsumeAndEmit(TokenType.FloatLiteral)
            };

        public Token? StringValue() =>
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
                    _ => Transition(Start).Emit(TokenType.Error),
                },
                '"' => Skip().Transition(Start).ConsumeAndEmit(TokenType.StringLiteral),
                '\n' => Transition(Start).Emit(TokenType.Error),
                _ => Append().Emit()
            };

        public Token? Keyword() =>
            Current switch
            {
                var x when char.IsLetterOrDigit(x) => Append().Emit(),
                _ => Lexeme.ToString() switch
                {
                    "func" => Transition(Start).Discard().Emit(TokenType.Func),
                    "define" => Transition(Start).Discard().Emit(TokenType.Define),
                    "args" => Transition(Start).Discard().Emit(TokenType.Args),
                    "locals" => Transition(Start).Discard().Emit(TokenType.Locals),
                    "halt" => Transition(Start).Discard().Emit(TokenType.Halt),
                    "nop" => Transition(Start).Discard().Emit(TokenType.Noop),
                    "add" => Transition(Start).Discard().Emit(TokenType.Add),
                    "sub" => Transition(Start).Discard().Emit(TokenType.Subtract),
                    "mul" => Transition(Start).Discard().Emit(TokenType.Multiply),
                    "div" => Transition(Start).Discard().Emit(TokenType.Divide),
                    "mod" => Transition(Start).Discard().Emit(TokenType.Modulo),
                    "ldconst" => Transition(Start).Discard().Emit(TokenType.LoadConst),
                    "ldlocal" => Transition(Start).Discard().Emit(TokenType.LoadLocal),
                    "stlocal" => Transition(Start).Discard().Emit(TokenType.StoreLocal),
                    "pop" => Transition(Start).Discard().Emit(TokenType.Pop),
                    "testeq" => Transition(Start).Discard().Emit(TokenType.TestEqual),
                    "testne" => Transition(Start).Discard().Emit(TokenType.TestNotEqual),
                    "testgt" => Transition(Start).Discard().Emit(TokenType.TestGreaterThan),
                    "testlt" => Transition(Start).Discard().Emit(TokenType.TestLessThan),
                    "jmp" => Transition(Start).Discard().Emit(TokenType.Jump),
                    "jmpt" => Transition(Start).Discard().Emit(TokenType.JumpTrue),
                    "jmpf" => Transition(Start).Discard().Emit(TokenType.JumpFalse),
                    "callfunc" => Transition(Start).Discard().Emit(TokenType.CallFunc),
                    "callinterface" => Transition(Start).Discard().Emit(TokenType.CallInterface),
                    "ret" => Transition(Start).Discard().Emit(TokenType.Return),
                    "newstruct" => Transition(Start).Discard().Emit(TokenType.NewStruct),
                    "ldfield" => Transition(Start).Discard().Emit(TokenType.LoadField),
                    "stfield" => Transition(Start).Discard().Emit(TokenType.StoreField),
                    "newarray" => Transition(Start).Discard().Emit(TokenType.NewArray),
                    "addr" => Transition(Start).Discard().Emit(TokenType.AddressType),
                    "i8" => Transition(Start).Discard().Emit(TokenType.i8Type),
                    "u8" => Transition(Start).Discard().Emit(TokenType.u8Type),
                    "i16" => Transition(Start).Discard().Emit(TokenType.i16Type),
                    "u16" => Transition(Start).Discard().Emit(TokenType.u16Type),
                    "i32" => Transition(Start).Discard().Emit(TokenType.i32Type),
                    "u32" => Transition(Start).Discard().Emit(TokenType.u32Type),
                    "i64" => Transition(Start).Discard().Emit(TokenType.i64Type),
                    "u64" => Transition(Start).Discard().Emit(TokenType.u64Type),
                    "f32" => Transition(Start).Discard().Emit(TokenType.f32Type),
                    "f64" => Transition(Start).Discard().Emit(TokenType.f64Type),
                    "str" => Transition(Start).Discard().Emit(TokenType.StringType),
                    _ => Transition(Start).ConsumeAndEmit(TokenType.Identifier)
                }
            };

        public Token? End()
        {
            return Transition(null).Discard().Emit(TokenType.EndOfFile);
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

            while (state.Mode != null)
            {
                state.Mode()?.AddTo(tokens);
            }

            return tokens;
        }
    }
}
