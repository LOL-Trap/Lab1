using System.Collections.Generic;

namespace Core.Models
{
    public class RefactoringParameters
    {
        public Dictionary<string, string> Parameters { get; set; }

        public RefactoringParameters()
        {
            Parameters = new Dictionary<string, string>();
        }
    }
}