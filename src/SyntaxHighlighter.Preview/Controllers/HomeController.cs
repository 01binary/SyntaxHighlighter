//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="01 Binary">
//     Copyright (C) 01 Binary.
// </copyright>
//-----------------------------------------------------------------------

namespace SyntaxHighlighter.Preview.Controllers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// The main application controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The path where input files and transform definitions are copied.
        /// </summary>
        private static readonly string InputPath = "~/bin/App_Data";
        
        /// <summary>
        /// The path for outputting transform results for review.
        /// </summary>
        private static readonly string OutputPath = "~/App_Data";

        /// <summary>
        /// The transform definition file name format.
        /// </summary>
        private static readonly string TransformFormat = "{0}.json";

        /// <summary>
        /// The input file name format.
        /// </summary>
        private static readonly string InputFormat = "{0}-input.txt";

        /// <summary>
        /// The output file name format.
        /// </summary>
        private static readonly string OutputFormat = "{0}-output.htm";

        /// <summary>
        /// The data file name format.
        /// </summary>
        private static readonly string DataFormat = "~/bin/App_Data/{0}";

        /// <summary>
        /// The route used to request transform definitions.
        /// </summary>
        private static readonly string TransformRequestFormat = "/Home/Data?file={0}";

        /// <summary>
        /// The default language to preview.
        /// </summary>
        private static readonly string DefaultLanguage = "csharp";

        /// <summary>
        /// The main application page used to preview transform results.
        /// </summary>
        /// <param name="language">The code highlight language to preview.</param>
        /// <returns>The Index view.</returns>
        public ActionResult Index(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                language = DefaultLanguage;
            }

            string sourcePath = Server.MapPath(
                Path.Combine(InputPath, string.Format(InputFormat, language)));

            string destPath =
                Server.MapPath(Path.Combine(OutputPath, string.Format(OutputFormat, language)));

            Options options = new Options()
            {
                DebugInfo = true
            };

            string transformFileName = string.Format(TransformFormat, language);
            string transformResult = new Highlighter(Server.MapPath(InputPath), options)
                .Transform(System.IO.File.ReadAllText(sourcePath), language);

            System.IO.File.WriteAllText(destPath, transformResult);

            ViewBag.Title = language;
            ViewBag.HighlightOutput = new HtmlString(transformResult);
            ViewBag.TransformDefinition = TransformDefinitionFactory.Load(
                InputPath, language, options);
            ViewBag.TransformPath = string.Format(
                TransformRequestFormat, transformFileName);

            return this.View();
        }

        /// <summary>
        /// Serve a static file from App_Data.
        /// </summary>
        /// <param name="file">The path relative to App_Data folder.</param>
        /// <returns>The file content result.</returns>
        public ActionResult Data(string file)
        {
            string serverPath = Server.MapPath(string.Format(DataFormat, file));
            byte[] data = System.IO.File.ReadAllBytes(serverPath);
            string contentType = MimeMapping.GetMimeMapping(serverPath);

            return this.File(data, contentType);
        }
    }
}