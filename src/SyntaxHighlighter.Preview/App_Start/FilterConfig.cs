//-----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Preview
{
    using System.Web.Mvc;

    /// <summary>
    /// Configures global application filters.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Adds filters to the global filter collection.
        /// </summary>
        /// <param name="filters">The global filter collection.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
