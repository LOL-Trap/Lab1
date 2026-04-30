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

        public bool CanApply(string code) => true;

        public string Apply(string code, RefactoringParameters parameters)
        {
            string methodName = parameters.Get<string>("methodName");
            string parameterName = parameters.Get<string>("parameterName");

            if (string.IsNullOrWhiteSpace(methodName) ||
                string.IsNullOrWhiteSpace(parameterName))
                return code;

            // =========================
            // 1. SIGNATURE (як у тебе)
            // =========================
            string pattern =
                $@"(?<start>\b\w+\s+{Regex.Escape(methodName)}\s*\()(?<params>[^)]*)(?<end>\))";

            var match = Regex.Match(code, pattern, RegexOptions.Singleline);

            if (!match.Success)
                return code;

            string oldParams = match.Groups["params"].Value;

            var paramList = oldParams
                .Split(',')
                .Select(p => Regex.Replace(p, @"\s+", " ").Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            int indexToRemove = -1;

            for (int i = 0; i < paramList.Count; i++)
            {
                var parts = paramList[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0 && parts[^1] == parameterName)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove == -1)
                return code;

            paramList.RemoveAt(indexToRemove);

            string newParams = string.Join(", ", paramList);

            // =========================
            // 2. REPLACE SIGNATURE (як у тебе)
            // =========================
            code =
                code.Substring(0, match.Groups["params"].Index) +
                newParams +
                code.Substring(match.Groups["params"].Index + match.Groups["params"].Length);

            // =========================
            // 3. CALLS (REWRITE STYLE, NOT INDEX STYLE)
            // =========================
            string callPattern = $@"\b{Regex.Escape(methodName)}\s*\(([^)]*)\)";

            code = Regex.Replace(code, callPattern, m =>
            {
                var args = m.Groups[1].Value
                    .Split(',')
                    .Select(a => Regex.Replace(a, @"\s+", " ").Trim())
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .ToList();

                var paramsInCall = args.ToList();

                // 🔥 знайти позицію через "логіку як у сигнатурі"
                var sigParams = match.Groups["params"].Value
                    .Split(',')
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .ToList();

                int idx = -1;

                for (int i = 0; i < sigParams.Count; i++)
                {
                    var parts = sigParams[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts[^1] == parameterName)
                    {
                        idx = i;
                        break;
                    }
                }

                if (idx >= 0 && idx < paramsInCall.Count)
                    paramsInCall.RemoveAt(idx);

                return $"{methodName}({string.Join(", ", paramsInCall)})";
            });

            return code;
        }
    }
}