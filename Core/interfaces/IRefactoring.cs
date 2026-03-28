using Core.Models;

namespace Core.interfaces
{
    public interface IRefactoring
    {
        string Name { get; }
        string Description { get; }
        bool CanApply(string code);
        string Apply(string code, RefactoringParameters parameters);
    }
}
