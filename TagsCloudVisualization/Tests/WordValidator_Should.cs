using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Implementations;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class WordValidator_Should
    {
        [Test]
        public void IsValid_ShouldReturnTrue_IfNoWordsExcluded()
        {
            var validator = new WordValidator();

            validator.IsValid("word").Should().BeTrue();
        }

        [Test]
        public void IsValid_ShouldReturnFalseOnWordsExcludedInConstructor()
        {
            var badWord = "badword";
            var validator = new WordValidator(new []{badWord});

            validator.IsValid(badWord).Should().BeFalse();
        }

        [Test]
        public void IsValid_ShouldReturnFalseOnWordsExcludedWithMethod()
        {
            var badWord = "badword";
            var validator = new WordValidator();
            validator.AddExcludedWord(badWord);

            validator.IsValid(badWord).Should().BeFalse();
        }

        [Test]
        public void IsValid_ShouldIgnoreCase()
        {
            var badWord = "badword";
            var capsBadWord = badWord.ToUpper();
            var validator = new WordValidator(new []{badWord});

            validator.IsValid(capsBadWord).Should().BeFalse();
        }
    }
}
