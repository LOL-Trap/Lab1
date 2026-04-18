using Core.Interfaces;
using Core.Models;
using System.Text.RegularExpressions;

namespace Core.Refactorings
{
    public class RenameMethodRefactoring : IRefactoring
    {
        public string Name => "Rename Method";

        public string Description => "Renames a method and all its calls";

        public bool CanApply(string code)
        {
            // Заглушка
            return true;
        }

        public string Apply(string code, RefactoringParameters parameters)
        {
            string oldName = parameters.Get<string>("oldName");
            string newName = parameters.Get<string>("newName");

            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
                return code;

            // Список ключових слів 
            var reservedKeywords = new HashSet<string>
            {
                "class", "void", "int", "string", "public", "private",
                "protected", "static", "return", "if", "else", "for",
                "while", "switch", "case", "break", "continue", "new"
            };

            // 5 тест — перевірка на ключове слово
            if (reservedKeywords.Contains(newName))
                throw new InvalidOperationException("New name is a reserved keyword");

            // Тільки повне співпадіння слова
            string pattern = $@"\b{oldName}\b";

            string result = Regex.Replace(code, pattern, newName);

            return result;
        }
    }
}