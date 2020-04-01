using System;
using System.Collections.Generic;
using System.Text;
using Caylang.Assembler;
using CayLang.Assembler;
using Xunit;
using Xunit.Sdk;

namespace CayLang.Assembler.Tests
{
    public class ParserTests
    {
        [Fact]
        public void SignedIntegerConvertsToValue()
        {
            var testInput = new Token[] { new ValueToken<ulong>(TokenType.IntegerLiteral, 1, 42UL) };
            var parser = new Parser(testInput);
            //var result = parser.SignedInteger();

            //Assert.Equal(42, result);
        }
    }
}
