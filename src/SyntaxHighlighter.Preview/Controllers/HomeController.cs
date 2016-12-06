//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Preview.Controllers
{
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// The main application controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The main application page used to preview transform results.
        /// </summary>
        /// <returns>The Index view.</returns>
        public ActionResult Index()
        {
            string sourcePath = Server.MapPath("~/bin/App_Data/Input.txt");
            string destPath = Server.MapPath("~/bin/App_Data/Output.htm");

            DebugOptions options = new DebugOptions()
            {
                BreakOnToken = null, // "services",
                BreakOnNotClass = null // "pl-v"
            };

            string transformResult = new Highlighter(Server.MapPath("~/bin/App_Data"), options)
                .Transform(System.IO.File.ReadAllText(sourcePath), "csharp");

            System.IO.File.WriteAllText(destPath, transformResult);

            ViewBag.Markdown = new HtmlString(transformResult);

            return this.View();
        }
    }
}