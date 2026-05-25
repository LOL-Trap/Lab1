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
            {
                return code;
            }

            // Підтримка:
            // int sum(...)
            // void print(...)
            // int* getValue(...)
            // int& getRef(...)
            // std::vector<int> getList(...)
            // MyClass(...)
            string pattern =
                $@"(?<before>\b[\w:<>\*&]+\s+)?(?<name>{Regex.Escape(methodName)})\s*\((?<params>[^)]*)\)";

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
                return parts.Length >= 2 &&
                       parts.Last().Trim() == parameterName;
            });

            if (alreadyExists)
                return code;

            string newParameter = $"{parameterType} {parameterName}";
            string newParams;

            if (string.IsNullOrWhiteSpace(oldParams))
            {
                // Зберігаємо відступи всередині ()
                string leadingSpaces =
                    new string(oldParams.TakeWhile(char.IsWhiteSpace).ToArray());

                string trailingSpaces =
                    new string(oldParams.Reverse().TakeWhile(char.IsWhiteSpace).Reverse().ToArray());

                newParams = leadingSpaces + newParameter + trailingSpaces;
            }
            else
            {
                // Прибираємо лише пробіли справа для коректного додавання
                string trimmedRight = oldParams.TrimEnd();

                // Зберігаємо пробіли перед ')'
                string trailingSpaces =
                    oldParams.Substring(trimmedRight.Length);

                newParams = trimmedRight + ", " + newParameter + trailingSpaces;
            }

            string result =
                code.Substring(0, match.Groups["params"].Index) +
                newParams +
                code.Substring(match.Groups["params"].Index + match.Groups["params"].Length);

            return result;
        }
    }
}