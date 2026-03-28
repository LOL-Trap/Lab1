using Core.interfaces;
using Core.Models;

namespace Core.Refactorings
{
    public class RenameVariableRefactoring : IRefactoring
    {
        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public string Apply(string code, RefactoringParameters parameters)
        {
            throw new NotImplementedException();
        }

        public bool CanApply(string code)
        {
            throw new NotImplementedException();
        }
    }
}
