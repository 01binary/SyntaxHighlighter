//-----------------------------------------------------------------------
// <copyright file="TransformToken.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Transforms a token that matches a pattern when previous token class name is not excluded.
    /// </summary>
    internal class TransformToken : ITransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformToken"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="className">The class name for the transformed token.</param>
        public TransformToken(Regex pattern, string className)
        {
            this.Pattern = pattern;
            this.ClassName = className;
        }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        public Regex Pattern { get; set; }

        /// <summary>
        /// Gets or sets the token class name.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Applies the transform to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to apply the transform to.</param>
        /// <returns>Whether the transformation was applied.</returns>
        public virtual bool Apply(Buffer buffer)
        {
            Match match = this.Pattern.Match(
                buffer.Data,
                buffer.Position,
                buffer.Next - buffer.Position);
#if DEBUG
            buffer.Break(match.Success, this.ClassName);
#endif
            if (match.Success)
            {
                string content = Buffer.ExplicitMatch(match);
                string token = Buffer.FormatToken(content, this.ClassName);

                buffer.ReplaceSpan(match, token, content.Length);
                buffer.PrevToken = content;
                buffer.PrevClass = this.ClassName;

                return true;
            }

            return false;
        }
    }
}
