//-----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Preview
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Configures application routing.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Adds application routes to the route collection.
        /// </summary>
        /// <param name="routes">The route collection.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Configure the default MVC route template.
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
