using Core.Models;
using Core.Refactorings;

namespace Tests
{
    public class RenameVariableRefactoringTests
    {
        [Fact]
        public void Apply_Renames_SimpleVariable_Declaration()
        {
            // Arrange
            var refactoring = new RenameVariableRefactoring();
            string inputCode = "int x = 5;";
            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "myNumber";

            // Act
            string result = refactoring.Apply(inputCode, parameters);
            // Assert

            Assert.Equal("int myNumber = 5;", result);
        }
    }
}
