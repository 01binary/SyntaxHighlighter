//-----------------------------------------------------------------------
// <copyright file="TransformFactory.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Loads transforms.
    /// </summary>
    public class TransformFactory
    {
        /// <summary>
        /// The JSON key for transform type that determines which class to instantiate.
        /// </summary>
        private const string TransformTypeKey = "transformType";

        /// <summary>
        /// The JSON key for the class name to use on the span element wrapping the token.
        /// </summary>
        private const string TokenClassKey = "tokenClass";

        /// <summary>
        /// The JSON key for the pattern name from patterns in transform definition.
        /// </summary>
        private const string TokenPatternKey = "tokenPattern";

        /// <summary>
        /// The JSON key for the pattern name that must match the previous token or separator.
        /// </summary>
        private const string ModifierPatternKey = "modifierPattern";

        /// <summary>
        /// The JSON key for token classes the token cannot be preceded by to match.
        /// </summary>
        private const string ExcludeClassesKey = "excludeClasses";

        /// <summary>
        /// The JSON key for the transforms inside a token span.
        /// </summary>
        private const string TransformsKey = "transforms";

        /// <summary>
        /// The JSON key for the transform name debug information.
        /// </summary>
        private const string NameKey = "name";

        /// <summary>
        /// The JSON key for the transform description debug information.
        /// </summary>
        private const string DescriptionKey = "description";

        /// <summary>
        /// The basic token transform that specifies a pattern and a transformed token class.
        /// </summary>
        private const string TokenType = "Token";

        /// <summary>
        /// The transform that replaces large spans of text and supports nested transforms.
        /// </summary>
        private const string TokenSpanType = "TokenSpan";

        /// <summary>
        /// Loads a transform from JSON node.
        /// </summary>
        /// <param name="definition">The transform definition containing the loaded patterns.</param>
        /// <param name="node">The serialized JSON node to load from.</param>
        /// <param name="options">The code highlight options.</param>
        /// <returns>The loaded transform.</returns>
        public static ITransform Load(TransformDefinition definition, JObject node, Options options)
        {
            string transformType = node.Property(TransformTypeKey) != null ?
                node.Property(TransformTypeKey).Value.ToString() : null;

            string className = node.Property(TokenClassKey)
                .Value.ToString();

            Regex pattern = definition.Patterns[node.Property(TokenPatternKey)
                .Value.ToString()];

            string[] excludeClassNames = node[ExcludeClassesKey] != null ?
                node[ExcludeClassesKey].ToObject<string[]>() : new string[0];

            string name = null;
            string description = null;
            string patternName = null;
            string modifierPatternName = null;
            Regex modifierPattern = null;

            if (node[ModifierPatternKey] != null)
            {
                modifierPatternName = node[ModifierPatternKey].ToString();
                modifierPattern = definition.Patterns[modifierPatternName];
            }

            if (options.DebugInfo)
            {
                if (node.Property(NameKey) != null)
                {
                    name = node.Property(NameKey).Value.ToString();
                }

                if (node.Property(DescriptionKey) != null)
                {
                    description = node.Property(DescriptionKey).Value.ToString();
                }

                patternName = node.Property(TokenPatternKey).Value.ToString();
            }

            switch (transformType)
            {
                default:
                case TokenType:
                    {
                        return new TransformToken(
                            name,
                            description,
                            patternName,
                            pattern,
                            options.DebugInfo ? modifierPatternName : null,
                            modifierPattern,
                            className,
                            excludeClassNames);
                    }

                case TokenSpanType:
                    {
                        JArray transformNodes = node[TransformsKey] as JArray;
                        List<TransformToken> transforms = new List<TransformToken>();

                        if (transformNodes != null && transformNodes.Count > 0)
                        {
                            foreach (JObject transformNode in transformNodes)
                            {
                                transforms.Add(
                                    TransformFactory.Load(definition, transformNode, options) as TransformToken);
                            }
                        }

                        return new TransformTokenSpan(
                            name,
                            description,
                            patternName,
                            pattern,
                            modifierPatternName,
                            modifierPattern,
                            className,
                            transforms,
                            excludeClassNames);
                    }
            }
        }
    }
}
