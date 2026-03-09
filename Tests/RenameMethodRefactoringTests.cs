using Core.Models;
using Core.Refactorings;

namespace Tests
{
    public class RenameMethodRefactoringTests
    {
        //1
        [Fact]
        public void Apply_Renames_Method_Declaration()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = "void calc() { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("void calculate() { }", result);
        }

        //2
        [Fact]
        public void Apply_Renames_Method_Call()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = "calc();";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("calculate();", result);
        }

        //3
        [Fact]
        public void Apply_Renames_Method_InMultiplePlaces()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = @"
            void calc() { }
            calc();
            calc();";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            void calculate() { }
            calculate();
            calculate();";

            Assert.Equal(expected, result);
        }

        //4
        [Fact]
        public void Apply_DoesNotRename_PartialMatches()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = @"
            void calc() { } 
            void calculator() { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            void calculate() { }
            void calculator() { }";

            Assert.Equal(expected, result);
        }

        //5
        [Fact]
        public void Apply_Throws_Exception_When_NewName_IsReservedKeyword()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = "void calc() { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "class";

            Assert.Throws<InvalidOperationException>(() => refactoring.Apply(code, parameters));
        }

        //6
        [Fact]
        public void Apply_Renames_Method_Call_Inside_Other_Method()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = @"
            void calc() {}
            void test() 
            {
                calc();
            }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            void calculate() {}
            void test() 
            {
                calculate();
            }";

            Assert.Equal(expected, result);
        }

        //7
        [Fact]
        public void Apply_Renames_Method_With_Parameters()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = "int calc(int a, int b) { return a+b; }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "sum";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("int sum(int a, int b) { return a+b; }", result);
        }

        //8
        [Fact]
        public void Apply_Renames_Static_Method()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = "static void calc() { }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            Assert.Equal("static void calculate() { }", result);
        }

        //9
        [Fact]
        public void Apply_Renames_Method_In_Class()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = @"
            class Test 
            {
             void calc() { }
            }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            class Test 
            {
             void calculate() { }
            }";

            Assert.Equal(expected, result);
        }

        //10
        [Fact]
        public void Apply_Renames_Method_And_All_Calls()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = @"
            void calc() { }
            calc();";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            void calculate() { }
            calculate();";

            Assert.Equal(expected, result);
        }
    }
}