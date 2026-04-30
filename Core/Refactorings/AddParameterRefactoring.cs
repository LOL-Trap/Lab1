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

        public string Description => "Adds a new parameter to a method declaration and updates all method calls.";

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
            // 1. ОНОВЛЕННЯ СИГНАТУРИ
            // ======================
            string pattern = $@"(?<start>\b\w+\s+{Regex.Escape(methodName)}\s*\()(?<params>[^)]*)(?<end>\))";
            Match match = Regex.Match(code, pattern, RegexOptions.Singleline);

            if (!match.Success)
                return code;

            string oldParams = match.Groups["params"].Value;

            var paramList = oldParams
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            bool alreadyExists = paramList.Any(p =>
            {
                var parts = p.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length >= 2 && parts.Last() == parameterName;
            });

            if (alreadyExists)
                return code;

            string newParameter = $"{parameterType} {parameterName}";

            string newParams = string.IsNullOrWhiteSpace(oldParams)
                ? newParameter
                : oldParams.Trim() + ", " + newParameter;

            // замінюємо тільки список параметрів
            code =
                code.Substring(0, match.Groups["params"].Index) +
                newParams +
                code.Substring(match.Groups["params"].Index + match.Groups["params"].Length);

            // ======================
            // 2. ОНОВЛЕННЯ ВИКЛИКІВ МЕТОДУ
            // ======================
            string callPattern = $@"\b{Regex.Escape(methodName)}\s*\((?<args>[^)]*)\)";

            code = Regex.Replace(code, callPattern, m =>
            {
                // пропускаємо сигнатуру методу
                if (m.Index == match.Index)
                    return m.Value;

                string args = m.Groups["args"].Value.Trim();

                // якщо аргумент вже є — не додаємо
                if (args.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Any(a => a.Trim() == parameterName))
                    return m.Value;

                if (string.IsNullOrWhiteSpace(args))
                    return $"{methodName}({parameterName})";

                return $"{methodName}({args}, {parameterName})";
            });

            // ======================
            // 3. ОНОВЛЕННЯ return
            // ======================
            string returnPattern = @"return\s+(?<expr>[^;]+);";

            code = Regex.Replace(code, returnPattern, m =>
            {
                string expr = m.Groups["expr"].Value.Trim();

                // якщо параметр вже є — нічого не робимо
                if (expr.Split(',').Any(e => e.Trim() == parameterName))
                    return m.Value;

                return $"return {expr}, {parameterName};";
            });

            return code;
        }
    }
}