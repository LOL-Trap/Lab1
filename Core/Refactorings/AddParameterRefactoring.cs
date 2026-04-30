using Core.Interfaces;
using Core.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Core.Refactorings
{
    public class AddParameterRefactoring : IRefactoring
    {
        public string Name => "Add Parameter";

        public string Description => "Adds a new parameter to a method declaration and updates return.";

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
            // 2. НОРМАЛІЗАЦІЯ ПАРАМЕТРІВ
            // ======================
            var paramList = oldParams
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => Regex.Replace(p, @"\s+", " ").Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            // перевірка на дубль
            bool alreadyExists = paramList.Any(p =>
            {
                var parts = p.Split(' ');
                return parts.Length >= 2 && parts.Last() == parameterName;
            });

            if (!alreadyExists)
            {
                paramList.Add($"{parameterType} {parameterName}");
            }

            string newParams = string.Join(", ", paramList);

            // ======================
            // 3. ЗАМІНА СИГНАТУРИ
            // ======================
            code =
                code.Substring(0, match.Groups["params"].Index) +
                newParams +
                code.Substring(match.Groups["params"].Index + match.Groups["params"].Length);

            // ======================
            // 4. ОНОВЛЕННЯ return
            // ======================
            string returnPattern = @"return\s+(?<expr>[^;]+);";

            code = Regex.Replace(code, returnPattern, m =>
            {
                string expr = m.Groups["expr"].Value.Trim();

                // якщо вже є b — не дублюємо
                if (expr.Split(',').Any(e => e.Trim() == parameterName))
                    return m.Value;

                return $"return {expr}, {parameterName};";
            });

            return code;
        }
    }
}