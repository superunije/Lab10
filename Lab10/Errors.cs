using System.Collections.Generic;

namespace PascalCompiler
{
    public static class Errors
    {
        private static readonly Dictionary<byte, string> _messages =
            new Dictionary<byte, string>()
            {
                { 1, "Файл не найден" },
                { 2, "Ошибка открытия файла" },
                { 3, "Ошибка чтения файла" },
                { 4, "Недопустимый символ" },
                { 5, "Слишком длинная лексема" },
                { 6, "Целое число выходит за допустимый диапазон" },
                { 7, "Ожидался идентификатор" },
                { 8, "Ожидалась точка с запятой" },
                { 9, "Ожидался символ :=" },
                { 10, "Идентификатор не объявлен" },
                { 11, "Идентификатор уже объявлен" },
                { 12, "Несовместимые типы" }
            };

        public static string GetMessage(byte errorCode)
        {
            if (_messages.ContainsKey(errorCode))
            {
                return _messages[errorCode];
            }

            return "Неизвестная ошибка";
        }
    }
}