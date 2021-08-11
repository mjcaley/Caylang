using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;

namespace Caylang.Assembler
{
    class Program
    {
        public class Options
        {
            #nullable disable
            [Value(0, Min=1, HelpText = "Program filenames", Required = true, MetaName = "FILENAME")]
            public IEnumerable<string> Filenames { get; set; }
            #nullable enable
        }

        enum ExitCode
        {
            Success = 0,
            NoArgs,
            FileNotFound,
            ParseError
        }

        static ExitCode Compile(Options options)
        {
            var filenames = options.Filenames.ToList();

            if (!File.Exists(filenames[0]))
            {
                Environment.Exit((int)ExitCode.FileNotFound);
            }

			var programText = File.ReadAllText(filenames[0]);
            var lexer = new AsmLexer(programText);
            var parser = new AsmParser(lexer);

            var tree = parser.ParseModule();
            if (!tree.IsOk)
            {
                Console.WriteLine("Parse failed");
                return ExitCode.ParseError;
            }

            var printerPass = new ParseTreePrinter();
            printerPass.Visit(tree.Ok.Value);

            return ExitCode.Success;
        }

        static ExitCode Fail(IEnumerable<Error> errors)
        {
            return ExitCode.NoArgs;
        }

        static void Main(string[] args)
        {
            var exitCode = Parser.Default.ParseArguments<Options>(args).MapResult(
                parsed => Compile(parsed),
                errors => Fail(errors)
            );

            Environment.Exit((int)exitCode);
        }
    }
}
