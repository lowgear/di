using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;

namespace TagsCloudVisualization
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Options
    {
        [Option('i', "input",
            HelpText = "Input file with words for tag cloud.",
            Required = true)]
        public string InputFilePath { get; set; }

        [Option('o', "output",
            HelpText = "Output file with tag cloud image.",
            Required = true)]
        public string OutputFilePath { get; set; }

        [Option('e', "excluded",
            HelpText = "File with words which should not be present in tag cloud.")]
        public string ExcludedWordsFilePath { get; set; }

        [Option('f', "format",
            HelpText = "Output file image format.",
            DefaultValue = "png")]
        public string ImageFormat { get; set; }

        [Option("fillcolor",
            DefaultValue = "white",
            HelpText = "Text fill color.")]
        public string FillColorName { get; set; }

        [Option("outlinecolor",
            DefaultValue = "black",
            HelpText = "Text outline color.")]
        public string OutlineColorName { get; set; }

        [Option("fontFamily",
            DefaultValue = "arial",
            HelpText = "Text font famyly.")]
        public string FontFamily { get; set; }

        [Option("encoding",
            DefaultValue = "utf-16",
            HelpText = "Encoding of the input file.")]
        public string Encoding { get; set; }

        [Option('s', "size",
            DefaultValue = 100,
            HelpText = "Size in points of the smalest tag in cloud.")]
        public float MinPointSize { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
