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
        /// <param name="className">The class name of the transformed token.</param>
        /// <param name="transforms">The transforms to apply to inner token content.</param>
        public TransformTokenSpan(string name, string description, string patternName, Regex pattern, string className, List<TransformToken> transforms)
            : base(name, description, patternName, pattern, className)
        {
            this.Transforms = transforms;
        }

        /// <summary>
        /// Gets or sets the transforms to apply to span contents.
        /// </summary>
        public List<TransformToken> Transforms { get; set; }

        /// <summary>
        /// Apply the transform to the text buffer.
        /// </summary>
        /// <param name="buffer">The text buffer.</param>
        /// <param name="options">The syntax highlight options.</param>
        /// <returns>Whether the transformation was applied.</returns>
        public override bool Apply(Buffer buffer, Options options)
        {
            Match match = this.Pattern.Match(buffer.Data, buffer.Position);

            if (match.Success && match.Index == buffer.Position)
            {
                string content;

                if (this.Transforms.Count > 0)
                {
                    content = this.TransformInnerTokens(match.Value, this.Transforms.ToArray());
                }
                else
                {
                    content = Buffer.EncodeContent(match.Value);
                }

                buffer.ReplaceSpan(match, Buffer.FormatToken(content, this.ClassName, this.Name));
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
            if (transforms.Length == 0)
            {
                return source;
            }

            return transforms[0].Pattern.Replace(
                source,
                match =>
            {
                return this.InnerTransformEvaluator(
                    match,
                    transforms[0].ClassName,
                    transforms.Skip(1).ToArray());
            });
        }

        /// <summary>
        /// Evaluates global transforms for each matching instance.
        /// </summary>
        /// <param name="match">The match to evaluate.</param>
        /// <param name="className">The token class name to apply.</param>
        /// <param name="transforms">The transforms to apply to inner content.</param>
        /// <returns>The transformed token.</returns>
        private string InnerTransformEvaluator(Match match, string className, TransformToken[] transforms)
        {
            string token = match.Value;
            string content = Buffer.ExplicitMatch(match);

            string formatted = Buffer.FormatToken(
                Buffer.EncodeContent(content),
                className,
                this.Name);

            int leftOffset = token.IndexOf(content);
            int rightOffset = token.Length - content.Length - leftOffset;

            if (leftOffset == 0 && rightOffset == 0)
            {
                return formatted;
            }

            string left = Buffer.EncodeContent(
                token.Substring(0, leftOffset));

            string right = Buffer.EncodeContent(
                token.Substring(token.Length - rightOffset));

            if (rightOffset > 3)
            {
                right = this.TransformInnerTokens(right, transforms);
            }

            return left + formatted + right;
        }
    }
}
