//-----------------------------------------------------------------------
// <copyright file="TransformTokenSpan.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Transforms the span formed by a token match.
    /// </summary>
    public class TransformTokenSpan : TransformToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformTokenSpan"/> class.
        /// </summary>
        /// <param name="name">The transform name for debugging.</param>
        /// <param name="description">The transform description for debugging.</param>
        /// <param name="patternName">The pattern name for debugging.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="modifierPatternName">The modifier pattern name for debugging.</param>
        /// <param name="modifierPattern">The pattern to match to the previous token and previous separator, whichever succeeds.</param>
        /// <param name="className">The class name of the transformed token.</param>
        /// <param name="transforms">The transforms to apply to inner token content.</param>
        /// <param name="escape">Whether to escape HTML entities.</param>
        /// <param name="excludeClassNames">The class name the previous token must not match.</param>
        public TransformTokenSpan(
            string name,
            string description,
            string patternName,
            Regex pattern,
            string modifierPatternName,
            Regex modifierPattern,
            string className,
            List<TransformToken> transforms,
            bool escape,
            params string[] excludeClassNames)
            : base(name, description, patternName, pattern, modifierPatternName, modifierPattern, className, excludeClassNames)
        {
            this.Transforms = transforms;
            this.Escape = escape;
        }

        /// <summary>
        /// Gets or sets the transforms to apply to span contents.
        /// </summary>
        public List<TransformToken> Transforms { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to escape web entities.
        /// </summary>
        public bool Escape { get; set; }

        /// <summary>
        /// Apply the transform to the text buffer.
        /// </summary>
        /// <param name="buffer">The text buffer.</param>
        /// <param name="options">The syntax highlight options.</param>
        /// <returns>Whether the transformation was applied.</returns>
        public override bool Apply(Buffer buffer, Options options)
        {
            Match tokenMatch = this.Pattern.Match(buffer.Data, buffer.Position);

            bool modifierMatch = this.ModifierPattern == null ||
                this.ModifierPattern.IsMatch(buffer.PrevToken) ||
                this.ModifierPattern.IsMatch(buffer.PrevSeparator.ToString());

            bool typeMatch = !this.ExcludeClassNames.Any(
                exclude => exclude != null && buffer.PrevClass == exclude);

            buffer.Break(tokenMatch.Success, this.Name, this.ClassName, tokenMatch.Value);

            if (tokenMatch.Success && tokenMatch.Index == buffer.Position &&
                modifierMatch &&
                typeMatch)
            {
                string content = Buffer.ExplicitMatch(tokenMatch);

                if (this.Transforms.Count > 0)
                {
                    content = this.TransformInnerTokens(tokenMatch.Value, this.Transforms.ToArray());
                }
                else if (this.Escape)
                {
                    content = Buffer.EncodeContent(tokenMatch.Value);
                }

                if (string.IsNullOrEmpty(this.ClassName))
                {
                    buffer.ReplaceSpan(tokenMatch, content);
                }
                else
                {
                    buffer.ReplaceSpan(tokenMatch, Buffer.FormatToken(content, this.ClassName, this.Name));
                }

                buffer.NextSeparator = buffer.PrevSeparator;
                buffer.PrevToken = string.Empty;
                buffer.PrevClass = this.ClassName;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs non context-specific token transformations.
        /// </summary>
        /// <param name="source">The source text.</param>
        /// <param name="transforms">The transforms to apply.</param>
        /// <returns>The transformed text.</returns>
        private string TransformInnerTokens(string source, TransformToken[] transforms)
        {
            for (int n = 0; n < transforms.Length; n++)
            {
                bool transformed = false;

                source = transforms[n].Pattern.Replace(
                    source,
                    match =>
                    {
                        transformed = true;

                        return this.InnerTransformEvaluator(
                            match,
                            transforms[n].Name,
                            transforms[n].ClassName,
                            transforms.Skip(1).ToArray());
                    });

                if (transformed)
                {
                    break;
                }
            }

            return source;
        }

        /// <summary>
        /// Evaluates global transforms for each matching instance.
        /// </summary>
        /// <param name="match">The match to evaluate.</param>
        /// <param name="transformName">The name of the transform, if debug options are on.</param>
        /// <param name="className">The token class name to apply.</param>
        /// <param name="transforms">The transforms to apply to inner content.</param>
        /// <returns>The transformed token.</returns>
        private string InnerTransformEvaluator(Match match, string transformName, string className, TransformToken[] transforms)
        {
            string token = match.Value;
            string content = Buffer.ExplicitMatch(match);
            string formatted = content;

            if (!string.IsNullOrEmpty(className))
            {
                formatted = Buffer.FormatToken(
                    this.Escape ? Buffer.EncodeContent(content) : content,
                    className,
                    transformName);
            }

            int leftOffset = token.IndexOf(content);
            int rightOffset = token.Length - content.Length - leftOffset;

            if (leftOffset == 0 && rightOffset == 0)
            {
                return formatted;
            }

            string left = this.Escape ? Buffer.EncodeContent(
                token.Substring(0, leftOffset)) : token.Substring(0, leftOffset);

            string right = this.Escape ? Buffer.EncodeContent(
                token.Substring(token.Length - rightOffset)) : token.Substring(token.Length - rightOffset);

            if (rightOffset > 3)
            {
                right = this.TransformInnerTokens(right, transforms);
            }

            return left + formatted + right;
        }
    }
}
