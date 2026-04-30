using Core.Interfaces;
using Core.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Refactorings
{
    public class AddParameterRefactoring : IRefactoring
    {
        public string Name => "Add Parameter";

        public string Description => "Adds a new parameter to a method declaration.";

        public bool CanApply(string code)
        {
            return !string.IsNullOrWhiteSpace(code);
        }

        public string Apply(string code, RefactoringParameters parameters)
        {
            string methodName = parameters.Get<string>("methodName");
            string parameterType = parameters.Get<string>("parameterType");
            string parameterName = parameters.Get<string>("parameterName");

            if (string.IsNullOrWhiteSpace(methodName) ||
                string.IsNullOrWhiteSpace(parameterType) ||
                string.IsNullOrWhiteSpace(parameterName))
                return code;

            // шукаємо метод
            string pattern = $@"(?<start>\b\w+\s+{Regex.Escape(methodName)}\s*\()(?<params>[^)]*)(?<end>\))";
            Match match = Regex.Match(code, pattern, RegexOptions.Singleline);

            if (!match.Success)
                return code;

            string oldParams = match.Groups["params"].Value;

            // перевірка чи параметр вже існує
            var paramList = oldParams
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim());

            bool alreadyExists = paramList.Any(p =>
            {
                var parts = p.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length >= 2 && parts.Last() == parameterName;
            });

            if (alreadyExists)
                return code;

            string newParameter = $"{parameterType} {parameterName}";
            string newParams;

            if (string.IsNullOrWhiteSpace(oldParams))
            {
                newParams = newParameter;
            }
            else
            {
                // 🔑 ЗБЕРІГАЄМО ФОРМАТУВАННЯ
                newParams = oldParams + ", " + newParameter;
            }

            // замінюємо тільки параметри
            string result =
                code.Substring(0, match.Groups["params"].Index) +
                newParams +
                code.Substring(match.Groups["params"].Index + match.Groups["params"].Length);

            return result;
        }
    }
}