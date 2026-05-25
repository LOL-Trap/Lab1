using Core.Models;
using Core.Refactorings;

namespace Tests
{
    public class RenameMethodRefactoringTests
    {
        //1 Перейменування оголошення методу
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

        //2 Перейменування виклику методу
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

        //3 Перейменування у кількох місцях
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

        //4 Не перейменовувати частини інших слів
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

        //5 Помилка якщо нова назва — ключове слово
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

        //6 Перейменування методу всередині іншого методу
        [Fact]
        public void Apply_Renames_Method_Call_Inside_Other_Method()
        {
            var refactoring = new RenameMethodRefactoring();

            string code = @"
            void calc() { }
            void test() 
            {
                calc();
            }";

            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "calc";
            parameters.Parameters["newName"] = "calculate";

            string result = refactoring.Apply(code, parameters);

            string expected = @"
            void calculate() { }
            void test() 
            {
                calculate();
            }";

            Assert.Equal(expected, result);
        }

        //7 Метод з параметрами
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

        //8 Статичний метод
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

        //9 Метод у класі
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

        //10 Перейменування методу і його викликів
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

        [Fact]
        public void Apply_DoesNotRename_Variable_With_Same_Name_As_Method()
        {
            var refactoring = new RenameMethodRefactoring();
            string code = "int foo = 10;\nvoid foo() {}";
            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "foo";
            parameters.Parameters["newName"] = "bar";

            string result = refactoring.Apply(code, parameters);

            string expected = "int foo = 10;\nvoid bar() {}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_DoesNotRename_Inside_StringLiteral()
        {
            var refactoring = new RenameMethodRefactoring();
            string code = "void log() { printf(\"foo called\\n\"); }\nvoid foo() {}";
            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "foo";
            parameters.Parameters["newName"] = "bar";

            string result = refactoring.Apply(code, parameters);

            string expected = "void log() { printf(\"foo called\\n\"); }\nvoid bar() {}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_DoesNotRename_Inside_Comment()
        {
            var refactoring = new RenameMethodRefactoring();
            string code = "// foo is a deprecated method\nvoid foo() {}";
            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "foo";
            parameters.Parameters["newName"] = "bar";

            string result = refactoring.Apply(code, parameters);

            string expected = "// foo is a deprecated method\nvoid bar() {}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Apply_DoesNotRename_When_NameIsPart_Of_AnotherIdentifier()
        {
            var refactoring = new RenameMethodRefactoring();
            string code = "void foo() {}\nvoid foo_bar() {}\nvoid my_foo() {}";
            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = "foo";
            parameters.Parameters["newName"] = "renamed";

            string result = refactoring.Apply(code, parameters);

            string expected = "void renamed() {}\nvoid foo_bar() {}\nvoid my_foo() {}";
            Assert.Equal(expected, result);
        }
    }
}