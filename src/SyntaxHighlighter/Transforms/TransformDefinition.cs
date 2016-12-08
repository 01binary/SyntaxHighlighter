//-----------------------------------------------------------------------
// <copyright file="TransformDefinition.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines transforms and patterns.
    /// </summary>
    public class TransformDefinition
    {
        /// <summary>
        /// The JSON key for named patterns used in transforms.
        /// </summary>
        private const string PatternsKey = "patterns";

        /// <summary>
        /// The JSON key for pattern string if the pattern includes options.
        /// </summary>
        private const string PatternKey = "pattern";

        /// <summary>
        /// The JSON key for explicit capture pattern option.
        /// </summary>
        private const string ExplicitKey = "explicit";

        /// <summary>
        /// The JSON key for the ordered array of transforms in the transform definition.
        /// </summary>
        private const string TransformsKey = "transforms";

        /// <summary>
        /// The transform options.
        /// </summary>
        private Options options;

        /// <summary>
        /// The transform patterns.
        /// </summary>
        private Dictionary<string, Regex> patterns;

        /// <summary>
        /// The transforms to apply.
        /// </summary>
        private List<ITransform> transforms;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformDefinition"/> class.
        /// </summary>
        public TransformDefinition()
        {
            this.patterns = new Dictionary<string, Regex>();
            this.transforms = new List<ITransform>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformDefinition"/> class.
        /// </summary>
        /// <param name="options">The syntax highlight options.</param>
        public TransformDefinition(Options options)
        {
            this.options = options;
            this.patterns = new Dictionary<string, Regex>();
            this.transforms = new List<ITransform>();
        }

        /// <summary>
        /// Gets the transform patterns.
        /// </summary>
        public IDictionary<string, Regex> Patterns
        {
            get
            {
                return this.patterns;
            }
        }

        /// <summary>
        /// Gets the transforms.
        /// </summary>
        public IList<ITransform> Transforms
        {
            get
            {
                return this.transforms;
            }
        }

        /// <summary>
        /// Loads a transform definition from file.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <param name="options">The syntax highlight options.</param>
        /// <returns>The loaded transform definition.</returns>
        public static TransformDefinition Load(string path, Options options)
        {
            JObject source = JObject.Parse(File.ReadAllText(path));
            JObject patterns = source[PatternsKey] as JObject;
            JArray transforms = source[TransformsKey] as JArray;

            TransformDefinition definition = new TransformDefinition(options);

            RegexOptions patternOptions =
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;

            foreach (JProperty pattern in patterns.Properties())
            {
                if (pattern.Value is JObject)
                {
                    JObject patternNode = pattern.Value as JObject;

                    bool explicitCapture = patternNode[ExplicitKey] != null ?
                        patternNode[ExplicitKey].Value<bool>() : false;

                    definition.Patterns[pattern.Name] = new Regex(
                        patternNode[PatternKey].ToString(),
                        explicitCapture ? patternOptions | RegexOptions.ExplicitCapture : patternOptions);
                }
                else
                {
                    definition.Patterns[pattern.Name] = new Regex(
                        pattern.Value.ToString(), patternOptions);
                }
            }

            foreach (JObject transform in transforms.Children())
            {
                definition.transforms.Add(
                    TransformFactory.Load(definition, transform, options));
            }

            return definition;
        }

        /// <summary>
        /// Applies the text transformations to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to transform.</param>
        /// <returns>Whether the transformation took place.</returns>
        public bool Apply(Buffer buffer)
        {
            foreach (ITransform transform in this.transforms)
            {
                if (transform.Apply(buffer, this.options))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
