using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using TagsCloudVisualization.Extensions;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MultithreadedAlignmentLayouter : IRectangleLayouter
    {
        private static PointF Center => PointF.Empty;

        private static PointF PutNextRectangle(SizeF rectangleSize, IList<RectangleF> existingRectangles,
            ISet<PointF> existingVertices)
        {
            var res = new RectangleF(PointF.Empty, rectangleSize);
            if (existingRectangles.Count == 0)
            {
                var halfSize = new SizeF(rectangleSize.Width / 2, rectangleSize.Height / 2);
                res.Location = PointF.Subtract(Center, halfSize);
            }
            else
                res.Location =
                    ChooseLocationForRectangle(rectangleSize, existingRectangles, existingVertices);

            existingRectangles.Add(res);
            foreach (var vertex in res.Vertices())
                existingVertices.Add(vertex);

            return res.Location;
        }

        private static PointF ChooseLocationForRectangle(SizeF rectangleSize,
            IList<RectangleF> existingRectangles, ICollection<PointF> existingVertices)
        {
            var rect = new RectangleF(PointF.Empty, rectangleSize);
            if (existingVertices.Count < 50)
                return existingVertices
                    .SelectMany(point => rect.LocationsIfHadVertex(point))
                    .MinBy(p =>
                    {
                        rect.Location = p;
                        if (existingRectangles.Any(r => r.IntersectsWith(rect)))
                            return double.MaxValue;
                        return rect.MaxDistanceTo(Center);
                    });

            var numberOfLogicalProcessors = Environment.ProcessorCount;
            var toCheck = new ConcurrentQueue<PointF>();
            AppendAllAsync(toCheck,
                existingVertices
                    .SelectMany(point => rect.LocationsIfHadVertex(point)));
            var tasks = new List<Task<PointF?>>();
            var numbersOfHandledVertices = new int[numberOfLogicalProcessors];
            var totalVerticesNumber = existingVertices.Count * 4;
            foreach (var i in Enumerable.Range(0, numberOfLogicalProcessors))
            {
                var taskIndex = i;
                var task = Task<PointF?>.Factory.StartNew(() =>
                {
                    var localRectangle = new RectangleF(PointF.Empty, rectangleSize);
                    var minDist = double.MaxValue;
                    PointF? bestPoint = null;
                    while (numbersOfHandledVertices.Sum() != totalVerticesNumber)
                    {
                        if (!toCheck.TryDequeue(out var curPoint))
                            continue;
                        numbersOfHandledVertices[taskIndex]++;
                        minDist = TryImprovePoint(existingRectangles, localRectangle, curPoint, minDist, ref bestPoint);
                    }
                    return bestPoint;
                });
                tasks.Add(task);
            }
            return tasks
                .Select(task => task.Result)
                .Where(point => point != null)
                .Cast<PointF>()
                .MinBy(point => point.DistanceTo(Center));
        }

        private static double TryImprovePoint(IList<RectangleF> existingRectangles, RectangleF localRectangle, PointF curPoint,
            double minDist, ref PointF? bestPoint)
        {
            localRectangle.Location = curPoint;
            if (existingRectangles.Any(r => r.IntersectsWith(localRectangle))) return minDist;
            var curDist = localRectangle.MaxDistanceTo(Center);
            if (!(curDist < minDist)) return minDist;
            minDist = curDist;
            bestPoint = curPoint;
            return minDist;
        }

        private static void AppendAllAsync<T>(ConcurrentQueue<T> queue, IEnumerable<T> values)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var pointF in values)
                    queue.Enqueue(pointF);
            });
        }

        public IEnumerable<PointF> LayoutRectangles(IEnumerable<SizeF> sizes)
        {
            IList<RectangleF> existingRectangles = new List<RectangleF>();
            ISet<PointF> existingVertices = new HashSet<PointF>();
            return sizes
                .Select(size => PutNextRectangle(size, existingRectangles, existingVertices));
        }
    }
}
