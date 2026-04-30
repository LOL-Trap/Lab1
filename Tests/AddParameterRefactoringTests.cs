using Core.Refactorings;
using Core.Models;

namespace Tests
{
    public class AddParameterRefactoringTests
    {
        [Fact]
        public void Apply_Adds_Parameter_To_End()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "int sum(int a) { return a; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "b";

            string expected = "int sum(int a, int b) { return a; }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Adds_Parameter_To_Empty_List()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "int sum() { return 0; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "a";

            string expected = "int sum(int a) { return 0; }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Adds_Parameter_To_Void_Method()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "void print(int a) { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "print";
            parameters.Parameters["parameterType"] = "string";
            parameters.Parameters["parameterName"] = "text";

            string expected = "void print(int a, string text) { }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Adds_Parameter_With_Spaces()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "int sum( int a ) { return a; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "b";

            string expected = "int sum( int a, int b ) { return a; }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Adds_Parameter_To_Method_With_Multiple_Params()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "int calc(int a, int b) { return a + b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "calc";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "c";

            string expected = "int calc(int a, int b, int c) { return a + b; }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_AddParameter_In_Multiline_Method()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode =
            @"void printSum(int a)
            {
                Console.WriteLine(a);
            }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "printSum";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "b";

            string expected =
            @"void printSum(int a, int b)
            {
                Console.WriteLine(a);
            }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Method_Not_Found()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "int sum(int a) { return a; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "calculate";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "b";

            string expected = inputCode;

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Parameter_Already_Exists()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "int sum(int a, int b) { return a + b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "sum";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "b";

            string expected = inputCode;

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Adds_String_Parameter()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "void log() { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "log";
            parameters.Parameters["parameterType"] = "string";
            parameters.Parameters["parameterName"] = "message";

            string expected = "void log(string message) { }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_Adds_Parameter_To_Method_With_Extra_Spaces()
        {
            var refactoring = new AddParameterRefactoring();
            string inputCode = "int test(  int a,   int b ) { return a + b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["methodName"] = "test";
            parameters.Parameters["parameterType"] = "int";
            parameters.Parameters["parameterName"] = "c";

            string expected = "int test(  int a,   int b, int c ) { return a + b; }";

            string result = refactoring.Apply(inputCode, parameters);

            Assert.Equal(expected, result);
        }
    }
}
