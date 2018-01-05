using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MoreLinq;
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

            var wordsLimit = Result.Of(
                () =>
                {
                    if (Options.WordsToTake < 0 || Options.WordsToTake > 5000)
                        throw new ArgumentException();
                    return Options.WordsToTake;
                },
                $"Words number limit should be not negative and not greater than 5000 but was {Options.WordsToTake}");

            var wordLengthLimit = Result.Of(
                () =>
                {
                    if (Options.WordLengthLimit < 0 || Options.WordLengthLimit > 500)
                        throw new ArgumentException();
                    return Options.WordLengthLimit;
                },
                $"Words length limit should be not negative and not greater than 500 but was {Options.WordLengthLimit}");

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
                pen,
                wordsLimit,
                wordLengthLimit
            })
            {
                if (result.IsSuccess) continue;
                WriteError(result.Error);
                anyErrors = true;
            }
            if (anyErrors)
                return;

            Containter.Register(
                Component.For<ICloudLayouter>().ImplementedBy<CloudLayouter>(),
                Component.For<IWordLoader>().ImplementedBy<TextFileExtractingWordLoader>(),
                Component.For<IWordSizeCalculator>().Instance(new SizeCalculator(Options.MinPointSize)),
                Component.For<IRectangleLayouter>().ImplementedBy<MultithreadedAlignmentLayouter>(),
                Component.For<IWordValidator>().UsingFactoryMethod(() => new WordValidator(
                    excludedWordsPath.Value == null
                        ? Enumerable.Empty<string>()
                        : Containter.Resolve<IWordLoader>().LoadWords(excludedWordsPath.Value,
                            excludedfWordsFileEncoding.Value),
                    wordLengthLimit.Value)),
                Component.For<IWordPreparer>().ImplementedBy<WordPreparer>(),
                Component.For<IMarginCalculator>()
                    .Instance(new RelativeMarginCalculator(Options.MarginToSizeCoefficient))
            );

            Run(fontFamily.Value, FontStyle.Regular, brush.Value, pen.Value, imageFormat.Value,
                inputEncoding.Value, wordsLimit.Value);

            Containter.Dispose();
        }

        private static void WriteError(string errorMessage)
        {
            Console.Error.WriteLine("Error:\n\t" + errorMessage);
        }

        private static void Run(FontFamily fontFamily, FontStyle fontStyle, Brush brush, Pen pen,
            ImageFormat imageFormat, Encoding encoding, int? wordsLimit)
        {
            var cloudLayouter = Containter.Resolve<ICloudLayouter>();
            var wordsLoader = Containter.Resolve<IWordLoader>();
            var wordPreparer = Containter.Resolve<IWordPreparer>();
            var wordValidator = Containter.Resolve<IWordValidator>();
            var wordSizeCalculator = Containter.Resolve<IWordSizeCalculator>();

            var words = wordsLoader
                .LoadWords(Options.InputFilePath, encoding)
                .Select(wordPreparer.PrepareWord)
                .Where(wordValidator.IsValid)
                .ToArray();
            IEnumerable<Tuple<string, int>> uniqWordsAndFrequencies = FrequencyCounter.CountFrequencies(words);

            if (wordsLimit != null)
                uniqWordsAndFrequencies = uniqWordsAndFrequencies.Take((int) wordsLimit);

            var pointSizes =
                wordSizeCalculator.CalculatePointSizes(uniqWordsAndFrequencies.Select(t => t.Item2).ToArray());
            var wordsAndSizes = uniqWordsAndFrequencies.Zip(pointSizes, (wordAndFrequency, size) =>
                new KeyValuePair<string, float>(wordAndFrequency.Item1, size)).ToDictionary();

            var bitmap = cloudLayouter.Layout(wordsAndSizes, Options.MarginToSizeCoefficient,
                StringFormat.GenericTypographic, fontFamily, fontStyle, brush, pen);
            if (!bitmap.IsSuccess)
            {
                WriteError(bitmap.Error);
                return;
            }
            var image = bitmap.Value.DrawLayout();
            if (!image.IsSuccess)
            {
                WriteError(image.Error);
                return;
            }
            image.Value.Save(Options.OutputFilePath, imageFormat);
        }
    }
}
