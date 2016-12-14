//-----------------------------------------------------------------------
// <copyright file="CSharpTests.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the C# language transforms.
    /// </summary>
    [TestClass]
    [TestTransform("csharp")]
    [DeploymentItem("Input", "Input")]
    [DeploymentItem("Expected", "Expected")]
    [DeploymentItem("Template", "Template")]
    [DeploymentItem("Transforms", "Transforms")]
    public class CSharpTests : SharedTests
    {
        /// <summary>
        /// The pattern used to test only number literal tokens.
        /// </summary>
        private static readonly string NumberPattern = @"\W?\d+\W?";

        /// <summary>
        /// Verifies that single line comments are decorated with "pl-c".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestSingleLineComments()
        {
            this.AssertTransformed("SingleLineComments", "pl-c");
        }

        /// <summary>
        /// Verifies that multi-line comments are decorated with "pl-c".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestMultilineComments()
        {
            this.AssertTransformed("MultilineComments", "pl-c");
        }

        /// <summary>
        /// Verifies that XML tags in comments are decorated with "pl-ent".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestCommentTags()
        {
            this.AssertTransformedInside("CommentTags", "pl-ent", "pl-c");
        }

        /// <summary>
        /// Verifies that XML attributes in comments are decorated with "pl-e".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestCommentAttributes()
        {
            this.AssertTransformedInside("CommentAttributes", "pl-e", "pl-c");
        }

        /// <summary>
        /// Verifies that XML attribute values in comments are decorated with "pl-s".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestCommentAttributeValues()
        {
            this.AssertTransformedInside("CommentAttributeValues", "pl-s", "pl-c");
        }

        /// <summary>
        /// Verifies that string literals are decorated with "pl-s".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestStrings()
        {
            this.AssertTransformedNotInside("Strings", "pl-s", "pl-c");
        }

        /// <summary>
        /// Verifies that string literals containing escape sequences are decorated with "pl-s".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestStringsWithEscape()
        {
            this.AssertTransformedNotInside("StringsWithEscape", "pl-s", "pl-c");
        }

        /// <summary>
        /// Verifies that signed and unsigned decimal number literals are decorated with "pl-c1".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestDecimalNumbers()
        {
            this.AssertTransformedOnlyPattern("DecimalNumbers", "pl-c1", NumberPattern);
        }

        /// <summary>
        /// Verifies that decimal number literals are decorated with "pl -c1".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestDecimalNumbersWithPostfix()
        {
            this.AssertTransformedOnlyPattern("DecimalNumbersWithPostfix", "pl-c1", NumberPattern);
        }

        /// <summary>
        /// Verifies that decimal number literals with exponent are decorated with "pl-c1".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestDecimalNumbersWithExponent()
        {
            this.AssertTransformed("DecimalNumbersWithExponent", "pl-c1");
        }

        /// <summary>
        /// Verifies that hexadecimal number literals are decorated with "pl-c1".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestHexNumbers()
        {
            this.AssertTransformedOnlyPattern("HexNumbers", "pl-c1", "0x");
        }

        /// <summary>
        /// Verifies that hexadecimal number literals with postfix are decorated with "pl-c1".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestHexNumbersWithPostfix()
        {
            // U, u, L, l, F, f, UL, Ul, ul, LL, Ll, ll
        }

        /// <summary>
        /// Verifies that C# keywords are decorated with "pl-k".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestKeywords()
        {
        }

        /// <summary>
        /// Verifies that C# literals like null, true, false are decorated with "pl-c1".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestConstantLiterals()
        {
        }

        /// <summary>
        /// Verifies that identifiers that follow "const" and "case" are decorated with "pl-c1".
        /// </summary>
        /// <remarks>This does not apply to string and number literals.</remarks>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestConstantIdentifiers()
        {
        }

        /// <summary>
        /// Verifies that container names when the containers are defined are decorated with "pl-e".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestStorageEntities()
        {
        }

        /// <summary>
        /// Verifies that object members are decorated with "pl-e".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestMemberEntities()
        {
        }

        /// <summary>
        /// Verifies that function calls and constructors are decorated with "pl-e".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestFunctionCallEntities()
        {
        }

        /// <summary>
        /// Verifies that base class name when defining a class is decorated with "pl-en".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestBaseClassEntityNames()
        {
        }

        /// <summary>
        /// Verifies that types of variables declared in classes, structs and functions are decorated with "pl-en".
        /// </summary>
        /// <remarks>This applies only to user types as built-in types are decorated as "pl-k".</remarks>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestMemberEntityNames()
        {
        }

        /// <summary>
        /// Verifies that types in generic templates are decorated with "pl-en".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestTemplateEntityNames()
        {
        }

        /// <summary>
        /// Verifies that the parameter to typeof() function call is decorated with "pl-en".
        /// </summary>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestTypeOfEntityNames()
        {
        }

        /// <summary>
        /// Verifies that the token after new operator is decorated with "pl-en".
        /// </summary>
        /// <remarks>This does not apply to built-in types, which are decorated with "pl-k".</remarks>
        [TestMethod]
        [TestCategory("C# Syntax Highlighting")]
        public void TestNewEntityNames()
        {
        }
    }
}