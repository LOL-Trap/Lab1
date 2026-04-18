using System;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Interfaces;
using Core.Models;

namespace Core.Refactorings
{
    public class RenameVariableRefactoring : IRefactoring
    {
        public string Name => "Rename Variable";

        public string Description => "Renames a variable in code";

        public string Apply(string code, RefactoringParameters parameters)
        {
            string oldName = parameters.Get<string>("oldName");
            string newName = parameters.Get<string>("newName");

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Нова назва змінної не може бути порожньою.");
            }

            if (IsReservedKeyword(newName))
            {
                throw new InvalidOperationException("Нова назва змінної не може бути ключовим словом.");
            }

            if (string.IsNullOrWhiteSpace(oldName))
            {
                return code;
            }

            string pattern = $@"\b{Regex.Escape(oldName)}\b";
            return Regex.Replace(code, pattern, newName);
        }

        public bool CanApply(string code)
        {
            return !string.IsNullOrWhiteSpace(code);
        }

        private bool IsReservedKeyword(string value)
        {
            string[] keywords =
            {
                "class", "int", "float", "double", "char", "void", "return",
                "if", "else", "for", "while", "switch", "case", "break",
                "continue", "public", "private", "protected", "static", "new"
            };

            return keywords.Contains(value);
        }
    }
}