using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TagsCloudVisualization.Implementations;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization
{
    public static class Program
    {
        private static readonly WindsorContainer Containter = new WindsorContainer();
        private static readonly Options Options = new Options();

        public static void Main(string[] args)
        {
            var isValid = CommandLine.Parser.Default.ParseArguments(args, Options);


            if (!isValid ||
                !IsValidInputPath(Options.InputFilePath) ||
                !IsValidOutputPath(Options.OutputFilePath) ||
                !TryParseFontFamily(out var fontFamily) ||
                !TryParseEncoding(out var encoding, Options.Encoding) ||
                !TryParseBrush(out var brush) ||
                !TryParseImageFormat(out var imageFormat) ||
                !TryParsePen(out var pen) ||
                !TryParseEncoding(out var excludedfWordsFileEncoding, Options.ExcludedWordsFileEncoding) ||
                !(string.IsNullOrEmpty(Options.ExcludedWordsFilePath) ||
                  IsValidInputPath(Options.ExcludedWordsFilePath))
            )
                return;
            foreach (var property in Options.GetType().GetProperties())
            {
                Console.WriteLine("{0} = {1}", property.Name, property.GetValue(Options));
            }
            Console.WriteLine(string.Concat(Enumerable.Repeat('=', Console.WindowWidth)));

            Containter.Register(
                Component.For<ICloudLayouter>().ImplementedBy<CircularCloudLayouter>(),
                Component.For<IWordLoader>().ImplementedBy<TextFileExtractingWordLoader>(),
                Component.For<IWordFrequencyCounter>().ImplementedBy<FrequencyCounter>(),
                Component.For<IWordSizeCalculator>().Instance(new SizeCalculator(Options.MinPointSize)),
                Component.For<IRectangleLayouter>().ImplementedBy<MultithreadedAlignmentLayouter>(),
                Component.For<IWordValidator>().UsingFactoryMethod(() => new WordValidator(
                    string.IsNullOrEmpty(Options.ExcludedWordsFilePath)
                        ? Enumerable.Empty<string>()
                        : Containter.Resolve<IWordLoader>().LoadWords(Options.ExcludedWordsFilePath, excludedfWordsFileEncoding))),
                Component.For<IWordPreparer>().ImplementedBy<WordPreparer>(),
                Component.For<IMarginCalculator>().Instance(new RelativeMarginCalculator(Options.MarginToSizeCoefficient))
            );

            Run(fontFamily, FontStyle.Regular, brush, pen, imageFormat, encoding);

            Containter.Dispose();
        }

        private static bool TryParsePen(out Pen pen)
        {
            try
            {
                pen = new Pen(Color.FromName(Options.OutlineColorName));
                return true;
            }
            catch (ArgumentException)
            {
                WriteError("Invalid fill color.");
                pen = null;
                return false;
            }
        }

        private static bool IsValidOutputPath(string outputFileName)
        {
            try
            {
                new FileStream(outputFileName, FileMode.Append).Dispose();
            }
            catch (Exception)
            {
                try
                {
                    File.Create(outputFileName).Dispose();
                    File.Delete(outputFileName);
                }
                catch (Exception)
                {
                    WriteError("Directory does not exist or write permission denied.");
                    return false;
                }
            }
            return true;
        }

        private static bool IsValidInputPath(string inputFileName)
        {
            try
            {
                File.OpenRead(inputFileName).Dispose();
            }
            catch (Exception)
            {
                WriteError("File does not exits or read permission denied.");
                return false;
            }
            return true;
        }

        private static bool TryParseImageFormat(out ImageFormat imageFormat)
        {
            try
            {
                imageFormat = (ImageFormat) typeof(ImageFormat)
                    .GetProperties()
                    .First(property =>
                        property.PropertyType == typeof(ImageFormat) &&
                        property.Name.Equals(Options.ImageFormat, StringComparison.OrdinalIgnoreCase))
                    .GetValue(null);
                return true;
            }
            catch (ArgumentException)
            {
                WriteError("Invalid fill color.");
                imageFormat = null;
                return false;
            }
        }

        private static bool TryParseBrush(out Brush brush)
        {
            try
            {
                brush = new SolidBrush(Color.FromName(Options.FillColorName));
                return true;
            }
            catch (ArgumentException)
            {
                WriteError("Invalid fill color.");
                brush = null;
                return false;
            }
        }

        private static bool TryParseEncoding(out Encoding encoding, string encodingName)
        {
            try
            {
                encoding = Encoding.GetEncoding(encodingName);
                return true;
            }
            catch (ArgumentException)
            {
                WriteError("Invalid encoding.");
                encoding = null;
                return false;
            }
        }

        private static bool TryParseFontFamily(out FontFamily fontFamily)
        {
            try
            {
                fontFamily = new FontFamily(Options.FontFamily);
                return true;
            }
            catch (ArgumentException)
            {
                WriteError("Invalid font.");
                fontFamily = null;
                return false;
            }
        }

        private static void WriteError(string errorMessage)
        {
            Console.Error.WriteLine("Error:\n\t" + errorMessage);
        }

        private static void Run(FontFamily fontFamily, FontStyle fontStyle, Brush brush, Pen pen, ImageFormat imageFormat, Encoding encoding)
        {
            var cloudLayouter = Containter.Resolve<ICloudLayouter>();
            var wordsLoader = Containter.Resolve<IWordLoader>();
            var wordPreparer = Containter.Resolve<IWordPreparer>();
            var wordValidator = Containter.Resolve<IWordValidator>();

            var words = wordsLoader
                .LoadWords(Options.InputFilePath, encoding)
                .Select(wordPreparer.PrepareWord)
                .Where(wordValidator.IsValid)
                .ToArray();
            var bitmap = cloudLayouter.Layout(words, Options.WordsToTake, Options.MarginToSizeCoefficient,
                StringFormat.GenericTypographic, fontFamily, fontStyle, brush, pen);
            bitmap.Save(Options.OutputFilePath, imageFormat);
        }
    }
}
