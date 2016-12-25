//-----------------------------------------------------------------------
// <copyright file="Buffer.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// The transform buffer.
    /// </summary>
    public class Buffer
    {
        /// <summary>
        /// The token span format.
        /// </summary>
        private static readonly string SpanFormat = "<span class=\"{0}\">{1}</span>";

        /// <summary>
        /// The token format with debug information included.
        /// </summary>
        private static readonly string DebugSpanFormat = "<span class=\"{0}\" data-transform=\"{1}\">{2}</span>";

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer"/> class.
        /// </summary>
        /// <param name="data">The initial data.</param>
        /// <param name="tokenSeparators">The pattern used to detect token separators.</param>
        public Buffer(string data, Regex tokenSeparators)
        {
            this.Data = data;
            this.Separators = tokenSeparators;
            this.PrevToken = string.Empty;
            this.Next = this.FindLastSeparator();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer"/> class.
        /// </summary>
        /// <param name="data">The initial data.</param>
        /// <param name="tokenSeparators">The pattern used to detect token separators.</param>
        /// <param name="options">The debugging options.</param>
        public Buffer(string data, Regex tokenSeparators, Options options)
        {
            this.Data = data;
            this.Separators = tokenSeparators;
            this.PrevToken = string.Empty;
            this.Next = this.FindLastSeparator();
            this.Options = options;
        }

        /// <summary>
        /// Gets or sets the text buffer being transformed.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the position of the next token separator.
        /// </summary>
        public int Next { get; set; }

        /// <summary>
        /// Gets or sets the previous token separator.
        /// </summary>
        public char PrevSeparator { get; set; }

        /// <summary>
        /// Gets or sets the next token separator.
        /// </summary>
        public char NextSeparator { get; set; }

        /// <summary>
        /// Gets or sets the last token parsed.
        /// </summary>
        public string PrevToken { get; set; }

        /// <summary>
        /// Gets or sets the last token class.
        /// </summary>
        public string PrevClass { get; set; }

        /// <summary>
        /// Gets or sets the pattern used to detect token separators.
        /// </summary>
        public Regex Separators { get; set; }

        /// <summary>
        /// Gets or sets the options for enabling verbose output and breaking on tokens.
        /// </summary>
        public Options Options { get; set; }

        /// <summary>
        /// Gets a value indicating whether the end of buffer was reached.
        /// </summary>
        /// <returns>Whether parsing is at the end of buffer.</returns>
        public bool Eof
        {
            get
            {
                return this.Position >= this.Data.Length - 1;
            }
        }

        /// <summary>
        /// Extracts the explicit capture group value from a match.
        /// </summary>
        /// <param name="match">The match to examine.</param>
        /// <returns>The explicit capture group value.</returns>
        public static string ExplicitMatch(Match match)
        {
            for (int n = 1; n < match.Groups.Count; n++)
            {
                if (match.Groups[n].Length > 0)
                {
                    return match.Groups[n].Value;
                }
            }

            return match.Groups[0].Value;
        }

        /// <summary>
        /// Encodes special characters to HTML entities.
        /// </summary>
        /// <param name="content">The content to encode.</param>
        /// <returns>The valid HTML.</returns>
        public static string EncodeContent(string content)
        {
            return content
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        /// <summary>
        /// Formats a token.
        /// </summary>
        /// <param name="tokenContent">The token content.</param>
        /// <param name="tokenClass">The class of token to output.</param>
        /// <param name="transformName">The transform name for debugging (can be null).</param>
        /// <param name="match">The entire match used to keep outer content, if specified.</param>
        /// <returns>The formatted token.</returns>
        public static string FormatToken(string tokenContent, string tokenClass, string transformName, Match match = null)
        {
            string formatted;

            if (transformName != null)
            {
                formatted = string.Format(DebugSpanFormat, tokenClass, transformName, tokenContent);
            }
            else
            {
                formatted = string.Format(SpanFormat, tokenClass, tokenContent);
            }

            if (match != null && match.Length > tokenContent.Length)
            {
                formatted += match.Value.Substring(tokenContent.Length);
            }

            return formatted;
        }

        /// <summary>
        /// Finds the beginning and the end of the next token to parse.
        /// </summary>
        /// <remarks>
        /// Stores the preceding token separator unless it was a whitespace.
        /// </remarks>
        public void NextToken()
        {
            this.PrevSeparator = this.NextSeparator;
            this.Next = this.FindLastSeparator();
        }

        /// <summary>
        /// Moves past the current token without transforming it.
        /// </summary>
        public void SkipToken()
        {
            this.Position = this.Next;
            this.PrevToken = string.Empty;
            this.PrevClass = null;
        }

        /// <summary>
        /// Finds the last token separator before the next token in the buffer.
        /// </summary>
        /// <returns>Index of the last token separator.</returns>
        public int FindLastSeparator()
        {
            int start = this.FindNextSeparator(this.Position);
            int length = 1;

            while (start < this.Data.Length &&
                   this.IsTokenSeparator(start, out length))
            {
                for (int n = start; n < start + length; n++)
                {
                    if (!char.IsWhiteSpace(this.Data[n]))
                    {
                        this.NextSeparator = this.Data[n];
                    }
                }

                start += length;
            }

            return start;
        }

        /// <summary>
        /// Finds the next token separator in the buffer and returns its index.
        /// </summary>
        /// <param name="start">Buffer position to search from.</param>
        /// <returns>Index of the next token separator.</returns>
        public int FindNextSeparator(int start)
        {
            int length = 1;

            while (start < this.Data.Length &&
                   !this.IsTokenSeparator(start, out length))
            {
                start += length;
            }

            return start + length - 1;
        }

        /// <summary>
        /// Determines if the character at the specified position in the buffer is a token separator.
        /// </summary>
        /// <param name="pos">The position in the buffer.</param>
        /// <param name="length">The length of the token separator.</param>
        /// <returns>Whether the character (possibly including next character) is a token separator.</returns>
        public bool IsTokenSeparator(int pos, out int length)
        {
            int maxLength = pos + 2 >= this.Data.Length ? 1 : 2;
            Match match = this.Separators.Match(this.Data, pos, maxLength);

            if (match.Success && match.Index == pos)
            {
                length = ExplicitMatch(match).Length;
                return true;
            }
            else
            {
                length = 1;
                return false;
            }
        }

        /// <summary>
        /// Determines if the character at the specified position in the buffer is a token separator.
        /// </summary>
        /// <param name="externalData">An external buffer to check.</param>
        /// <param name="pos">The position in the buffer.</param>
        /// <returns>Whether the character (possibly including next character) is a token separator.</returns>
        public bool IsTokenSeparator(string externalData, int pos)
        {
            int maxLength = pos + 2 >= externalData.Length ? 1 : 2;
            Match match = this.Separators.Match(externalData, pos, maxLength);

            if (match.Success && match.Index == pos)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Performs a transform replacement.
        /// </summary>
        /// <param name="match">The raw token to replace.</param>
        /// <param name="token">The formatted token to replace with.</param>
        /// <param name="length">The portion of the raw token to replace (all by default).</param>
        public void ReplaceSpan(Match match, string token, int length = 0)
        {
            if (length == 0)
            {
                length = match.Length;
            }

            this.Data = string.Concat(
                this.Data.Substring(0, match.Index),
                token,
                this.Data.Substring(match.Index + length));

            this.Position = match.Index + token.Length +
                match.Length - length;
        }

        /// <summary>
        /// Breaks on the specified token and class if the debug options are turned on.
        /// </summary>
        /// <param name="match">Whether the token has the specified class name.</param>
        /// <param name="transformName">The name of the transform if debug info is enabled.</param>
        /// <param name="className">The detected token class.</param>
        /// <param name="token">The token to evaluate.</param>
        public void Break(bool match, string transformName, string className, string token = null)
        {
            if (this.Options == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(token))
            {
                token = this.Data.Substring(
                    this.Position, this.Next - this.Position);
            }

            bool breakOnTransform = false;
            bool breakOnToken = false;
            string breakMessage = string.Empty;

            if (!string.IsNullOrEmpty(this.Options.BreakOnTransform) &&
                !string.IsNullOrEmpty(transformName) &&
                transformName == this.Options.BreakOnTransform)
            {
                breakOnTransform = true;
                breakMessage = string.Format(
                    "Breaking on transform {0}",
                    this.Options.BreakOnTransform);
            }

            if (!string.IsNullOrEmpty(this.Options.BreakOnToken) &&
                token.StartsWith(this.Options.BreakOnToken) &&
                (this.Options.BreakOnNotClass == null ||
                    (match && this.Options.BreakOnNotClass != className) ||
                    (!match && this.Options.BreakOnNotClass == className)))
            {
                if (this.Options.BreakOnNotClass != null)
                {
                    string reason = string.Empty;

                    if (match && this.Options.BreakOnNotClass != className)
                    {
                        reason = "matches unexpected class";
                    }
                    else
                    {
                        reason = "does not match expected class";
                    }

                    breakOnToken = true;
                    breakMessage = string.Format(
                        "Breaking because '{0}' {1} {2}",
                        this.Options.BreakOnToken,
                        reason,
                        this.Options.BreakOnNotClass.ToString());
                }
                else if (!this.Options.BreakOnMatch || (this.Options.BreakOnMatch && match))
                {
                    breakOnToken = true;
                    breakMessage = string.Format(
                        "Breaking on {0}",
                        this.Options.BreakOnToken);
                }

                if (breakOnToken || breakOnTransform)
                {
                    if (this.Options.BreakSkip > 0)
                    {
                        this.Options.BreakSkip--;
                        return;
                    }

                    if (!string.IsNullOrEmpty(this.Options.BreakOnToken) && !breakOnToken)
                    {
                        // If break on token specified, break only if token matches.
                        return;
                    }

                    if (!string.IsNullOrEmpty(this.Options.BreakOnTransform) && !breakOnTransform)
                    {
                        // If brak on transform specified, break only if transform matches.
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine(breakMessage);
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
    }
}