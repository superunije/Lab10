using System;

namespace PascalCompiler
{
    class SyntaxAnalyzer
    {
        private readonly LexicalAnalyzer _lexicalAnalyzer;

        private LexicalAnalyzer.Lexeme _current;
        private LexicalAnalyzer.Lexeme _next;

        private bool _hasErrors = false;
        private int _errorCount = 0;

        public SyntaxAnalyzer(LexicalAnalyzer lexicalAnalyzer)
        {
            _lexicalAnalyzer = lexicalAnalyzer;

            _current = _lexicalAnalyzer.GetNextLexeme();
            _next = _lexicalAnalyzer.GetNextLexeme();
        }

        public void Analyze()
        {
            Console.WriteLine("СИНТАКСИЧЕСКИЙ АНАЛИЗ:\n");

            ParseProgram();

            if (_hasErrors)
            {
                Console.WriteLine(
                    "Синтаксический анализ завершён с ошибками.");

                Console.WriteLine(
                    "Количество синтаксических ошибок: " +
                    _errorCount);
            }
            else
            {
                Console.WriteLine(
                    "Синтаксический анализ завершён успешно.");

                Console.WriteLine(
                    "Количество синтаксических ошибок: 0");
            }
        }

        // Переход к следующей лексеме.
        private void NextLexeme()
        {
            _current = _next;
            _next = _lexicalAnalyzer.GetNextLexeme();
        }

        // Вывод сообщения о синтаксической ошибке.
        private void SyntaxError(string message)
        {
            _hasErrors = true;
            _errorCount++;

            Console.WriteLine(
                "Синтаксическая ошибка: " +
                message +
                ". Лексема: \"" +
                _current.Text +
                "\"");
        }

        // Разбор программы.
        private void ParseProgram()
        {
            if (_current.Code != LexicalAnalyzer.programsy)
            {
                SyntaxError("ожидалось program");

                RecoverTo(
                    LexicalAnalyzer.programsy,
                    LexicalAnalyzer.varsy,
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.dot);

                if (_current.Code != LexicalAnalyzer.programsy)
                {
                    return;
                }
            }

            NextLexeme();

            // Имя программы.
            if (_current.Code == LexicalAnalyzer.ident)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось имя программы");
            }

            // Точка с запятой после имени программы.
            if (_current.Code == LexicalAnalyzer.semicolon)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалась ';' после имени программы");

                RecoverTo(
                    LexicalAnalyzer.varsy,
                    LexicalAnalyzer.beginsy);

                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                }
            }

            // Раздел переменных.
            if (_current.Code == LexicalAnalyzer.varsy)
            {
                ParseVariableSection();
            }

            // Объявления процедур.
            while (_current.Code == LexicalAnalyzer.proceduresy)
            {
                ParseProcedureDeclaration();
            }

            // Основной составной оператор.
            ParseCompoundStatement();

            // Точка в конце программы.
            if (_current.Code == LexicalAnalyzer.dot)
            {
                NextLexeme();
            }
            else if (!InputOutput.EndOfFile)
            {
                SyntaxError("ожидалась '.' в конце программы");

                RecoverTo(LexicalAnalyzer.dot);

                if (_current.Code == LexicalAnalyzer.dot)
                {
                    NextLexeme();
                }
            }
        }

        // Разбор раздела переменных.
        private void ParseVariableSection()
        {
            NextLexeme();

            /*
             * Объявление переменной начинается с идентификатора,
             * после которого обязательно должен идти ':'.
             *
             * Благодаря проверке _next.Code оператор вроде
             * x := 10 не будет принят за объявление переменной.
             */
            while (_current.Code == LexicalAnalyzer.ident &&
                   _next.Code == LexicalAnalyzer.colon)
            {
                ParseVariableDeclaration();
            }
        }

        // Разбор объявления переменной.
        private void ParseVariableDeclaration()
        {
            // Имя переменной.
            if (_current.Code == LexicalAnalyzer.ident)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидался идентификатор");
                return;
            }

            // Двоеточие.
            if (_current.Code == LexicalAnalyzer.colon)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидался ':'");

                RecoverTo(
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.proceduresy);

                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                }

                return;
            }

            // Тип переменной.
            ParseType();

            // Точка с запятой.
            if (_current.Code == LexicalAnalyzer.semicolon)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError(
                    "ожидалась ';' после объявления переменной");

                RecoverTo(
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.proceduresy);

                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                }
            }
        }

        // Разбор типа переменной.
        private void ParseType()
        {
            if (_current.Code == LexicalAnalyzer.integerty ||
                _current.Code == LexicalAnalyzer.realsy ||
                _current.Code == LexicalAnalyzer.charsy ||
                _current.Code == LexicalAnalyzer.booleansy)
            {
                NextLexeme();
                return;
            }

            if (_current.Code == LexicalAnalyzer.arraysy)
            {
                ParseArrayType();
                return;
            }

            SyntaxError(
                "ожидался стандартный тип или array");

            RecoverTo(
                LexicalAnalyzer.semicolon,
                LexicalAnalyzer.beginsy,
                LexicalAnalyzer.proceduresy);
        }

        // Разбор объявления массива.
        private void ParseArrayType()
        {
            NextLexeme();

            // Открывающая квадратная скобка.
            if (_current.Code == LexicalAnalyzer.leftbracket)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалась '['");

                RecoverTo(
                    LexicalAnalyzer.leftbracket,
                    LexicalAnalyzer.semicolon);

                if (_current.Code == LexicalAnalyzer.leftbracket)
                {
                    NextLexeme();
                }
                else
                {
                    return;
                }
            }

            // Нижняя граница.
            if (_current.Code == LexicalAnalyzer.intconst)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалась нижняя граница массива");
            }

            // Диапазон.
            if (_current.Code == LexicalAnalyzer.range)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидался '..'");

                /*
                 * Дополнительная обработка случая,
                 * когда лексический анализатор выдал
                 * две точки как отдельные лексемы.
                 */
                if (_current.Code == LexicalAnalyzer.dot &&
                    _next.Code == LexicalAnalyzer.dot)
                {
                    NextLexeme();
                    NextLexeme();
                }
                else
                {
                    RecoverTo(
                        LexicalAnalyzer.rightbracket,
                        LexicalAnalyzer.semicolon);
                }
            }

            // Верхняя граница.
            if (_current.Code == LexicalAnalyzer.intconst)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалась верхняя граница массива");
            }

            // Закрывающая квадратная скобка.
            if (_current.Code == LexicalAnalyzer.rightbracket)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалась ']'");
            }

            // Ключевое слово of.
            if (_current.Code == LexicalAnalyzer.ofsy)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось of");
                return;
            }

            // Тип элементов массива.
            if (_current.Code == LexicalAnalyzer.integerty ||
                _current.Code == LexicalAnalyzer.realsy ||
                _current.Code == LexicalAnalyzer.charsy ||
                _current.Code == LexicalAnalyzer.booleansy)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидался тип элементов массива");
            }
        }

        // Разбор объявления процедуры.
        private void ParseProcedureDeclaration()
        {
            NextLexeme();

            // Имя процедуры.
            if (_current.Code == LexicalAnalyzer.ident)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось имя процедуры");
            }

            // Список параметров.
            if (_current.Code == LexicalAnalyzer.leftparen)
            {
                ParseParameterList();
            }

            // Точка с запятой после заголовка.
            if (_current.Code == LexicalAnalyzer.semicolon)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError(
                    "ожидалась ';' после заголовка процедуры");

                RecoverTo(
                    LexicalAnalyzer.varsy,
                    LexicalAnalyzer.beginsy);

                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                }
            }

            // Локальные переменные.
            if (_current.Code == LexicalAnalyzer.varsy)
            {
                ParseVariableSection();
            }

            // Вложенные процедуры.
            while (_current.Code == LexicalAnalyzer.proceduresy)
            {
                ParseProcedureDeclaration();
            }

            // Тело процедуры.
            ParseCompoundStatement();

            // Точка с запятой после процедуры.
            if (_current.Code == LexicalAnalyzer.semicolon)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError(
                    "ожидалась ';' после процедуры");

                RecoverTo(
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.dot);

                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                }
            }
        }

        // Разбор списка параметров процедуры.
        private void ParseParameterList()
        {
            NextLexeme();

            // Пустой список параметров.
            if (_current.Code == LexicalAnalyzer.rightparen)
            {
                NextLexeme();
                return;
            }

            while (!InputOutput.EndOfFile)
            {
                // Имя параметра.
                if (_current.Code == LexicalAnalyzer.ident)
                {
                    NextLexeme();
                }
                else
                {
                    SyntaxError("ожидался идентификатор параметра");

                    RecoverTo(
                        LexicalAnalyzer.semicolon,
                        LexicalAnalyzer.rightparen);

                    if (_current.Code == LexicalAnalyzer.semicolon)
                    {
                        NextLexeme();
                        continue;
                    }

                    break;
                }

                // Двоеточие.
                if (_current.Code == LexicalAnalyzer.colon)
                {
                    NextLexeme();
                }
                else
                {
                    SyntaxError("ожидался ':' после параметра");

                    RecoverTo(
                        LexicalAnalyzer.semicolon,
                        LexicalAnalyzer.rightparen);

                    if (_current.Code == LexicalAnalyzer.semicolon)
                    {
                        NextLexeme();
                        continue;
                    }

                    break;
                }

                // Тип параметра.
                if (_current.Code == LexicalAnalyzer.integerty ||
                    _current.Code == LexicalAnalyzer.realsy ||
                    _current.Code == LexicalAnalyzer.charsy ||
                    _current.Code == LexicalAnalyzer.booleansy)
                {
                    NextLexeme();
                }
                else
                {
                    SyntaxError("ожидался тип параметра");

                    RecoverTo(
                        LexicalAnalyzer.semicolon,
                        LexicalAnalyzer.rightparen);
                }

                // Следующий параметр.
                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                    continue;
                }

                break;
            }

            // Закрывающая скобка.
            if (_current.Code == LexicalAnalyzer.rightparen)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалась ')' после параметров");

                RecoverTo(
                    LexicalAnalyzer.rightparen,
                    LexicalAnalyzer.semicolon);

                if (_current.Code == LexicalAnalyzer.rightparen)
                {
                    NextLexeme();
                }
            }
        }

        // Разбор составного оператора.
        private void ParseCompoundStatement()
        {
            // Правильный вариант: begin.
            if (_current.Code == LexicalAnalyzer.beginsy)
            {
                NextLexeme();
            }
            else
            {
                /*
                 * Если вместо begin находится идентификатор,
                 * считаем его ошибочной лексемой и продолжаем
                 * разбор операторов.
                 *
                 * Например:
                 *
                 * beginaa
                 *   x := 10;
                 *
                 * Ошибкой является только beginaa.
                 */
                if (_current.Code == LexicalAnalyzer.ident)
                {
                    SyntaxError("ожидалось begin");

                    NextLexeme();

                    ParseStatementList();
                    return;
                }

                SyntaxError("ожидалось begin");

                RecoverTo(
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.endsy,
                    LexicalAnalyzer.dot);

                if (_current.Code == LexicalAnalyzer.beginsy)
                {
                    NextLexeme();
                }
                else
                {
                    return;
                }
            }

            ParseStatementList();

            // Конец составного оператора.
            if (_current.Code == LexicalAnalyzer.endsy)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось end");

                RecoverTo(
                    LexicalAnalyzer.endsy,
                    LexicalAnalyzer.dot);

                if (_current.Code == LexicalAnalyzer.endsy)
                {
                    NextLexeme();
                }
            }
        }

        // Разбор последовательности операторов.
        private void ParseStatementList()
        {
            while (!InputOutput.EndOfFile &&
                   _current.Code != LexicalAnalyzer.endsy &&
                   _current.Code != LexicalAnalyzer.dot)
            {
                ParseStatement();

                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                }
                else if (_current.Code != LexicalAnalyzer.endsy &&
                         _current.Code != LexicalAnalyzer.dot)
                {
                    SyntaxError("ожидалась ';'");

                    RecoverTo(
                        LexicalAnalyzer.semicolon,
                        LexicalAnalyzer.endsy,
                        LexicalAnalyzer.dot);

                    if (_current.Code == LexicalAnalyzer.semicolon)
                    {
                        NextLexeme();
                    }
                }
            }
        }

        // Разбор оператора.
        private void ParseStatement()
        {
            if (_current.Code == LexicalAnalyzer.ident)
            {
                ParseIdentifierStatement();
                return;
            }

            if (_current.Code == LexicalAnalyzer.beginsy)
            {
                ParseCompoundStatement();
                return;
            }

            if (_current.Code == LexicalAnalyzer.ifsy)
            {
                ParseIfStatement();
                return;
            }

            if (_current.Code == LexicalAnalyzer.whilesy)
            {
                ParseWhileStatement();
                return;
            }

            if (_current.Code == LexicalAnalyzer.forsy)
            {
                ParseForStatement();
                return;
            }

            if (_current.Code == LexicalAnalyzer.repeatsy)
            {
                ParseRepeatStatement();
                return;
            }

            // Пустой оператор.
            if (_current.Code == LexicalAnalyzer.semicolon)
            {
                return;
            }

            SyntaxError("ожидался оператор");

            RecoverTo(
                LexicalAnalyzer.semicolon,
                LexicalAnalyzer.endsy,
                LexicalAnalyzer.dot);
        }

        // Разбор оператора, начинающегося с идентификатора.
        private void ParseIdentifierStatement()
        {
            NextLexeme();

            // Индексированная переменная.
            if (_current.Code == LexicalAnalyzer.leftbracket)
            {
                ParseIndexes();
            }

            // Присваивание.
            if (_current.Code == LexicalAnalyzer.assign)
            {
                NextLexeme();

                ParseExpression();
                return;
            }

            // Вызов процедуры.
            if (_current.Code == LexicalAnalyzer.leftparen)
            {
                ParseArgumentList();
                return;
            }

            SyntaxError(
                "ожидалось ':=' или вызов процедуры");

            RecoverTo(
                LexicalAnalyzer.semicolon,
                LexicalAnalyzer.endsy,
                LexicalAnalyzer.dot);
        }

        // Разбор индексов массива.
        private void ParseIndexes()
        {
            while (_current.Code == LexicalAnalyzer.leftbracket)
            {
                NextLexeme();

                ParseExpression();

                if (_current.Code == LexicalAnalyzer.rightbracket)
                {
                    NextLexeme();
                }
                else
                {
                    SyntaxError("ожидалась ']'");

                    RecoverTo(
                        LexicalAnalyzer.rightbracket,
                        LexicalAnalyzer.assign,
                        LexicalAnalyzer.semicolon);

                    if (_current.Code == LexicalAnalyzer.rightbracket)
                    {
                        NextLexeme();
                    }
                }
            }
        }

        // Разбор аргументов вызова процедуры.
        private void ParseArgumentList()
        {
            if (_current.Code != LexicalAnalyzer.leftparen)
            {
                return;
            }

            NextLexeme();

            // Пустые скобки.
            if (_current.Code == LexicalAnalyzer.rightparen)
            {
                NextLexeme();
                return;
            }

            while (!InputOutput.EndOfFile)
            {
                ParseExpression();

                if (_current.Code == LexicalAnalyzer.comma)
                {
                    NextLexeme();
                    continue;
                }

                break;
            }

            if (_current.Code == LexicalAnalyzer.rightparen)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалась ')'");

                RecoverTo(
                    LexicalAnalyzer.rightparen,
                    LexicalAnalyzer.semicolon);

                if (_current.Code == LexicalAnalyzer.rightparen)
                {
                    NextLexeme();
                }
            }
        }

        // Разбор условного оператора.
        private void ParseIfStatement()
        {
            NextLexeme();

            ParseExpression();

            if (_current.Code == LexicalAnalyzer.thensy)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось then");

                RecoverTo(
                    LexicalAnalyzer.thensy,
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.endsy);

                if (_current.Code == LexicalAnalyzer.thensy)
                {
                    NextLexeme();
                }
            }

            ParseStatement();

            if (_current.Code == LexicalAnalyzer.elsesy)
            {
                NextLexeme();
                ParseStatement();
            }
        }

        // Разбор цикла while.
        private void ParseWhileStatement()
        {
            NextLexeme();

            ParseExpression();

            if (_current.Code == LexicalAnalyzer.dosy)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось do");

                RecoverTo(
                    LexicalAnalyzer.dosy,
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.endsy);

                if (_current.Code == LexicalAnalyzer.dosy)
                {
                    NextLexeme();
                }
            }

            ParseStatement();
        }

        // Разбор цикла for.
        private void ParseForStatement()
        {
            NextLexeme();

            if (_current.Code == LexicalAnalyzer.ident)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидался идентификатор");
            }

            if (_current.Code == LexicalAnalyzer.assign)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидался ':='");
            }

            ParseExpression();

            if (_current.Code == LexicalAnalyzer.tosy ||
                _current.Code == LexicalAnalyzer.downtosy)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось to или downto");
            }

            ParseExpression();

            if (_current.Code == LexicalAnalyzer.dosy)
            {
                NextLexeme();
            }
            else
            {
                SyntaxError("ожидалось do");
            }

            ParseStatement();
        }

        // Разбор цикла repeat.
        private void ParseRepeatStatement()
        {
            NextLexeme();

            while (!InputOutput.EndOfFile &&
                   _current.Code != LexicalAnalyzer.untilsy &&
                   _current.Code != LexicalAnalyzer.dot)
            {
                ParseStatement();

                if (_current.Code == LexicalAnalyzer.semicolon)
                {
                    NextLexeme();
                }
                else if (_current.Code != LexicalAnalyzer.untilsy)
                {
                    SyntaxError("ожидалась ';'");

                    RecoverTo(
                        LexicalAnalyzer.semicolon,
                        LexicalAnalyzer.untilsy);

                    if (_current.Code == LexicalAnalyzer.semicolon)
                    {
                        NextLexeme();
                    }
                }
            }

            if (_current.Code == LexicalAnalyzer.untilsy)
            {
                NextLexeme();
                ParseExpression();
            }
            else
            {
                SyntaxError("ожидалось until");
            }
        }

        // Разбор выражения.
        private void ParseExpression()
        {
            ParseSimpleExpression();

            if (_current.Code == LexicalAnalyzer.equal ||
                _current.Code == LexicalAnalyzer.less ||
                _current.Code == LexicalAnalyzer.greater ||
                _current.Code == LexicalAnalyzer.lessequal ||
                _current.Code == LexicalAnalyzer.greaterequal ||
                _current.Code == LexicalAnalyzer.notequal)
            {
                NextLexeme();
                ParseSimpleExpression();
            }
        }

        // Разбор простого выражения.
        private void ParseSimpleExpression()
        {
            if (_current.Code == LexicalAnalyzer.plus ||
                _current.Code == LexicalAnalyzer.minus)
            {
                NextLexeme();
            }

            ParseTerm();

            while (_current.Code == LexicalAnalyzer.plus ||
                   _current.Code == LexicalAnalyzer.minus ||
                   _current.Code == LexicalAnalyzer.orsy)
            {
                NextLexeme();
                ParseTerm();
            }
        }

        // Разбор терма.
        private void ParseTerm()
        {
            ParseFactor();

            while (_current.Code == LexicalAnalyzer.multiply ||
                   _current.Code == LexicalAnalyzer.divide ||
                   _current.Code == LexicalAnalyzer.divsy ||
                   _current.Code == LexicalAnalyzer.modsy ||
                   _current.Code == LexicalAnalyzer.andsy)
            {
                NextLexeme();
                ParseFactor();
            }
        }

        // Разбор множителя.
        private void ParseFactor()
        {
            if (_current.Code == LexicalAnalyzer.ident)
            {
                NextLexeme();

                if (_current.Code == LexicalAnalyzer.leftbracket)
                {
                    ParseIndexes();
                }

                if (_current.Code == LexicalAnalyzer.leftparen)
                {
                    ParseArgumentList();
                }

                return;
            }

            if (_current.Code == LexicalAnalyzer.intconst ||
                _current.Code == LexicalAnalyzer.realconst ||
                _current.Code == LexicalAnalyzer.charconst)
            {
                NextLexeme();
                return;
            }

            if (_current.Code == LexicalAnalyzer.leftparen)
            {
                NextLexeme();

                ParseExpression();

                if (_current.Code == LexicalAnalyzer.rightparen)
                {
                    NextLexeme();
                }
                else
                {
                    SyntaxError("ожидалась ')'");

                    RecoverTo(
                        LexicalAnalyzer.rightparen,
                        LexicalAnalyzer.semicolon,
                        LexicalAnalyzer.endsy);

                    if (_current.Code == LexicalAnalyzer.rightparen)
                    {
                        NextLexeme();
                    }
                }

                return;
            }

            if (_current.Code == LexicalAnalyzer.notsy)
            {
                NextLexeme();
                ParseFactor();
                return;
            }

            SyntaxError("ожидалось выражение");

            RecoverTo(
                LexicalAnalyzer.semicolon,
                LexicalAnalyzer.rightparen,
                LexicalAnalyzer.rightbracket,
                LexicalAnalyzer.endsy);
        }

        // Восстановление после синтаксической ошибки.
        // Анализ продолжается с ближайшего допустимого символа.
        private void RecoverTo(params byte[] codes)
        {
            while (!InputOutput.EndOfFile)
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    if (_current.Code == codes[i])
                    {
                        return;
                    }
                }

                NextLexeme();
            }
        }
    }
}