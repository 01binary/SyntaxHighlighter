﻿//-----------------------------------------------------------------------
// <copyright file="TransformDefinitionFactory.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Loads transform definitions.
    /// </summary>
    public class TransformDefinitionFactory
    {
        /// <summary>
        /// The loaded transform definitions.
        /// </summary>
        private static Dictionary<string, TransformDefinition> definitions =
            new Dictionary<string, TransformDefinition>();

        /// <summary>
        /// Loads a transform definition if not already loaded.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="name">The transform definition name.</param>
        /// <param name="options">The syntax highlight options.</param>
        /// <returns>The loaded transform definition.</returns>
        public static TransformDefinition Load(string basePath, string name, Options options)
        {
            if (!definitions.ContainsKey(name))
            {
                definitions[name] = TransformDefinition.Load(
                    Path.Combine(basePath, string.Format("{0}.json", name)),
                    options);
            }

            return definitions[name];
        }
    }
}
