using Core.interfaces;
using Core.Models;

namespace Core.Refactorings
{
    public class RenameMethodRefactoring : IRefactoring
    {
        public string Name => "Rename Method";

        public string Description => "Renames a method and all its calls";

        public bool CanApply(string code)
        {
            // Заглушка
            return true;
        }

        public string Apply(string code, RefactoringParameters parameters)
        {
            string oldName = parameters.Get<string>("oldName");
            string newName = parameters.Get<string>("newName");

            // Реалізація поки відсутня
            return code;
        }
    }
}