using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TagsCloudVisualization.Implementations;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization
{
    public static class Program
    {
        private const string usage = @"Naval Fate.

    Usage:
      naval_fate.exe ship new <name>...
      naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
      naval_fate.exe ship shoot <x> <y>
      naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
      naval_fate.exe (-h | --help)
      naval_fate.exe --version

    Options:
      -h --help     Show this screen.
      --version     Show version.
      --speed=<kn>  Speed in knots [default: 10].
      --moored      Moored (anchored) mine.
      --drifting    Drifting mine.

    ";

        public static void Main(string[] args)
        {
            var containter = new WindsorContainer();
            containter.Register(Component.For<ICloudLayouter>().ImplementedBy<CircularCloudLayouter>());
            containter.Register(Component.For<IWordLoader>().ImplementedBy<TextFileLineByLineWordLoader>());
            containter.Register(Component.For<IWordFrequencyCounter>().ImplementedBy<FrequencyCounter>());
            containter.Register(Component.For<IWordSizeCalculator>().ImplementedBy<SizeCalculater>());
            containter.Register(Component.For<IRectangleLayouter>().ImplementedBy<AlignmentLayouter>());
            containter.Register(Component.For<FontFamily>().Instance(new FontFamily("Arial Narrow")));
            containter.Register(Component.For<Brush>().Instance(Brushes.Black));
            containter.Register(Component.For<Encoding>().Instance(Encoding.GetEncoding("Windows-1251")));
            containter.Register(Component.For<ImageFormat>().Instance(ImageFormat.Png));
            containter.Register(Component.For<Func<TextRenderingHint>>()
                .Instance(() => TextRenderingHint.AntiAliasGridFit));


            var cloudLayouter = containter.Resolve<ICloudLayouter>();
            var wordsLoader = containter.Resolve<IWordLoader>();
            var imageFormat = containter.Resolve<ImageFormat>();

            var words = wordsLoader.LoadWords(args[0]);
            var bitmap = cloudLayouter.Layout(words);
            bitmap.Save("result.png", imageFormat);
        }
    }
}
