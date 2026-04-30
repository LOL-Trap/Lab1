using Core.Models;
using Core.Refactorings;

namespace Tests
{
    public class BlockFormatterTests
    {
        [Fact]
        public void Apply_AllmanStyle_OpeningBraceOnNewLine()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "void foo() {\n    int x = 1;\n}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "Allman";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Contains("void foo()\n{", result);
        }

        [Fact]
        public void Apply_KAndRStyle_OpeningBraceOnSameLine()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "void foo()\n{\n    int x = 1;\n}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "KAndR";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Contains("void foo() {", result);
        }

        [Fact]
        public void Apply_InconsistentIndentation_NormalizedTo4Spaces()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "void foo() {\n   int x = 1;\n}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "KAndR";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Contains("    int x = 1;", result);
        }

        [Fact]
        public void Apply_EmptyBlock_FormatsCorrectly()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "void foo(){}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "Allman";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Contains("void foo()\n{\n}", result);
        }

        [Fact]
        public void Apply_IfElseBlock_AllmanFormatted()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "if(x > 0){y=1;}else{y=-1;}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "Allman";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Contains("if (x > 0)\n{", result);
        }

        [Fact]
        public void Apply_NestedBlocks_DoubleIndented()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "void foo() {\nfor(int i=0;i<10;i++){\nint x=i;\n}\n}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "KAndR";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Contains("        int x=i;", result);
        }

        [Fact]
        public void Apply_AlreadyFormatted_ReturnsUnchanged()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "void foo() {\n    int x = 1;\n}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "KAndR";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Equal(code, result);
        }

        [Fact]
        public void Apply_PreservesInnerCode()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "int add(int a,int b){return a+b;}";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "Allman";
            parameters.Parameters["indentSize"] = 4;

            string result = refactoring.Apply(code, parameters);

            Assert.Contains("return a+b;", result);
        }

        [Fact]
        public void NormalizeIndentation_TabToSpaces_Converted()
        {
            var refactoring = new BlockFormatterRefactoring();

            string result = refactoring.NormalizeIndentation("\tint x = 1;", 4);

            Assert.Equal("    int x = 1;", result);
        }

        [Fact]
        public void Apply_MalformedCode_ThrowsException()
        {
            var refactoring = new BlockFormatterRefactoring();
            string code = "void foo(";

            var parameters = new RefactoringParameters();
            parameters.Parameters["braceStyle"] = "KAndR";
            parameters.Parameters["indentSize"] = 4;

            Assert.Throws<ArgumentException>(() => refactoring.Apply(code, parameters));
        }
    }
}