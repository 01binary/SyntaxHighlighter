//-----------------------------------------------------------------------
// <copyright file="DebugOptions.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    /// <summary>
    /// Syntax highlight debug options.
    /// </summary>
    public class DebugOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to print each token.
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets the token to break on.
        /// </summary>
        public string BreakOnToken { get; set; }

        /// <summary>
        /// Gets or sets the class name that controls whether to break on the specified token.
        /// </summary>
        /// <remarks>Break on token matched with unexpected class, or not matched with expected class.</remarks>
        public string BreakOnNotClass { get; set; }

        /// <summary>
        /// Gets or sets the number of matches to skip before breaking.
        /// </summary>
        public int BreakSkip { get; set; }
    }
}
