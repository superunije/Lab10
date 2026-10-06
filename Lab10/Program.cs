using System;
using System.IO;

namespace PascalCompiler
{
    class Program
    {
        static void Main()
        {
            string filePath =
                @"C:\Users\unije\source\repos\Lab10\Lab10\Pascal\program.pas";

            if (!File.Exists(filePath))
            {
                Console.WriteLine(
                    "Файл не найден: " + filePath);

                Console.ReadKey();
                return;
            }

            InputOutput.Init(filePath);

            LexicalAnalyzer lexicalAnalyzer =
                new LexicalAnalyzer();

            SyntaxAnalyzer syntaxAnalyzer =
                new SyntaxAnalyzer(lexicalAnalyzer);

            // lexicalAnalyzer.Analyze();

            syntaxAnalyzer.Analyze();

            InputOutput.Finish();

            Console.ReadKey();
        }
    }
}
