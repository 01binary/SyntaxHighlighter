//-----------------------------------------------------------------------
// <copyright file="TransformToken.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Transforms a token that matches a pattern when previous token class name is not excluded.
    /// </summary>
    public class TransformToken : ITransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformTokenModifier"/> class.
        /// </summary>
        /// <param name="name">The transform name for debugging.</param>
        /// <param name="description">The transform description for debugging.</param>
        /// <param name="patternName">The pattern name for debugging.</param>
        /// <param name="pattern">The pattern to match to the current token.</param>
        /// <param name="modifierPatternName">The modifier pattern name for debugging.</param>
        /// <param name="modifierPattern">The pattern to match to the previous token and previous separator, whichever succeeds.</param>
        /// <param name="className">The transformed token class.</param>
        /// <param name="excludeClassNames">The class name the previous token must not match.</param>
        public TransformToken(string name, string description, string patternName, Regex pattern, string modifierPatternName, Regex modifierPattern, string className, params string[] excludeClassNames)
        {
            this.Name = name;
            this.Description = description;
            this.PatternName = patternName;
            this.Pattern = pattern;
            this.ClassName = className;
            this.ModifierPatternName = modifierPatternName;
            this.ModifierPattern = modifierPattern;
            this.ExcludeClassNames = excludeClassNames;
        }

        /// <summary>
        /// Gets or sets the transform name for debugging.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the transform description for debugging.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        public Regex Pattern { get; set; }

        /// <summary>
        /// Gets or sets the pattern name for debugging.
        /// </summary>
        public string PatternName { get; set; }

        /// <summary>
        /// Gets or sets the token class name.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the modifier pattern name for debugging.
        /// </summary>
        public string ModifierPatternName { get; set; }

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
        /// <param name="options">The syntax highlight options.</param>
        /// <returns>Whether the transformation took place.</returns>
        public virtual bool Apply(Buffer buffer, Options options)
        {
            Match tokenMatch = this.Pattern.Match(
                buffer.Data, buffer.Position, buffer.Next - buffer.Position);

            bool modifierMatch = this.ModifierPattern == null ||
                this.ModifierPattern.IsMatch(buffer.PrevToken) ||
                this.ModifierPattern.IsMatch(buffer.PrevSeparator.ToString());

            bool typeMatch = !this.ExcludeClassNames.Any(
                exclude => exclude != null && buffer.PrevClass == exclude);
#if DEBUG
            buffer.Break(modifierMatch && tokenMatch.Success && typeMatch, this.Name, this.ClassName);
#endif
            if (typeMatch && modifierMatch && tokenMatch.Success)
            {
                string content = Buffer.ExplicitMatch(tokenMatch);
                string token = Buffer.FormatToken(content, this.ClassName, this.Name);

                buffer.ReplaceSpan(tokenMatch, token, content.Length);

                buffer.PrevToken = content;
                buffer.PrevClass = this.ClassName;

                return true;
            }

            return false;
        }
    }
}
