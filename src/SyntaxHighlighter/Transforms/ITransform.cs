//-----------------------------------------------------------------------
// <copyright file="ITransform.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    /// <summary>
    /// The text transform contract.
    /// </summary>
    internal interface ITransform
    {
        /// <summary>
        /// Apply the text transform.
        /// </summary>
        /// <param name="buffer">The buffer to apply to.</param>
        /// <param name="options">The syntax highlight options.</param>
        /// <returns>Whether the transformation took place.</returns>
        bool Apply(Buffer buffer, Options options);
    }
}
