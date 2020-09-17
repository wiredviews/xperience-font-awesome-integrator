using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExCSS;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Xperience.FontAwesomeIntegrator
{
    public class Program
    {
        [Option(
            "-cpath|--cms-path",
            CommandOptionType.SingleValue,
            Description = @"Full path to 'CMS' folder")]
        public (bool hasValue, string value) CMSPath { get; }

        [Option(
            "-fpath|--font-awesome-path",
            CommandOptionType.SingleValue,
            Description = @"Full path to content folder extracted from fontawesome-**-web.zip")]
        public (bool hasValue, string value) FontAwesomePath { get; }

        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        public void OnExecute()
        {
            if (!CMSPath.hasValue)
            {
                throw new Exception($"Must specify a {nameof(CMSPath)} value -c");
            }

            if (!FontAwesomePath.hasValue)
            {
                throw new Exception($"Must specify a {nameof(FontAwesomePath)} value -f");
            }

            var cmsThemePath = Fluent.IO.Path.Get(CMSPath.value, "App_Themes", "Default");
            var fontAwesomePath = Fluent.IO.Path.Get(FontAwesomePath.value);

            var customPath = cmsThemePath
                .CreateDirectory()
                .CreateSubDirectory("Custom");

            fontAwesomePath
                .Directories(d => d.FileName == "webfonts")
                .Copy(customPath.CreateSubDirectory("webfonts"));

            fontAwesomePath
                .Files(f => f.FileName == "LICENSE.txt")
                .Copy(customPath);

            string iconsMeta = fontAwesomePath
                .Directories(d => d.FileName == "metadata")
                .First()
                .Files(f => f.FileName == "icons.json").FullPath;

            string cssClassManifest = customPath
                .CreateFile("icon-css-classes.txt", string.Empty)
                .FullPath;

            ProcessFiles(
                iconsMeta,
                cssClassManifest,
                CreateIconManifest);

            string allCss = fontAwesomePath
                .Directories(d => d.FileName == "css")
                .First()
                .Files(f => f.FileName == "all.min.css")
                .First()
                .FullPath;

            string styleCss = customPath
                .CreateFile("style.css", string.Empty)
                .FullPath;

            ProcessFiles(
                allCss,
                styleCss,
                UpdateCSSFile);
        }

        /// <summary>
        /// Accepts and input file stream and output file stream and performs the given action on them
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <param name="processor"></param>
        private void ProcessFiles(string inputPath, string outputPath, Action<StreamReader, StreamWriter> processor)
        {
            using var inputStream = new StreamReader(inputPath);
            using var outputStream = new StreamWriter(outputPath);

            processor(inputStream, outputStream);
        }

        /// <summary>
        /// Creates a manifest file, that Xperience can parse, from the metadata.json file
        /// supplied by Font Awesome
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        private void CreateIconManifest(StreamReader inputStream, StreamWriter outputStream)
        {
            string fileString = inputStream.ReadToEnd();

            var icons = JsonConvert.DeserializeObject<Dictionary<string, IconType>>(fileString);

            foreach (var icon in icons)
            {
                string iconName = icon.Key;

                foreach (string iconType in icon.Value.Styles)
                {
                    switch (iconType)
                    {
                        case "solid":
                            outputStream.WriteLine($"fas fa-{iconName}");
                            break;
                        case "regular":
                            outputStream.WriteLine($"far fa-{iconName}");
                            break;
                        case "light":
                            outputStream.WriteLine($"fal fa-{iconName}");
                            break;
                        case "duotone":
                            outputStream.WriteLine($"fad fa-{iconName}");
                            break;
                        case "brands":
                            outputStream.WriteLine($"fab fa-{iconName}");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Represents an Icon entry in the Font Awesome icon metadata JSON file
        /// </summary>
        public class IconType
        {
            public string[] Styles { get; set; } = Enumerable.Empty<string>().ToArray();
        }

        /// <summary>
        /// Processes the Font Awesome CSS file, modifying selectors so icons
        /// display correctly in the Xperience Content Management application
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        private void UpdateCSSFile(StreamReader inputStream, StreamWriter outputStream)
        {
            var sheet = new StylesheetParser().Parse(inputStream.BaseStream);

            foreach (var node in sheet.Children)
            {
                switch (node)
                {
                    case StyleRule styleNode:
                        ReplaceSelectorText(styleNode);
                        break;
                    case IFontFaceRule fontRule:
                        ReplaceSrc(fontRule);
                        break;
                    default:
                        break;
                }
            }

            sheet.ToCss(outputStream, CompressedStyleFormatter.Instance);
        }

        /// <summary>
        /// Replaces the selector text for the main Font Awesome rules to be
        /// compatible with the more specific selectors in Xperience
        /// </summary>
        /// <param name="rule"></param>
        private void ReplaceSelectorText(StyleRule rule)
        {
            switch (rule.SelectorText)
            {
                // The free vs pro Font Awesome .zip exports order the selectors differently
                case ".fa,.fab,.fad,.fal,.far,.fas":
                case ".fa,.fas,.far,.fal,.fad,.fab":
                    rule.SelectorText = @".fa,.fas,.far,.fal,.fad,.fab,.cms-bootstrap [class^='icon-'].fa, .cms-bootstrap [class*=' icon-'].fa,.cms-bootstrap [class^='icon-'].fab, .cms-bootstrap [class*=' icon-'].fab,.cms-bootstrap [class^='icon-'].fad, .cms-bootstrap [class*=' icon-'].fad,.cms-bootstrap [class^='icon-'].fal, .cms-bootstrap [class*=' icon-'].fal,.cms-bootstrap [class^='icon-'].far, .cms-bootstrap [class*=' icon-'].far,.cms-bootstrap [class^='icon-'].fas, .cms-bootstrap [class*=' icon-'].fas";
                    break;

                case ".fab":
                    rule.SelectorText = @".fab,.cms-bootstrap [class^='icon-'].fab, .cms-bootstrap [class*=' icon-'].fab";
                    break;

                case ".fad":
                    rule.SelectorText = @".fad,.cms-bootstrap [class^='icon-'].fad, .cms-bootstrap [class*=' icon-'].fad";
                    break;

                case ".fal":
                    rule.SelectorText = @".fal,.cms-bootstrap [class^='icon-'].fal, .cms-bootstrap [class*=' icon-'].fal";
                    break;

                case ".fal,.far":
                    rule.SelectorText = @".fal,.far,.cms-bootstrap [class^='icon-'].fal, .cms-bootstrap [class*=' icon-'].fal,.cms-bootstrap [class^='icon-'].far, .cms-bootstrap [class*=' icon-'].far";
                    break;

                case ".far":
                    rule.SelectorText = @".far,.cms-bootstrap [class^='icon-'].far, .cms-bootstrap [class*=' icon-'].far";
                    break;

                case ".fa,.fas":
                    rule.SelectorText = @".fa,.fas,.cms-bootstrap [class^='icon-'].fa, .cms-bootstrap [class*=' icon-'].fa,.cms-bootstrap [class^='icon-'].fas, .cms-bootstrap [class*=' icon-'].fas";
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Unnests the src path for the font-face rules, since Font Awesome
        /// has them referencing a different relative path than they use in Xperience
        /// </summary>
        /// <param name="rule"></param>
        private void ReplaceSrc(IFontFaceRule rule) =>
            rule.SetProperty("src", rule.GetPropertyValue("src").Replace("(\"../webfonts", "(\"./webfonts"));
    }
}
