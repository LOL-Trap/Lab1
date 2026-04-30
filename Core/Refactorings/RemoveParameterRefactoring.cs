using Core.Interfaces;
using Core.Models;
using System.Text.RegularExpressions;
using System.Linq;

namespace Core.Refactorings
{
    public class RemoveParameterRefactoring : IRefactoring
    {
        public string Name => "Remove Parameter";
        public string Description => "Removes a parameter from a method declaration and its calls.";

        public bool CanApply(string code)
        {
            return true;
        }

        public string Apply(string code, RefactoringParameters parameters)
        {
            string methodName = parameters.Get<string>("methodName");
            string parameterName = parameters.Get<string>("parameterName");

            if (string.IsNullOrEmpty(methodName) || string.IsNullOrEmpty(parameterName))
                return code;

            //  1. СИГНАТУРА
            string methodPattern = $@"(\b{Regex.Escape(methodName)}\s*\()([^)]*)(\))";
            var match = Regex.Match(code, methodPattern);

            if (!match.Success)
                return code;

            var paramList = match.Groups[2].Value
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .ToList();

            int indexToRemove = -1;

            for (int i = 0; i < paramList.Count; i++)
            {
                var parts = paramList[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts[^1] == parameterName)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove == -1)
                return code;

            paramList.RemoveAt(indexToRemove);

            string newParams = string.Join(", ", paramList);

            string result =
                code.Substring(0, match.Groups[2].Index) +
                newParams +
                code.Substring(match.Groups[2].Index + match.Groups[2].Length);

            //  2. ВИКЛИКИ (ВАЖЛИВО: rebuild повністю)

            string callPattern = $@"\b{Regex.Escape(methodName)}\s*\(([^)]*)\)";
            var callMatches = Regex.Matches(result, callPattern).ToList();

            //  працюємо З КІНЦЯ
            for (int i = callMatches.Count - 1; i >= 0; i--)
            {
                var call = callMatches[i];

                var args = call.Groups[1].Value
                    .Split(',')
                    .Select(a => a.Trim())
                    .Where(a => a.Length > 0)
                    .ToList();

                if (indexToRemove < args.Count)
                    args.RemoveAt(indexToRemove);

                string newArgs = string.Join(", ", args);

                result =
                    result.Substring(0, call.Groups[1].Index) +
                    newArgs +
                    result.Substring(call.Groups[1].Index + call.Groups[1].Length);
            }

            return result;
        }
    }
}