//-----------------------------------------------------------------------
// <copyright file="TransformTokenModifier.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Transforms a token if the previous token or separator matches the modifier.
    /// </summary>
    internal class TransformTokenModifier : TransformToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformTokenModifier"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to match to the current token.</param>
        /// <param name="modifierPattern">The pattern to match to the previous token and previous separator, whichever succeeds.</param>
        /// <param name="className">The transformed token class.</param>
        /// <param name="excludeClassNames">The class name the previous token must not match.</param>
        public TransformTokenModifier(Regex pattern, Regex modifierPattern, string className, params string[] excludeClassNames)
            : base(pattern, className)
        {
            this.ModifierPattern = modifierPattern;
            this.ExcludeClassNames = excludeClassNames;
        }

        /// <summary>
        /// Gets or sets the modifier pattern.
        /// </summary>
        /// <remarks>
        /// This pattern must match either the previous token or previous separator.
        /// </remarks>
        public Regex ModifierPattern { get; set; }

        /// <summary>
        /// Gets or sets the previous token class name to exclude.
        /// </summary>
        public string[] ExcludeClassNames { get; set; }

        /// <summary>
        /// Applies the transform to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to apply to.</param>
        /// <returns>Whether the transformation took place.</returns>
        public override bool Apply(Buffer buffer)
        {
            Match tokenMatch = this.Pattern != null ?
                this.Pattern.Match(buffer.Data, buffer.Position, buffer.Next - buffer.Position) : null;

            bool matchesModifier = this.ModifierPattern.IsMatch(buffer.PrevToken) ||
                 this.ModifierPattern.IsMatch(buffer.PrevSeparator.ToString());

            bool matchesToken = this.Pattern == null || tokenMatch.Success;

            bool matchesType = !this.ExcludeClassNames.Any(
                exclude => exclude != null && buffer.PrevClass == exclude);
#if DEBUG
            buffer.Break(matchesModifier && matchesToken && matchesType, this.ClassName);
#endif
            if (matchesType && matchesModifier && matchesToken)
            {
                string content;

                if (this.Pattern == null)
                {
                    content = buffer.Data.Substring(
                        buffer.Position, buffer.Next - buffer.Position);
                }
                else
                {
                    content = Buffer.ExplicitMatch(tokenMatch);
                }

                string token = Buffer.FormatToken(content, this.ClassName);

                if (tokenMatch != null && tokenMatch.Length > content.Length)
                {
                    token += tokenMatch.Value.Substring(content.Length);
                }

                buffer.Data = string.Concat(
                    buffer.Data.Substring(0, buffer.Position),
                    token,
                    buffer.Data.Substring(buffer.Next));

                buffer.PrevToken = content;
                buffer.PrevClass = this.ClassName;
                buffer.Position += token.Length;

                return true;
            }

            return false;
        }
    }
}
