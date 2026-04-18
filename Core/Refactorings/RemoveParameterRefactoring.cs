using Core.Interfaces;
using Core.Models;
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

            // Знаходимо дужки методу
            string pattern = $@"\b{Regex.Escape(methodName)}\s*\(([^)]*)\)";
            var match = Regex.Match(code, pattern, RegexOptions.Singleline);
            if (!match.Success)
                return code;

            string paramList = match.Groups[1].Value;
            int paramStartIndex = match.Groups[1].Index;

            if (string.IsNullOrWhiteSpace(paramList))
                return code;

            int currentIndex = 0;
            bool removed = false;

            while (currentIndex < paramList.Length)
            {
                int commaIndex = paramList.IndexOf(',', currentIndex);
                int endIndex = commaIndex >= 0 ? commaIndex : paramList.Length;

                string paramSubstring = paramList.Substring(currentIndex, endIndex - currentIndex);
                string trimmed = paramSubstring.Trim();
                string[] parts = trimmed.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                string name = parts.Length > 0 ? parts[^1] : "";

                if (!removed && name == parameterName)
                {
                    removed = true;

                    // Якщо перший параметр і після '(' є пробіл, видаляємо його також
                    bool isFirst = currentIndex == 0;
                    int removeLength = endIndex - currentIndex;

                    if (commaIndex >= 0)
                    {
                        // Якщо не останній параметр, видаляємо кома після нього
                        removeLength += 1;
                    }
                    else
                    {
                        // Якщо останній параметр і перед ним кома, видаляємо її
                        if (currentIndex > 0 && paramList[currentIndex - 1] == ',')
                        {
                            currentIndex -= 1;
                            removeLength += 1;
                        }
                    }

                    // Для першого параметра видаляємо пробіл після '('
                    if (isFirst && currentIndex > 0 && paramList[currentIndex - 1] == ' ')
                    {
                        currentIndex -= 1;
                        removeLength += 1;
                    }

                    paramList = paramList.Remove(currentIndex, removeLength);
                    break;
                }

                currentIndex = endIndex + 1;
            }

            string newCode = code.Substring(0, paramStartIndex) +
                             paramList +
                             code.Substring(paramStartIndex + match.Groups[1].Length);

            return newCode;
        }
    }
}