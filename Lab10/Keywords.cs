using System.Collections.Generic;

namespace PascalCompiler
{
    class Keywords
    {
        private Dictionary<string, byte> _keywords;

        public Keywords()
        {
            _keywords =
                new Dictionary<string, byte>();

            // Управление программой.
            _keywords["program"] = LexicalAnalyzer.programsy;
            _keywords["begin"] = LexicalAnalyzer.beginsy;
            _keywords["end"] = LexicalAnalyzer.endsy;

            // Объявления.
            _keywords["const"] = LexicalAnalyzer.constsy;
            _keywords["var"] = LexicalAnalyzer.varsy;
            _keywords["type"] = LexicalAnalyzer.typesy;
            _keywords["array"] = LexicalAnalyzer.arraysy;
            _keywords["record"] = LexicalAnalyzer.recordsy;
            _keywords["set"] = LexicalAnalyzer.setsy;
            _keywords["file"] = LexicalAnalyzer.filesy;
            _keywords["packed"] = LexicalAnalyzer.packedsy;

            // Типы данных.
            _keywords["integer"] = LexicalAnalyzer.integerty;
            _keywords["real"] = LexicalAnalyzer.realsy;
            _keywords["char"] = LexicalAnalyzer.charsy;
            _keywords["boolean"] = LexicalAnalyzer.booleansy;

            // Условия и циклы.
            _keywords["if"] = LexicalAnalyzer.ifsy;
            _keywords["then"] = LexicalAnalyzer.thensy;
            _keywords["else"] = LexicalAnalyzer.elsesy;

            _keywords["for"] = LexicalAnalyzer.forsy;
            _keywords["to"] = LexicalAnalyzer.tosy;
            _keywords["downto"] = LexicalAnalyzer.downtosy;
            _keywords["do"] = LexicalAnalyzer.dosy;

            _keywords["while"] = LexicalAnalyzer.whilesy;
            _keywords["repeat"] = LexicalAnalyzer.repeatsy;
            _keywords["until"] = LexicalAnalyzer.untilsy;

            // Процедуры и функции.
            _keywords["procedure"] =
                LexicalAnalyzer.proceduresy;

            _keywords["function"] =
                LexicalAnalyzer.functionsy;

            // Другие конструкции.
            _keywords["case"] = LexicalAnalyzer.casesy;
            _keywords["of"] = LexicalAnalyzer.ofsy;
            _keywords["with"] = LexicalAnalyzer.withsy;
            _keywords["goto"] = LexicalAnalyzer.gotosy;
            _keywords["label"] = LexicalAnalyzer.labelsy;

            // Логические операции.
            _keywords["and"] = LexicalAnalyzer.andsy;
            _keywords["or"] = LexicalAnalyzer.orsy;
            _keywords["not"] = LexicalAnalyzer.notsy;

            // Арифметические операции.
            _keywords["div"] = LexicalAnalyzer.divsy;
            _keywords["mod"] = LexicalAnalyzer.modsy;

            // Другие ключевые слова.
            _keywords["in"] = LexicalAnalyzer.insy;
            _keywords["nil"] = LexicalAnalyzer.nilsy;
        }

        public byte CheckKeyword(string name)
        {
            name = name.ToLowerInvariant();

            if (_keywords.TryGetValue(name, out byte code))
            {
                return code;
            }

            return LexicalAnalyzer.unknown;
        }
    }
}