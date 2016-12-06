//-----------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Preview
{
    using System.Web.Optimization;

    /// <summary>
    /// Configures site asset bundling.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Register assets required by Bootstrap.
        /// </summary>
        /// <param name="bundles">The bundle collection.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
