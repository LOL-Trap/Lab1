using Core.Models;
using Core.Refactorings;
using Xunit;

namespace Tests
{
    public class RenameVariableRefactoringTests
    {
        [Fact]
        public void Apply_Renames_Variable_Declaration()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = "int x = 5;";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "count";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("int count = 5;", result);
        }

        // 2 Перейменування використання змінної
        [Fact]
        public void Apply_Renames_Variable_Usage()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = "x = x + 1;";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "count";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("count = count + 1;", result);
        }

        // 3 Перейменування змінної у кількох місцях
        [Fact]
        public void Apply_Renames_Variable_InMultiplePlaces()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = @"
            int x = 5;
            x = x + 1;
            return x;";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "count";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            int count = 5;
            count = count + 1;
            return count;";

            Assert.Equal(expected, result);
        }

        // 4 Не перейменовувати частини інших ідентифікаторів
        [Fact]
        public void Apply_DoesNotRename_PartialMatches()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = @"
            int x = 5;
            int xValue = 10;";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "count";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            int count = 5;
            int xValue = 10;";

            Assert.Equal(expected, result);
        }

        // 5 Помилка якщо нова назва є ключовим словом
        [Fact]
        public void Apply_Throws_Exception_When_NewName_IsReservedKeyword()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = "int x = 5;";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "class";

            Assert.Throws<InvalidOperationException>(() => refactoring.Apply(code, parameters));
        }

        // 6 Перейменування змінної всередині блоку
        [Fact]
        public void Apply_Renames_Variable_Inside_Block()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = @"
            {
                int x = 10;
                x++;
            }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "counter";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            {
                int counter = 10;
                counter++;
            }";

            Assert.Equal(expected, result);
        }

        // 7 Перейменування параметра методу
        [Fact]
        public void Apply_Renames_Method_Parameter()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = "int sum(int x) { return x + 1; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "value";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("int sum(int value) { return value + 1; }", result);
        }

        // 8 Перейменування змінної циклу
        [Fact]
        public void Apply_Renames_Loop_Variable()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = "for(int i = 0; i < 10; i++) { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "i";
            parameters.Parameters["newName"] = "index";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("for(int index = 0; index < 10; index++) { }", result);
        }

        // 9 Код не змінюється якщо змінну не знайдено
        [Fact]
        public void Apply_DoesNothing_When_Variable_NotFound()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = "int y = 5;";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "count";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("int y = 5;", result);
        }

        // 10 Помилка якщо нова назва порожня
        [Fact]
        public void Apply_Throws_Exception_When_NewName_IsEmpty()
        {
            var refactoring = new RenameVariableRefactoring();

            string code = "int x = 5;";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "x";
            parameters.Parameters["newName"] = "";

            Assert.Throws<ArgumentException>(() => refactoring.Apply(code, parameters));
        }
    }
}