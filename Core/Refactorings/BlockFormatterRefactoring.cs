using System.Text.RegularExpressions;
using Core.Interfaces;
using Core.Models;

namespace Core.Refactorings
{
    public class BlockFormatterRefactoring : IRefactoring
    {
        public string Name => "Block Formatter";

        public string Description => "Formats code blocks using Allman or K&R brace style.";

        public bool CanApply(string code)
        {
            return !string.IsNullOrWhiteSpace(code) && IsBalanced(code);
        }

        public string Apply(string code, RefactoringParameters parameters)
        {
            string styleText = parameters.Get<string>("braceStyle");
            int indentSize = parameters.Get<int>("indentSize");

            if (indentSize <= 0)
            {
                indentSize = 4;
            }

            BraceStyle style = ParseStyle(styleText);

            if (!IsBalanced(code))
            {
                throw new ArgumentException("Malformed code: unbalanced brackets");
            }

            code = NormalizeIndentation(code, indentSize);
            code = ExpandBraces(code);
            code = ReIndent(code, indentSize);
            code = AddKeywordSpaces(code);
            code = NormalizeBraces(code, style);
            code = code.TrimEnd('\n', '\r');

            return code;
        }

        public string NormalizeIndentation(string code, int indentSize)
        {
            return code.Replace("\t", new string(' ', indentSize));
        }

        public string NormalizeBraces(string code, BraceStyle style)
        {
            if (style == BraceStyle.Allman)
            {
                code = Regex.Replace(code, @"([^\n{])\s*\{", "$1\n{");
                return code;
            }
            else
            {
                var lines = code.Split('\n').ToList();
                var result = new List<string>();

                foreach (var line in lines)
                {
                    if (line.Trim() == "{" && result.Count > 0)
                    {
                        result[result.Count - 1] = result[result.Count - 1].TrimEnd() + " {";
                    }
                    else
                    {
                        result.Add(line);
                    }
                }

                return string.Join("\n", result);
            }
        }

        private BraceStyle ParseStyle(string? styleText)
        {
            if (string.Equals(styleText, "Allman", StringComparison.OrdinalIgnoreCase))
            {
                return BraceStyle.Allman;
            }

            return BraceStyle.KAndR;
        }

        private bool IsBalanced(string code)
        {
            int curly = 0;
            int paren = 0;

            foreach (char c in code)
            {
                if (c == '{')
                {
                    curly++;
                }
                else if (c == '}')
                {
                    curly--;
                }
                else if (c == '(')
                {
                    paren++;
                }
                else if (c == ')')
                {
                    paren--;
                }

                if (curly < 0 || paren < 0)
                {
                    return false;
                }
            }

            return curly == 0 && paren == 0;
        }

        private string ExpandBraces(string code)
        {
            code = Regex.Replace(code, @"\s*\{", "\n{");
            code = Regex.Replace(code, @"\{([^\n])", "{\n$1");
            code = Regex.Replace(code, @"([^\n])\}", "$1\n}");
            code = Regex.Replace(code, @"\}([^\n])", "}\n$1");
            return code;
        }

        private string ReIndent(string code, int indentSize)
        {
            var lines = code.Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList();

            int depth = 0;
            var result = new List<string>();

            foreach (var line in lines)
            {
                if (line == "}")
                {
                    depth = Math.Max(0, depth - 1);
                    result.Add(new string(' ', depth * indentSize) + "}");

                    if (depth == 0)
                    {
                        result.Add(string.Empty);
                    }
                }
                else if (line == "{")
                {
                    result.Add(new string(' ', depth * indentSize) + "{");
                    depth++;
                }
                else
                {
                    result.Add(new string(' ', depth * indentSize) + line);
                }
            }

            return string.Join("\n", result);
        }

        private string AddKeywordSpaces(string code)
        {
            return Regex.Replace(code, @"\b(if|for|while|switch)\s*\(", "$1 (");
        }
    }
}