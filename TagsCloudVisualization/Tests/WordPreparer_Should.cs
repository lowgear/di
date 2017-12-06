using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Implementations;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    class WordPreparer_Should
    {
        private readonly WordPreparer wordPreparer = new WordPreparer();

        [Test]
        public void ShouldMakeWordsLowerCase()
        {
            const string word = "WoRd";
            const string expected = "word";

            wordPreparer.PrepareWord(word).Should().Be(expected);
        }

        [Test]
        public void ShouldTrimWords()
        {
            const string word = "  \tword\r  ";
            const string expected = "word";

            wordPreparer.PrepareWord(word).Should().Be(expected);
        }
    }
}