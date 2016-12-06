//-----------------------------------------------------------------------
// <copyright file="Shared.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Tests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Shared unit test functionality.
    /// </summary>
    internal class Shared
    {
        /// <summary>
        /// The marker signaling beginning of transformed token span.
        /// </summary>
        private static readonly string SpanMarker = "<span class=\"";

        /// <summary>
        /// The marker signaling the beginning of span content.
        /// </summary>
        private static readonly string SpanContentMarker = "\">";

        /// <summary>
        /// The marker signaling the end of span content.
        /// </summary>
        private static readonly string SpanEndMarker = "</span>";

        /// <summary>
        /// The marker inserted into actual result within test output folder.
        /// </summary>
        private static readonly string ErrorMarker = "<span class=\"err\">&#214;</span>";

        /// <summary>
        /// Performs the syntax highlight transform using sample in Input folder.
        /// </summary>
        /// <param name="baseDirectory">The test deployment directory.</param>
        /// <param name="sample">The name of code snippet .txt file in Input folder.</param>
        /// <param name="codeType">The type of code transform.</param>
        /// <param name="className">The class name of the tokens to verify.</param>
        public static void VerifyTransform(string baseDirectory, string sample, string codeType, string className)
        {
            // Initialize highlighter with transforms from test deployment folder.
            Highlighter highlighter = new Highlighter(Path.Combine(baseDirectory, "Transforms"));

            // Get the test inputs deployment folder.
            string input = Path.Combine(baseDirectory, "Input", string.Format("{0}.txt", sample));

            // Get the test outputs deployment folder.
            string actualDir = Path.Combine(baseDirectory, "Actual");

            // Get the actual result paths.
            string sampleHtml = string.Format("{0}.htm", sample);
            string actual = Path.Combine(actualDir, sampleHtml);
            string actualCopy = Path.Combine(GetProjectActualDirectory(baseDirectory), sampleHtml);

            // Get the test expected results deployment folder.
            string expected = Path.Combine(baseDirectory, "Expected", string.Format("{0}.htm", sample));

            // Get the template deployment folder.
            string template = Path.Combine(baseDirectory, "Template", "Template.htm");

            // Transform the input sample.
            string content = highlighter.Transform(File.ReadAllText(input), codeType);

            // Dress up the result with a template for manual viewing.
            string actualContent = string.Format(File.ReadAllText(template), content);

            if (!Directory.Exists(actualDir))
            {
                Directory.CreateDirectory(actualDir);
            }

            // Compare actual result with expected result.
            Exception assert = null;
            int assertPos = 0;

            try
            {
                VerifySpans(File.ReadAllText(expected), actualContent, className, out assertPos);
            }
            catch (Exception ex)
            {
                // We can't let assert fail the test immediately because we want to get the position.
                assert = ex;
            }

            try
            {
                // Write the actual result without error marker to project Actual folder,
                // to enable using source control diffs for debugging.
                File.WriteAllText(actualCopy, actualContent);
            }
            catch
            {
                // Test shouldn't fail because of this optional step.
            }

            if (assert != null)
            {
                // Modify actual result written to the Output folder and insert an error marker.
                actualContent = string.Concat(
                    actualContent.Substring(0, assertPos),
                    ErrorMarker,
                    actualContent.Substring(assertPos));
            }

            // Write the actual result with error marker (if there was an error) to the test output folder.
            File.WriteAllText(actual, actualContent);

            // If the test failed, throw at this point.
            if (assert != null)
            {
                throw assert;
            }
        }

        /// <summary>
        /// Verifies that spans in expected HTML match spans in actual HTML.
        /// </summary>
        /// <param name="expected">The expected HTML.</param>
        /// <param name="actual">The actual HTML.</param>
        /// <param name="className">The span class name.</param>
        /// <param name="pos">Character offset in actual string where the error occurred.</param>
        private static void VerifySpans(string expected, string actual, string className, out int pos)
        {
            int expectedPos = 0;
            int actualPos = pos = 0;

            string expectedClass;
            string actualClass;
            string expectedContent;
            string actualContent;

            while (NextSpan(expected, ref expectedPos, out expectedClass, out expectedContent))
            {
                bool found = NextSpan(actual, ref actualPos, out actualClass, out actualContent);

                if (expectedClass != className)
                {
                    continue;
                }

                pos = actualPos;

                Assert.IsTrue(
                    found,
                    string.Format("expected token {0} with class {1} at {2} not found", expectedContent, className, actualPos));

                Assert.AreEqual(
                    expectedContent,
                    actualContent,
                    string.Format("unexpected token value at {0}", actualPos));

                Assert.AreEqual(
                    actualClass,
                    className,
                    string.Format("token {0} at {1} does not have {2} class", actualContent, actualPos, className));
            }
        }

        /// <summary>
        /// Finds the next span and extracts class and content.
        /// </summary>
        /// <param name="content">The string to search for spans.</param>
        /// <param name="pos">The current position in the string.</param>
        /// <param name="spanClass">The extracted span class.</param>
        /// <param name="spanContent">The extracted span content.</param>
        /// <returns>Whether a span was found at or after the current position.</returns>
        private static bool NextSpan(string content, ref int pos, out string spanClass, out string spanContent)
        {
            pos = content.IndexOf(SpanMarker, pos);
            spanClass = string.Empty;
            spanContent = string.Empty;

            if (pos == -1)
            {
                return false;
            }

            int end = FindNestedSpanEnd(content, pos + SpanMarker.Length);
            int contentStart = content.IndexOf(SpanContentMarker, pos);

            spanClass = content.Substring(
                pos + SpanMarker.Length,
                contentStart - pos - SpanMarker.Length);

            spanContent = content.Substring(
                contentStart + SpanContentMarker.Length,
                end - contentStart - SpanContentMarker.Length);

            pos = contentStart + SpanContentMarker.Length;

            return true;
        }

        /// <summary>
        /// Finds the end position of a span that can contain other spans.
        /// </summary>
        /// <param name="content">The content to search.</param>
        /// <param name="start">The search start position.</param>
        /// <returns>The end position.</returns>
        private static int FindNestedSpanEnd(string content, int start)
        {
            int depth = 1;

            while (start < content.Length - SpanMarker.Length)
            {
                if (content.Substring(start, SpanMarker.Length) == SpanMarker)
                {
                    depth++;
                }
                else if (content.Substring(start, SpanEndMarker.Length) == SpanEndMarker)
                {
                    depth--;

                    if (depth < 1)
                    {
                        break;
                    }
                }

                start = content.IndexOf('<', start + 1);
            }

            return start;
        }

        /// <summary>
        /// Gets the path to the test project Actual folder for storing test outputs.
        /// </summary>
        /// <param name="baseDirectory">The test deployment directory.</param>
        /// <returns>The project Actual path.</returns>
        /// <remarks>
        /// Test outputs are copied from deployment folder back to Actual folder
        /// in the project so that we can use Visual Studio integrated diff to see
        /// how HTML written by a failed test is different from expected HTML.
        /// These files should be checked in only if expected HTML changes.
        /// </remarks>
        private static string GetProjectActualDirectory(string baseDirectory)
        {
            string deployDirectory = Path.GetDirectoryName(baseDirectory);
            string testResultsDirectory = Path.GetDirectoryName(deployDirectory);
            string solutionDirectory = Path.GetDirectoryName(testResultsDirectory);
            string testProjectDirectory = Path.Combine(
                solutionDirectory,
                "SyntaxHighlighter.Tests");

            return Path.Combine(testProjectDirectory, "Actual");
        }
    }
}
