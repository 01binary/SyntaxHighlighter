//-----------------------------------------------------------------------
// <copyright file="SharedTests.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Shared test functionality.
    /// </summary>
    public class SharedTests
    {
        /// <summary>
        /// Gets or sets the name of the transform definition being tested.
        /// </summary>
        public string TestTransformDefinitionName
        {
            get
            {
                var attr = Attribute.GetCustomAttribute(
                    this.GetType(), typeof(TestTransformAttribute), true) as TestTransformAttribute;

                if (attr != null)
                {
                    return attr.TransformDefinitionName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the automatic test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Verifies whether transforming the specified sample produced the expected output.
        /// </summary>
        /// <param name="sample">The sample file name.</param>
        /// <param name="className">The class name of tokens to verify.</param>
        public void AssertTransformed(string sample, string className)
        {
            Shared.VerifyTransform(
                TestContext.DeploymentDirectory, sample, this.TestTransformDefinitionName, className, null, null, null);
        }

        /// <summary>
        /// Verifies whether transforming the specified sample produced the expected output.
        /// </summary>
        /// <param name="sample">The sample file name.</param>
        /// <param name="className">The class name of tokens to verify.</param>
        /// <param name="insideClassName">Verify only tokens inside a span of this class.</param>
        public void AssertTransformedInside(string sample, string className, string insideClassName)
        {
            Shared.VerifyTransform(
                TestContext.DeploymentDirectory, sample, this.TestTransformDefinitionName, className, insideClassName, null, null);
        }

        /// <summary>
        /// Verifies whether transforming the specified sample produced the expected output.
        /// </summary>
        /// <param name="sample">The sample file name.</param>
        /// <param name="className">The class name of tokens to verify.</param>
        /// <param name="outsideClassName">Verify only tokens inside a span of this class.</param>
        public void AssertTransformedNotInside(string sample, string className, string outsideClassName)
        {
            Shared.VerifyTransform(
                TestContext.DeploymentDirectory, sample, this.TestTransformDefinitionName, className, null, outsideClassName, null);
        }

        /// <summary>
        /// Verifies whether transforming the specified sample produced the expected output.
        /// </summary>
        /// <param name="sample">The sample file name.</param>
        /// <param name="className">The class name of tokens to verify.</param>
        /// <param name="pattern">Verify only tokens that match this pattern.</param>
        public void AssertTransformedOnlyPattern(string sample, string className, string pattern)
        {
            Shared.VerifyTransform(
                TestContext.DeploymentDirectory, sample, this.TestTransformDefinitionName, className, null, null, pattern);
        }
    }
}
