using Core.Refactorings;
using Core.Models;

namespace Tests
{
    public class RemoveParameterTests
    {
        [Fact]
        // Тест видалення останнього параметра методу
        public void Apply_Removes_Last_Parameter()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int sum(int a, int b) { return a + b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterName"] = "b";

            string expected = "int sum(int a) { return a + b; }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест видалення першого параметра з трьох
        public void Apply_Removes_First_Parameter()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int sum(int a, int b, int c) { return a + b + c; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterName"] = "a";

            string expected = "int sum(int b, int c) { return a + b + c; }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест видалення середнього параметра з трьох
        public void Apply_Removes_Middle_Parameter()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int multiply(int x, int y, int z) { return x * y * z; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "multiply";
            parameters.Parameters["parameterName"] = "y";

            string expected = "int multiply(int x, int z) { return x * y * z; }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест видалення єдиного параметра
        public void Apply_Removes_Only_Parameter()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int square(int x) { return x * x; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "square";
            parameters.Parameters["parameterName"] = "x";

            string expected = "int square() { return x * x; }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест видалення параметра з void-методу
        public void Apply_Removes_Parameter_From_Void_Method()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "void print(int a, int b) { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "print";
            parameters.Parameters["parameterName"] = "b";

            string expected = "void print(int a) { }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест видалення другого параметра з трьох
        public void Apply_Method_With_Three_Parameters()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int calc(int a, int b, int c) { return a + b + c; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "calc";
            parameters.Parameters["parameterName"] = "b";

            string expected = "int calc(int a, int c) { return a + b + c; }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест видалення параметра з пробілами навколо
        public void Apply_Method_With_Spaces_In_Parameters()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int sum( int a , int b ) { return a + b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterName"] = "b";

            string expected = "int sum( int a ) { return a + b; }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест видалення параметра у void-методі з багаторядковим тілом
        public void Apply_RemoveParameter_In_VoidMethodWithBody()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode =
            @"void printSum(int a, int b)
            {
                int sum = a + b;
                Console.WriteLine(sum);
            }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "printSum";
            parameters.Parameters["parameterName"] = "b";

            string expected =
            @"void printSum(int a)
            {
                int sum = a + b;
                Console.WriteLine(sum);
            }";

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест, коли параметр не знайдено — код лишається без змін
        public void Apply_Parameter_Not_Found()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int sum(int a, int b) { return a + b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterName"] = "c";

            string expected = inputCode;

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        // Тест, коли метод не знайдено — код лишається без змін
        public void Apply_Method_Not_Found()
        {
            // Arrange
            var refactoring = new RemoveParameterRefactoring();
            string inputCode = "int sum(int a, int b) { return a + b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "calculate";
            parameters.Parameters["parameterName"] = "b";

            string expected = inputCode;

            // Act
            string result = refactoring.Apply(inputCode, parameters);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
