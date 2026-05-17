using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core.Refactorings
{
    public class RemoveParameterRefactoring : IRefactoring
    {
        public string Name => "Remove Parameter";
        public string Description => "Removes an unused parameter from a method declaration.";

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

            string pattern =
                $@"(?:void|int|string|double|float|bool|char|long|short|byte|decimal|[\w<>]+\s*\*?)\s+{Regex.Escape(methodName)}\s*\(([^)]*)\)";

            var match = Regex.Match(code, pattern, RegexOptions.Singleline);

            if (!match.Success)
                return code;

            string paramList = match.Groups[1].Value;

            if (string.IsNullOrWhiteSpace(paramList))
                return code;

            var parametersList = paramList.Split(',');
            List<string> newParams = new List<string>();

            foreach (var param in parametersList)
            {
                string trimmed = param.Trim();

                string withoutDefault = trimmed.Split('=')[0].Trim();

                string[] parts = withoutDefault.Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    continue;

                string name = parts[^1].TrimStart('*', '&');

                if (name != parameterName)
                    newParams.Add(param);
            }

            if (newParams.Count == parametersList.Length)
                return code;

            bool removedFirstParameter =
                parametersList.Length > 0 &&
                parametersList[0].Contains(parameterName);

            string newParamList;

            if (removedFirstParameter && newParams.Count > 0)
            {
                bool firstWasPointer = parametersList[0].Contains("*");

                if (firstWasPointer)
                    newParamList = string.Join(",", newParams).TrimStart();
                else
                    newParamList = " " + string.Join(",", newParams).TrimStart();
            }
            else
            {
                newParamList = string.Join(",", newParams);
            }

            string result =
                code.Substring(0, match.Groups[1].Index) +
                newParamList +
                code.Substring(match.Groups[1].Index + match.Groups[1].Length);

            return result;
        }
    }
}
