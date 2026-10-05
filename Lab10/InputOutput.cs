using System;
using System.Collections.Generic;
using System.IO;

namespace PascalCompiler
{
    class InputOutput
    {
        // Позиция символа
        public struct TextPosition
        {
            public uint LineNumber;
            public byte CharNumber;

            public TextPosition(
                uint lineNumber = 0,
                byte charNumber = 0)
            {
                this.LineNumber = lineNumber;
                this.CharNumber = charNumber;
            }
        }

        // Ошибка
        public struct Err
        {
            public TextPosition ErrorPosition;
            public byte ErrorCode;

            public Err(
                TextPosition errorPosition,
                byte errorCode)
            {
                this.ErrorPosition = errorPosition;
                this.ErrorCode = errorCode;
            }
        }

        private static List<string> _lines =
            new List<string>();

        private static List<Err> _errors =
            new List<Err>();

        private static TextPosition _positionNow =
            new TextPosition(1, 0);

        private static char _ch = '\0';

        private static int _lastInLine = 0;

        private static string _line = "";

        public static void Init(string fileName)
        {
            _lines.Clear();
            _errors.Clear();

            _positionNow =
                new TextPosition(1, 0);

            _ch = '\0';
            _line = "";
            _lastInLine = 0;

            try
            {
                using (StreamReader reader =
                       new StreamReader(fileName))
                {
                    string? currentLine;

                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        _lines.Add(currentLine);
                    }
                }

                if (_lines.Count > 0)
                {
                    ReadNextLine();
                    NextCh();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(
                    "Ошибка 1: " +
                    Errors.GetMessage(1));
            }
            catch (IOException)
            {
                Console.WriteLine(
                    "Ошибка 2: " +
                    Errors.GetMessage(2));
            }
        }

        private static void ReadNextLine()
        {
            int index =
                (int)_positionNow.LineNumber - 1;

            if (index >= 0 &&
                index < _lines.Count)
            {
                _line = _lines[index];
                _lastInLine = _line.Length;
            }
            else
            {
                _line = "";
                _lastInLine = 0;
            }
        }

        public static void NextCh()
        {
            // Если дошли до конца текущей строки
            if (_positionNow.CharNumber >= _lastInLine)
            {
                // Выводим текущую строку
                if (_positionNow.LineNumber <= _lines.Count)
                {
                    ListThisLine();
                }

                // Выводим накопленные ошибки
                if (_errors.Count > 0)
                {
                    ListErrors();
                }

                // Переходим к следующей строке
                _positionNow.LineNumber++;
                _positionNow.CharNumber = 0;

                // Проверяем конец файла
                if (_positionNow.LineNumber > _lines.Count)
                {
                    _ch = '\0';
                    return;
                }

                ReadNextLine();

                // Если строка пустая,
                // сразу переходим к следующей
                if (_lastInLine == 0)
                {
                    NextCh();
                    return;
                }
            }

            // Читаем следующий символ
            _ch = _line[_positionNow.CharNumber];
            _positionNow.CharNumber++;
        }

        public static void Error(
            byte errorCode,
            TextPosition position)
        {
            if (_errors.Count < 10)
            {
                _errors.Add(
                    new Err(position, errorCode));
            }
        }

        private static void ListThisLine()
        {
            if (_positionNow.LineNumber <= _lines.Count)
            {
                Console.WriteLine(
                    _positionNow.LineNumber +
                    ": " +
                    _lines[
                        (int)_positionNow.LineNumber - 1
                    ]);
            }
        }

        private static void ListErrors()
        {
            foreach (Err e in _errors)
            {
                Console.WriteLine(
                    "   ошибка в строке " +
                    e.ErrorPosition.LineNumber +
                    ", символ " +
                    e.ErrorPosition.CharNumber +
                    ", код " +
                    e.ErrorCode +
                    ": " +
                    Errors.GetMessage(e.ErrorCode));
            }

            _errors.Clear();
        }

        public static char CurrentChar
        {
            get
            {
                return _ch;
            }
        }

        public static TextPosition CurrentPosition
        {
            get
            {
                return _positionNow;
            }
        }

        public static bool EndOfFile
        {
            get
            {
                return _ch == '\0' &&
                       _positionNow.LineNumber > _lines.Count;
            }
        }

        public static void Finish()
        {
            if (_errors.Count > 0)
            {
                ListErrors();
            }

            Console.WriteLine();
            Console.WriteLine("Компиляция окончена.");
        }
    }
}