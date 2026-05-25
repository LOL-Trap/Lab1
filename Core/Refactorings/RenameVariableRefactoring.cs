using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Interfaces;
using Core.Models;

namespace Core.Refactorings
{
    public class RenameVariableRefactoring : IRefactoring
    {
        public string Name => "Rename Variable";

        public string Description => "Renames a variable in code";

        public string Apply(string code, RefactoringParameters parameters)
        {
            string oldName = parameters.Get<string>("oldName");
            string newName = parameters.Get<string>("newName");

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Нова назва змінної не може бути порожньою.");
            }

            if (IsReservedKeyword(newName))
            {
                throw new InvalidOperationException("Нова назва змінної не може бути ключовим словом.");
            }

            if (string.IsNullOrWhiteSpace(oldName))
            {
                return code;
            }

            return RenameOutsideProtectedZones(code, oldName, newName);
        }

        public bool CanApply(string code)
        {
            return !string.IsNullOrWhiteSpace(code);
        }

        private bool IsReservedKeyword(string value)
        {
            string[] keywords =
            {
                "class", "int", "float", "double", "char", "void", "return",
                "if", "else", "for", "while", "switch", "case", "break",
                "continue", "public", "private", "protected", "static", "new"
            };

            return keywords.Contains(value);
        }

        private string RenameOutsideProtectedZones(string code, string oldName, string newName)
        {
            List<string> protectedParts = new List<string>();
            StringBuilder builder = new StringBuilder();

            int i = 0;

            while (i < code.Length)
            {
                if (StartsWith(code, i, "//"))
                {
                    int start = i;
                    i += 2;

                    while (i < code.Length && code[i] != '\n')
                    {
                        i++;
                    }

                    string part = code.Substring(start, i - start);
                    builder.Append(StoreProtectedPart(protectedParts, part));
                }
                else if (StartsWith(code, i, "/*"))
                {
                    int start = i;
                    i += 2;

                    while (i < code.Length - 1 && !StartsWith(code, i, "*/"))
                    {
                        i++;
                    }

                    if (i < code.Length - 1)
                    {
                        i += 2;
                    }

                    string part = code.Substring(start, i - start);
                    builder.Append(StoreProtectedPart(protectedParts, part));
                }
                else if (code[i] == '"')
                {
                    int start = i;
                    i++;

                    while (i < code.Length)
                    {
                        if (code[i] == '\\' && i + 1 < code.Length)
                        {
                            i += 2;
                            continue;
                        }

                        if (code[i] == '"')
                        {
                            i++;
                            break;
                        }

                        i++;
                    }

                    string part = code.Substring(start, i - start);
                    builder.Append(StoreProtectedPart(protectedParts, part));
                }
                else if (code[i] == '\'')
                {
                    int start = i;
                    i++;

                    while (i < code.Length)
                    {
                        if (code[i] == '\\' && i + 1 < code.Length)
                        {
                            i += 2;
                            continue;
                        }

                        if (code[i] == '\'')
                        {
                            i++;
                            break;
                        }

                        i++;
                    }

                    string part = code.Substring(start, i - start);
                    builder.Append(StoreProtectedPart(protectedParts, part));
                }
                else
                {
                    builder.Append(code[i]);
                    i++;
                }
            }

            string pattern = $@"\b{Regex.Escape(oldName)}\b";
            string processed = Regex.Replace(builder.ToString(), pattern, newName);

            for (int index = 0; index < protectedParts.Count; index++)
            {
                processed = processed.Replace($"__PROTECTED_{index}__", protectedParts[index]);
            }

            return processed;
        }

        private string StoreProtectedPart(List<string> protectedParts, string part)
        {
            string token = $"__PROTECTED_{protectedParts.Count}__";
            protectedParts.Add(part);
            return token;
        }

        private bool StartsWith(string text, int index, string value)
        {
            if (index + value.Length > text.Length)
            {
                return false;
            }

            for (int j = 0; j < value.Length; j++)
            {
                if (text[index + j] != value[j])
                {
                    return false;
                }
            }

            return true;
        }
    }
}