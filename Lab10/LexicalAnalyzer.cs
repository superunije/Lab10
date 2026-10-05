using System;

namespace PascalCompiler
{
    class LexicalAnalyzer
    {
        // Ключевые слова.
        public const byte dosy = 1;
        public const byte ifsy = 2;
        public const byte ofsy = 3;
        public const byte tosy = 4;
        public const byte endsy = 5;
        public const byte varsy = 6;
        public const byte forsy = 7;
        public const byte thensy = 8;
        public const byte elsesy = 9;
        public const byte typesy = 10;
        public const byte beginsy = 11;
        public const byte whilesy = 12;
        public const byte arraysy = 13;
        public const byte constsy = 14;
        public const byte untilsy = 15;
        public const byte downtosy = 16;
        public const byte repeatsy = 17;
        public const byte programsy = 18;
        public const byte functionsy = 19;
        public const byte proceduresy = 20;

        public const byte recordsy = 44;
        public const byte setsy = 45;
        public const byte filesy = 46;
        public const byte packedsy = 47;

        public const byte integerty = 48;
        public const byte realsy = 49;
        public const byte charsy = 50;
        public const byte booleansy = 51;

        public const byte casesy = 52;
        public const byte withsy = 53;
        public const byte gotosy = 54;
        public const byte labelsy = 55;

        public const byte andsy = 56;
        public const byte orsy = 57;
        public const byte notsy = 58;

        public const byte divsy = 59;
        public const byte modsy = 60;

        public const byte insy = 61;
        public const byte nilsy = 62;

        // Другие лексемы.
        public const byte ident = 21;
        public const byte intconst = 22;
        public const byte realconst = 23;
        public const byte charconst = 24;

        // Арифметические операции.
        public const byte plus = 25;
        public const byte minus = 26;
        public const byte multiply = 27;
        public const byte divide = 28;

        // Операции сравнения.
        public const byte equal = 29;
        public const byte less = 30;
        public const byte greater = 31;
        public const byte lessequal = 32;
        public const byte greaterequal = 33;
        public const byte notequal = 34;

        // Присваивание.
        public const byte assign = 35;

        // Разделители.
        public const byte leftparen = 36;
        public const byte rightparen = 37;
        public const byte leftbracket = 38;
        public const byte rightbracket = 39;
        public const byte comma = 40;
        public const byte colon = 41;
        public const byte semicolon = 42;
        public const byte dot = 43;

        // Дополнительные символы.
        public const byte range = 63;

        public const byte unknown = 0;

        // Лексема для синтаксического анализатора.
        public struct Lexeme
        {
            public byte Code;
            public string Text;

            public Lexeme(byte code, string text)
            {
                Code = code;
                Text = text;
            }
        }

        private Keywords _keywords;

        public LexicalAnalyzer()
        {
            _keywords = new Keywords();
        }

        // Проверочный запуск лексического анализа.
        public void Analyze()
        {
            Console.WriteLine("ЛЕКСИЧЕСКИЙ АНАЛИЗ");
            Console.WriteLine("------------------");

            while (!InputOutput.EndOfFile)
            {
                Lexeme lexeme = GetNextLexeme();

                if (lexeme.Code != 0)
                {
                    Console.WriteLine(
                        "Код лексемы: " +
                        lexeme.Code);
                }
            }

            Console.WriteLine("------------------");
        }

        // Получить следующую лексему.
        public Lexeme GetNextLexeme()
        {
            // Пропускаем пробелы и табуляцию.
            while (InputOutput.CurrentChar == ' ' ||
                   InputOutput.CurrentChar == '\t')
            {
                InputOutput.NextCh();
            }

            if (InputOutput.EndOfFile)
            {
                return new Lexeme(0, "");
            }

            // Идентификатор или ключевое слово.
            if (char.IsLetter(InputOutput.CurrentChar))
            {
                string name = "";

                while (char.IsLetterOrDigit(
                    InputOutput.CurrentChar))
                {
                    name += InputOutput.CurrentChar;
                    InputOutput.NextCh();
                }

                byte keywordCode =
                    _keywords.CheckKeyword(name);

                if (keywordCode != 0)
                {
                    return new Lexeme(
                        keywordCode,
                        name);
                }

                return new Lexeme(
                    ident,
                    name);
            }

            // Числовая константа.
            if (char.IsDigit(InputOutput.CurrentChar))
            {
                string number = "";

                while (char.IsDigit(
                    InputOutput.CurrentChar))
                {
                    number += InputOutput.CurrentChar;
                    InputOutput.NextCh();
                }

                // Вещественное число.
                if (InputOutput.CurrentChar == '.')
                {
                    InputOutput.NextCh();

                    // Проверяем диапазон ...
                    if (InputOutput.CurrentChar == '.')
                    {
                        return new Lexeme(
                            intconst,
                            number);
                    }

                    // После точки должны быть цифры.
                    if (char.IsDigit(
                        InputOutput.CurrentChar))
                    {
                        number += ".";

                        while (char.IsDigit(
                            InputOutput.CurrentChar))
                        {
                            number +=
                                InputOutput.CurrentChar;

                            InputOutput.NextCh();
                        }

                        return new Lexeme(
                            realconst,
                            number);
                    }

                    return new Lexeme(
                        intconst,
                        number);
                }

                return new Lexeme(
                    intconst,
                    number);
            }

            // Символьная константа.
            if (InputOutput.CurrentChar == '\'')
            {
                string value = "";

                InputOutput.NextCh();

                if (InputOutput.CurrentChar != '\0')
                {
                    value +=
                        InputOutput.CurrentChar;

                    InputOutput.NextCh();

                    if (InputOutput.CurrentChar == '\'')
                    {
                        InputOutput.NextCh();

                        return new Lexeme(
                            charconst,
                            value);
                    }
                }

                return new Lexeme(
                    unknown,
                    value);
            }

            // Операторы и разделители.
            switch (InputOutput.CurrentChar)
            {
                case '+':
                    InputOutput.NextCh();

                    return new Lexeme(
                        plus,
                        "+");

                case '-':
                    InputOutput.NextCh();

                    return new Lexeme(
                        minus,
                        "-");

                case '*':
                    InputOutput.NextCh();

                    // Если это конец комментария *).
                    if (InputOutput.CurrentChar == ')')
                    {
                        InputOutput.NextCh();

                        return GetNextLexeme();
                    }

                    return new Lexeme(
                        multiply,
                        "*");

                case '/':
                    InputOutput.NextCh();

                    return new Lexeme(
                        divide,
                        "/");

                case '=':
                    InputOutput.NextCh();

                    return new Lexeme(
                        equal,
                        "=");

                case '<':
                    InputOutput.NextCh();

                    if (InputOutput.CurrentChar == '=')
                    {
                        InputOutput.NextCh();

                        return new Lexeme(
                            lessequal,
                            "<=");
                    }

                    if (InputOutput.CurrentChar == '>')
                    {
                        InputOutput.NextCh();

                        return new Lexeme(
                            notequal,
                            "<>");
                    }

                    return new Lexeme(
                        less,
                        "<");

                case '>':
                    InputOutput.NextCh();

                    if (InputOutput.CurrentChar == '=')
                    {
                        InputOutput.NextCh();

                        return new Lexeme(
                            greaterequal,
                            ">=");
                    }

                    return new Lexeme(
                        greater,
                        ">");

                case ':':
                    InputOutput.NextCh();

                    if (InputOutput.CurrentChar == '=')
                    {
                        InputOutput.NextCh();

                        return new Lexeme(
                            assign,
                            ":=");
                    }

                    return new Lexeme(
                        colon,
                        ":");

                case '(':
                    InputOutput.NextCh();

                    // Начало комментария (*.
                    if (InputOutput.CurrentChar == '*')
                    {
                        SkipComment();

                        return GetNextLexeme();
                    }

                    return new Lexeme(
                        leftparen,
                        "(");

                case ')':
                    InputOutput.NextCh();

                    return new Lexeme(
                        rightparen,
                        ")");

                case '[':
                    InputOutput.NextCh();

                    return new Lexeme(
                        leftbracket,
                        "[");

                case ']':
                    InputOutput.NextCh();

                    return new Lexeme(
                        rightbracket,
                        "]");

                case ',':
                    InputOutput.NextCh();

                    return new Lexeme(
                        comma,
                        ",");

                case ';':
                    InputOutput.NextCh();

                    return new Lexeme(
                        semicolon,
                        ";");

                case '.':
                    InputOutput.NextCh();

                    if (InputOutput.CurrentChar == '.')
                    {
                        InputOutput.NextCh();

                        return new Lexeme(
                            range,
                            "..");
                    }

                    return new Lexeme(
                        dot,
                        ".");

                case '{':
                    SkipBraceComment();

                    return GetNextLexeme();

                default:
                    InputOutput.TextPosition position =
                        InputOutput.CurrentPosition;

                    InputOutput.Error(
                        4,
                        position);

                    string unknownSymbol =
                        InputOutput.CurrentChar.ToString();

                    InputOutput.NextCh();

                    return new Lexeme(
                        unknown,
                        unknownSymbol);
            }
        }

        // Пропуск комментария { ... }.
        private void SkipBraceComment()
        {
            InputOutput.NextCh();

            while (!InputOutput.EndOfFile)
            {
                if (InputOutput.CurrentChar == '}')
                {
                    InputOutput.NextCh();
                    return;
                }

                InputOutput.NextCh();
            }
        }

        // Пропуск комментария (* ... *).
        private void SkipComment()
        {
            InputOutput.NextCh();

            while (!InputOutput.EndOfFile)
            {
                if (InputOutput.CurrentChar == '*')
                {
                    InputOutput.NextCh();

                    if (InputOutput.CurrentChar == ')')
                    {
                        InputOutput.NextCh();
                        return;
                    }
                }
                else
                {
                    InputOutput.NextCh();
                }
            }
        }
    }
}