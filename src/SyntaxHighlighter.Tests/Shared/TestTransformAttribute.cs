//-----------------------------------------------------------------------
// <copyright file="TestTransformAttribute.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Tests
{
    using System;

    /// <summary>
    /// Allows specifying transform definition name once for a test class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TestTransformAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestTransformAttribute"/> class.
        /// </summary>
        /// <param name="transformDefinitionName">The name of the transform definition to test.</param>
        public TestTransformAttribute(string transformDefinitionName)
        {
            this.TransformDefinitionName = transformDefinitionName;
        }

        /// <summary>
        /// Gets or sets the name of the transform definition.
        /// </summary>
        public string TransformDefinitionName { get; set; }
    }
}
