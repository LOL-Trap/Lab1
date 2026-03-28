using Core.Models;

namespace Core.Interfaces
{
    public interface IRefactoring
    {
        string Apply(string code, RefactoringParameters parameters);
    }
}