using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Models;
namespace Core.Interfaces
{
    public interface IRefactoring
    {
        string Name { get; }
        string Description { get; }
        bool CanApply(string code);
        string Apply(string code, RefactoringParameters parameters);
    }
}
