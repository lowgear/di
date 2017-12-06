using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using Moq;
using TagsCloudVisualization.Implementations;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        [Test]
        public void WordFrequencyCounter_ShouldReceiveExactlySameWordsAsWereGivenToLayouter()
        {
            var words = new[] {"word1", "word2", "word1", "word3", "word4"};
            var mock = new Mock<IWordFrequencyCounter>();
            mock.Setup(counter => counter.CountFrequencies(It.IsAny<IEnumerable<string>>()))
                .Returns(new[] {Tuple.Create("word", 1)});
            var frequencyCounter = mock.Object;
            var sizeCalculatorMock = new Mock<IWordSizeCalculator>();
            sizeCalculatorMock.Setup(calculator => calculator.CalculatePointSizes(new[] {1})).Returns(new []{1.0F});
            var rectMockLayouterMock = new Mock<IRectangleLayouter>();
            rectMockLayouterMock.Setup(calculator => calculator.LayoutRectangles(It.IsAny<IEnumerable<SizeF>>())).Returns(new []{PointF.Empty, });
            var marginCalculatorMock = new Mock<IMarginCalculator>();
            marginCalculatorMock.Setup(calculator => calculator.CalculateBounds(It.IsAny<RectangleF>())).Returns(new RectangleF(0,0, 1, 1));

            var layouter = new CircularCloudLayouter(frequencyCounter, sizeCalculatorMock.Object, rectMockLayouterMock.Object, marginCalculatorMock.Object);
            layouter.Layout(words, 0, default(int), StringFormat.GenericTypographic,
                FontFamily.GenericMonospace, FontStyle.Regular, Brushes.AliceBlue, Pens.AliceBlue);

            mock.Verify(counter => counter.CountFrequencies(words), Times.Once);
            mock.Verify(counter => counter.CountFrequencies(It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
