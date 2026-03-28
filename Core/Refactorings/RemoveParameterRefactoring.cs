using Core.interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Refactorings
{
    public class RemoveParameterRefactoring : IRefactoring
    {
        // Назва рефакторингу

        public string Name => "Remove Parameter";


        // Опис рефакторингу

        public string Description => "Removes an unused parameter from a method declaration.";

      
        // Перевіряє чи можна застосувати рефакторинг до коду
 
        public bool CanApply(string code)
        {
            // Логіка буде реалізована пізніше
            throw new NotImplementedException();
        }


        // Виконує рефакторинг видалення параметра

        public string Apply(string code, RefactoringParameters parameters)
        {
            // Логіка буде реалізована пізніше
            throw new NotImplementedException();
        }
    }
}
