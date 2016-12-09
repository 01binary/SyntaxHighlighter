//-----------------------------------------------------------------------
// <copyright file="Shared.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Preview
{
    /// <summary>
    /// Shared functionality.
    /// </summary>
    public class Shared
    {
        /// <summary>
        /// Gets the transform display name for rendering.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>The transform display name.</returns>
        public static string GetTransformDisplayName(ITransform transform)
        {
            if (transform.GetType() == typeof(TransformToken))
            {
                return "tok";
            }
            else if (transform.GetType() == typeof(TransformTokenModifier))
            {
                return "mod";
            }
            else if (transform.GetType() == typeof(TransformTokenSpan))
            {
                return "spn";
            }

            return string.Empty;
        }
    }
}