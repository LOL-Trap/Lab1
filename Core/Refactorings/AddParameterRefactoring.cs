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

            // ======================
            // 1. ЗНАХОДИМО МЕТОД
            // ======================
            string pattern = $@"(?<start>\b\w+\s+{Regex.Escape(methodName)}\s*\()(?<params>[^)]*)(?<end>\))";
            Match match = Regex.Match(code, pattern, RegexOptions.Singleline);

            if (!match.Success)
                return code;

            string oldParams = match.Groups["params"].Value;

            // ======================
            // 2. ПЕРЕВІРКА НА ДУБЛЬ
            // ======================
            var existingParams = oldParams
                .Split(',', StringSplitOptions.RemoveEmptyEntries);

            bool alreadyExists = existingParams.Any(p =>
            {
                var parts = p.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length >= 2 && parts.Last() == parameterName;
            });

            if (alreadyExists)
                return code;

            // ======================
            // 3. ФОРМУЄМО НОВИЙ СПИСОК (БЕЗ ЛАМАННЯ ПРОБІЛІВ)
            // ======================
            string newParams;

            if (string.IsNullOrWhiteSpace(oldParams))
            {
                newParams = $"{parameterType} {parameterName}";
            }
            else
            {
                // шукаємо пробіли в кінці
                var trailingSpacesMatch = Regex.Match(oldParams, @"\s*$");
                string trailingSpaces = trailingSpacesMatch.Value;

                // основна частина без кінцевих пробілів
                string coreParams = oldParams.Substring(0, oldParams.Length - trailingSpaces.Length);

                newParams = coreParams + $", {parameterType} {parameterName}" + trailingSpaces;
            }

            // ======================
            // 4. ЗАМІНА СИГНАТУРИ
            // ======================
            code =
                code.Substring(0, match.Groups["params"].Index) +
                newParams +
                code.Substring(match.Groups["params"].Index + match.Groups["params"].Length);

            return code;
        }
    }
}