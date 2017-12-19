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
using TagsCloudVisualization.util;

namespace TagsCloudVisualization
{
    public static class Program
    {
        private static readonly WindsorContainer Containter = new WindsorContainer();
        private static readonly Options Options = new Options();

        public static void Main(string[] args)
        {
            var isValid = CommandLine.Parser.Default.ParseArguments(args, Options);
            if (!isValid)
                return;


            var inputPath = Result.Of(
                () =>
                {
                    File.OpenRead(Options.InputFilePath).Dispose();
                    return Options.InputFilePath;
                },
                "Unable to read from input path. File does not exits or read permission denied.");

            var outputPath = Result.Of(
                () =>
                {
                    File.Create(Options.OutputFilePath).Dispose();
                    File.Delete(Options.OutputFilePath);
                    return Options.OutputFilePath;
                },
                "Unable to write to output path. Directory does not exist or write permission denied.");

            var excludedWordsPath = Result.Of(
                () =>
                {
                    if (string.IsNullOrEmpty(Options.ExcludedWordsFilePath))
                        return null;
                    File.OpenRead(Options.ExcludedWordsFilePath).Dispose();
                    return Options.ExcludedWordsFilePath;
                },
                "Unable to read from excluded words path. File does not exits or read permission denied.");

            var inputEncoding = Result.Of(
                () => Encoding.GetEncoding(Options.InputEncoding),
                $"No such encoding detected {Options.InputEncoding}");

            var excludedfWordsFileEncoding = Result.Of(
                () => Encoding.GetEncoding(Options.ExcludedWordsFileEncoding),
                $"No such encoding detected {Options.ExcludedWordsFileEncoding}");

            var imageFormat = Result.Of(
                () => (ImageFormat) typeof(ImageFormat)
                    .GetProperties()
                    .First(property =>
                        property.PropertyType == typeof(ImageFormat) &&
                        property.Name.Equals(Options.ImageFormat, StringComparison.OrdinalIgnoreCase))
                    .GetValue(null),
                $"No such image format detected {Options.ImageFormat}");

            var fontFamily = Result.Of(
                () => new FontFamily(Options.FontFamily),
                $"No such font family detected {Options.FontFamily}");

            var brush = Result.Of(
                () =>
                {
                    if (!Enum.TryParse<KnownColor>(Options.FillColorName, true, out var _))
                        throw new ArgumentException();
                    return new SolidBrush(Color.FromName(Options.FillColorName));
                },
                $"No such color detected {Options.FillColorName}");

            var pen = Result.Of(
                () =>
                {
                    if (!Enum.TryParse<KnownColor>(Options.OutlineColorName, true, out var _))
                        throw new ArgumentException();
                    return new Pen(Color.FromName(Options.OutlineColorName));
                },
                $"No such color detected {Options.OutlineColorName}");

            foreach (var property in Options.GetType().GetProperties())
            {
                Console.WriteLine("{0} = {1}", property.Name, property.GetValue(Options));
            }
            Console.WriteLine(string.Concat(Enumerable.Repeat('=', Console.WindowWidth)));

            var anyErrors = false;
            foreach (var result in new IResult[]
            {
                inputPath,
                outputPath,
                excludedWordsPath,
                inputEncoding,
                excludedfWordsFileEncoding,
                imageFormat,
                fontFamily,
                brush,
                pen
            })
            {
                if (result.IsSuccess) continue;
                WriteError(result.Error);
                anyErrors = true;
            }
            if (anyErrors)
                return;

            Containter.Register(
                Component.For<ICloudLayouter>().ImplementedBy<CircularCloudLayouter>(),
                Component.For<IWordLoader>().ImplementedBy<TextFileExtractingWordLoader>(),
                Component.For<IWordFrequencyCounter>().ImplementedBy<FrequencyCounter>(),
                Component.For<IWordSizeCalculator>().Instance(new SizeCalculator(Options.MinPointSize)),
                Component.For<IRectangleLayouter>().ImplementedBy<MultithreadedAlignmentLayouter>(),
                Component.For<IWordValidator>().UsingFactoryMethod(() => new WordValidator(
                    excludedWordsPath.Value == null
                        ? Enumerable.Empty<string>()
                        : Containter.Resolve<IWordLoader>().LoadWords(excludedWordsPath.Value,
                            excludedfWordsFileEncoding.Value))),
                Component.For<IWordPreparer>().ImplementedBy<WordPreparer>(),
                Component.For<IMarginCalculator>()
                    .Instance(new RelativeMarginCalculator(Options.MarginToSizeCoefficient))
            );

            Run(fontFamily.Value, FontStyle.Regular, brush.Value, pen.Value, imageFormat.Value,
                inputEncoding.Value);

            Containter.Dispose();
        }

        private static void WriteError(string errorMessage)
        {
            Console.Error.WriteLine("Error:\n\t" + errorMessage);
        }

        private static void Run(FontFamily fontFamily, FontStyle fontStyle, Brush brush, Pen pen,
            ImageFormat imageFormat, Encoding encoding)
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
            if (!bitmap.IsSuccess)
            {
                WriteError(bitmap.Error);
                return;
            }
            bitmap.Value.Save(Options.OutputFilePath, imageFormat);
        }
    }
}
