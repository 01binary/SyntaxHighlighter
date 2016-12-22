//-----------------------------------------------------------------------
// <copyright file="Highlighter.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    /// <summary>
    /// Performs code syntax highlighting.
    /// </summary>
    public class Highlighter
    {
        /// <summary>
        /// The pre-defined name for pattern used to detect token separators.
        /// </summary>
        private static readonly string SeparatorsPattern = "separators";

        /// <summary>
        /// The base path for loading transform definitions.
        /// </summary>
        private string transformPath;

        /// <summary>
        /// The debug options for diagnosing unexpected token transforms.
        /// </summary>
        private Options options;

        /// <summary>
        /// The current transform definition for the current code block type.
        /// </summary>
        private TransformDefinition transformDefinition;

        /// <summary>
        /// The transform buffer.
        /// </summary>
        private Buffer buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Highlighter"/> class.
        /// </summary>
        public Highlighter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Highlighter"/> class.
        /// </summary>
        /// <param name="transformBasePath">The base path for loading transform definitions.</param>
        public Highlighter(string transformBasePath)
        {
            this.transformPath = transformBasePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Highlighter"/> class.
        /// </summary>
        /// <param name="transformBasePath">The base path for loading transform definitions.</param>
        /// <param name="highlightOptions">The debug options for diagnosing unexpected token transforms.</param>
        public Highlighter(string transformBasePath, Options highlightOptions)
        {
            this.transformPath = transformBasePath;
            this.options = highlightOptions;
        }

        /// <summary>
        /// Transform the code by adding highlight markup and styles to recognized tokens.
        /// </summary>
        /// <param name="code">The code block to transform.</param>
        /// <param name="type">The code block type.</param>
        /// <returns>The transformed block.</returns>
        public string Transform(string code, string type)
        {
            this.transformDefinition = TransformDefinitionFactory.Load(
                this.transformPath, type, this.options);

            this.buffer = new Buffer(
                code, this.transformDefinition.Patterns[SeparatorsPattern], this.options);

            while (!this.buffer.Eof)
            {
                string test = this.buffer.Data.Substring(this.buffer.Position, this.buffer.Next - this.buffer.Position);
                if (!this.transformDefinition.Apply(this.buffer))
                {
                    this.buffer.SkipToken();
                }

                this.buffer.NextToken();
            }

            return this.buffer.Data;
        }
    }
}
