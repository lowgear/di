using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Implementations;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class TextFileLineByLineWordLoader_Should
    {
        private const string TestFileName = "testFile.txt";

        [TestCase("utf-16")]
        [TestCase("utf-8")]
        [TestCase("windows-1251")]
        [TestCase("ascii")]
        public void LoadWords_ShouldfLoadWordsAsTheyAre(string encodingName)
        {
            var encoding = Encoding.GetEncoding(encodingName);
            var loader = new TextFileLineByLineWordLoader();
            var words = new[] {"word1", "  word2  ", "\taBacAba\t", ""};
            File.WriteAllLines(TestFileName, words, encoding);

            loader.LoadWords(TestFileName, encoding).ShouldAllBeEquivalentTo(words, options => options.WithStrictOrdering());
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(TestFileName))
                File.Delete(TestFileName);
        }
    }
}
