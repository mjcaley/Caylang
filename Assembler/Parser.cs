using System.Globalization;
using Caylang.Assembler.ParseTree;
using CayToken = Caylang.Assembler.Token;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<Caylang.Assembler.Token>;
using Pidgin.Permutation;
using System.Collections.Generic;

namespace Caylang.Assembler
{
    public static class Parser
    {
        public static Parser<CayToken, CayToken> CaylangToken(TokenType type)
        {
            return Token(t => t.Type == type);
        }

        private static readonly Parser<CayToken, Literal> PositiveInteger =
            CaylangToken(TokenType.IntegerLiteral).Bind(t => OneOf(
                CaylangToken(TokenType.Integer8Type).ThenReturn(new Integer8Literal(sbyte.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Integer16Type).ThenReturn(new Integer16Literal(short.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Integer32Type).ThenReturn(new Integer32Literal(int.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Integer64Type).ThenReturn(new Integer64Literal(long.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.UInteger8Type).ThenReturn(new UnsignedInteger8Literal(byte.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.UInteger16Type).ThenReturn(new UnsignedInteger16Literal(ushort.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.UInteger32Type).ThenReturn(new UnsignedInteger32Literal(uint.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.UInteger64Type).ThenReturn(new UnsignedInteger64Literal(ulong.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>()
            ));

        private static readonly Parser<CayToken, Literal> NegativeInteger =
            CaylangToken(TokenType.IntegerLiteral).Bind(t => OneOf(
                CaylangToken(TokenType.Integer8Type).ThenReturn(new Integer8Literal((sbyte)-sbyte.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Integer16Type).ThenReturn(new Integer16Literal((short)-short.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Integer32Type).ThenReturn(new Integer32Literal(-int.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Integer64Type).ThenReturn(new Integer64Literal(-long.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>()
                ));

        private static readonly Parser<CayToken, Literal> PositiveFloat =
            CaylangToken(TokenType.FloatLiteral).Bind(t => OneOf(
                CaylangToken(TokenType.Float32Type).ThenReturn(new Float32Literal(float.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Float64Type).ThenReturn(new Float64Literal(double.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>()
                ));

        private static readonly Parser<CayToken, Literal> NegativeFloat =
            CaylangToken(TokenType.FloatLiteral).Bind(t => OneOf(
                CaylangToken(TokenType.Float32Type).ThenReturn(new Float32Literal(-float.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>(),
                CaylangToken(TokenType.Float64Type).ThenReturn(new Float64Literal(-double.Parse(t.Value, CultureInfo.InvariantCulture), t.Line)).OfType<Literal>()
            ));

        private static readonly Parser<CayToken, Literal> PositiveNumber =
            OneOf(PositiveInteger, PositiveFloat);

        private static readonly Parser<CayToken, Literal> NegativeNumber =
            CaylangToken(TokenType.Negative).Then(OneOf(NegativeInteger, NegativeFloat));

        private static readonly Parser<CayToken, StringLiteral> StringRule =
            CaylangToken(TokenType.StringLiteral).Before(CaylangToken(TokenType.StringType).Optional())
                .Select(t => new StringLiteral(t.Value, t.Line));

        private static readonly Parser<CayToken, IdentifierLiteral> Identifier =
            CaylangToken(TokenType.Identifier).Select(t => new IdentifierLiteral(t.Value, t.Line));
        
        public static readonly Parser<CayToken, Literal> Literal =
            OneOf(
                PositiveNumber,
                NegativeNumber,
                StringRule.OfType<Literal>(),
                Identifier.OfType<Literal>()
            );

        private static readonly Parser<CayToken, Statement> Label =
            CaylangToken(TokenType.Identifier).Before(CaylangToken(TokenType.Colon))
                .Select<Statement>(t => new Label(t.Value, t.Line))
                .Labelled("label statement");

        private static readonly Parser<CayToken, Literal> IndexLiteral = 
            OneOf(
                Identifier.OfType<Literal>(),
                CaylangToken(TokenType.IntegerLiteral)
                    .Before(CaylangToken(TokenType.UInteger16Type).Optional())
                    .Select<Literal>(
                        t => new UnsignedInteger16Literal(
                            ushort.Parse(t.Value, CultureInfo.InvariantCulture),
                            t.Line))
            ); 
        
        private static readonly Parser<CayToken, FieldType> FieldTypes =
             OneOf(
                 CaylangToken(TokenType.Integer8Type).ThenReturn(FieldType.Integer8),
                 CaylangToken(TokenType.Integer16Type).ThenReturn(FieldType.Integer16),
                 CaylangToken(TokenType.Integer32Type).ThenReturn(FieldType.Integer32),
                 CaylangToken(TokenType.Integer64Type).ThenReturn(FieldType.Integer64),
                 CaylangToken(TokenType.UInteger8Type).ThenReturn(FieldType.UnsignedInteger8),
                 CaylangToken(TokenType.UInteger16Type).ThenReturn(FieldType.UnsignedInteger16),
                 CaylangToken(TokenType.UInteger32Type).ThenReturn(FieldType.UnsignedInteger32),
                 CaylangToken(TokenType.UInteger64Type).ThenReturn(FieldType.UnsignedInteger64),
                 CaylangToken(TokenType.Float32Type).ThenReturn(FieldType.FloatingPoint32),
                 CaylangToken(TokenType.Float64Type).ThenReturn(FieldType.FloatingPoint64),
                 CaylangToken(TokenType.AddressType).ThenReturn(FieldType.Address)
             );

        private static readonly Parser<CayToken, Statement> Instruction =
            OneOf(
                CaylangToken(TokenType.Halt).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Halt, t.Line)),
                
                CaylangToken(TokenType.Pop).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Pop, t.Line)),
                
                CaylangToken(TokenType.Noop).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Noop, t.Line)),

        #region Arithmetic instructions
                CaylangToken(TokenType.Add).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Add, t.Line)),

                CaylangToken(TokenType.Subtract).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Subtract, t.Line)),

                CaylangToken(TokenType.Multiply).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Multiply, t.Line)),

                CaylangToken(TokenType.Divide).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Divide, t.Line)),

                CaylangToken(TokenType.Modulo).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Modulo, t.Line)),
        #endregion

        #region Local instructions
                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.LoadConst, operand, instruction.Line),
                    CaylangToken(TokenType.LoadConst),
                    Literal).OfType<Statement>(),

                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.LoadLocal, operand, instruction.Line),
                    CaylangToken(TokenType.LoadLocal),
                    IndexLiteral).OfType<Statement>(),

                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.StoreLocal, operand, instruction.Line),
                    CaylangToken(TokenType.StoreLocal),
                    IndexLiteral).OfType<Statement>(),
        #endregion
                
        #region Test instructions
                CaylangToken(TokenType.TestEqual).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.TestEqual, t.Line)),

                CaylangToken(TokenType.TestNotEqual).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.TestNotEqual, t.Line)),

                CaylangToken(TokenType.TestGreaterThan).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.TestGreaterThan, t.Line)),

                CaylangToken(TokenType.TestLessThan).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.TestLessThan, t.Line)),
        #endregion

        #region Jump instructions
                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.Jump, operand, instruction.Line),
                    CaylangToken(TokenType.Jump),
                    Identifier).OfType<Statement>(),
                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.JumpTrue, operand, instruction.Line),
                    CaylangToken(TokenType.JumpTrue),
                    Identifier).OfType<Statement>(),
                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.JumpFalse, operand, instruction.Line),
                    CaylangToken(TokenType.JumpFalse),
                    Identifier).OfType<Statement>(),
        #endregion

        #region Call instructions
                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.CallFunc, operand, instruction.Line),
                    CaylangToken(TokenType.CallFunc),
                    Identifier).OfType<Statement>(),

                CaylangToken(TokenType.CallVirtual).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.CallVirtual, t.Line)),

                CaylangToken(TokenType.Return).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.Return, t.Line)),
        #endregion

        #region Structure instructions
                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.NewStruct, operand, instruction.Line),
                    CaylangToken(TokenType.NewStruct),
                    Identifier).OfType<Statement>(),

                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.LoadField, operand, instruction.Line),
                    CaylangToken(TokenType.LoadField),
                    IndexLiteral).OfType<Statement>(),

                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.StoreField, operand, instruction.Line),
                    CaylangToken(TokenType.StoreField),
                    IndexLiteral).OfType<Statement>(),
        #endregion

        #region Array instructions
                Map(
                    (instruction, operand) => new UnaryInstruction(ParseTree.Instruction.NewArray, operand, instruction.Line),
                    CaylangToken(TokenType.NewArray),
                    Identifier).OfType<Statement>(),

                CaylangToken(TokenType.LoadElement).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.LoadElement, t.Line)),

                CaylangToken(TokenType.StoreElement).Select<Statement>(t => new NullaryInstruction(ParseTree.Instruction.StoreElement, t.Line))
        #endregion
            );

        public static readonly Parser<CayToken, Statement> Statement =
            OneOf(Label, Instruction);

        public static readonly Parser<CayToken, IEnumerable<Statement>> Statements =
            Statement.AtLeastOnce();

        #region Function parsers
        private static readonly Parser<CayToken, CayToken> FunctionLocals =
            CaylangToken(TokenType.Locals).Then(CaylangToken(TokenType.Equal)).Then(CaylangToken(TokenType.IntegerLiteral));

        private static readonly Parser<CayToken, CayToken> FunctionArguments =
            CaylangToken(TokenType.Args).Then(CaylangToken(TokenType.Equal)).Then(CaylangToken(TokenType.IntegerLiteral));

        private static readonly Parser<CayToken, FunctionParameters> FunctionParameters =
            PermutationParser.Create<CayToken>()
                .Add(FunctionArguments)
                .Add(FunctionLocals)
                .Build()
                .Select(
                    tokens => {
                        var ((_, args), locals) = tokens;

                        return new FunctionParameters(
                            int.Parse(locals.Value, CultureInfo.InvariantCulture),
                            int.Parse(args.Value, CultureInfo.InvariantCulture)
                        );
                    });

        private static readonly Parser<CayToken, FunctionDefinition> FunctionDefinition =
            Map(
                (_, name, parameters, statements) => new FunctionDefinition(name.Value, parameters, statements, _.Line),
                CaylangToken(TokenType.Func),
                CaylangToken(TokenType.Identifier),
                FunctionParameters,
                Statements);
        #endregion

        #region Constant parser
        private static readonly Parser<CayToken, ConstantDefinition> ConstantDefinition =
            Map(
                (d, i, _, o) => new ConstantDefinition(i.Value, o, d.Line),
                CaylangToken(TokenType.Define),
                Identifier.OfType<IdentifierLiteral>(),
                CaylangToken(TokenType.Equal),
                Literal
                );
        #endregion

        #region Struct parser
        private static readonly Parser<CayToken, StructDefinition> StructDefinition =
            Map(
                (instruction, name, fieldType) => new StructDefinition(name.Value, fieldType, instruction.Line),
                CaylangToken(TokenType.Struct),
                Identifier.OfType<IdentifierLiteral>(),
                FieldTypes.Separated(CaylangToken(TokenType.Comma))
                );
        #endregion

        public static readonly Parser<CayToken, Definition> Definition =
            OneOf(
                FunctionDefinition.OfType<Definition>(),
                ConstantDefinition.OfType<Definition>(),
                StructDefinition.OfType<Definition>()
                );

        public static readonly Parser<CayToken, IEnumerable<Definition>> Start =
            Definition.AtLeastOnce();
    }
}
