using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BentlyOttman.Tests.TestInput;

namespace BentlyOttman.Tests
{
    [TestClass]
    public class SweepLineTests
    {

        /// <summary>
        /// Проверяет обход очереди событий
        /// То что не будет бесконечного цикла
        /// https://stackoverflow.com/questions/12145510/how-to-fail-a-test-that-is-stuck-in-an-infinite-loop
        /// </summary>
        [TestMethod]
        //[Timeout(1000)]
        public void TraverseEventQueueTest()
        {
            SweepLine SL = new SweepLine(TestingColl1);
            SL.TraverseEventQueue();
            Assert.IsTrue(SL.EQ.Count == 0);
        }

        /// <summary>
        /// Добавление в набор сканирующей линии множества линий и проверка порядка их размещения в SortedSet
        /// </summary>
        [TestMethod]
        public void AboveBelowComparerTest()
        {
            SweepLine SL = new SweepLine(TestingColl1);
            //1
            SL.X = 130;
            SL.SLIntersectingLines.Add(testLine2);
            SL.SLIntersectingLines.Add(testLine3);
            SL.SLIntersectingLines.Add(testLine4);
            SL.SLIntersectingLines.Add(testLine5);
            SL.SLIntersectingLines.Add(testLine7);
            SL.SLIntersectingLines.Add(testLine8);
            SL.SLIntersectingLines.Add(testLine9);

            ILine line0 = SL.SLIntersectingLines.ElementAt(0);
            ILine line1 = SL.SLIntersectingLines.ElementAt(1);
            ILine line2 = SL.SLIntersectingLines.ElementAt(2);
            ILine line3 = SL.SLIntersectingLines.ElementAt(3);
            ILine line4 = SL.SLIntersectingLines.ElementAt(4);
            ILine line5 = SL.SLIntersectingLines.ElementAt(5);

            Assert.AreEqual(testLine5, line0);
            Assert.AreEqual(testLine7, line1);
            Assert.AreEqual(testLine9, line2);
            Assert.IsTrue( testLine2.Equals( line3)|| testLine3.Equals(line3));
            Assert.IsTrue(testLine4.Equals(line5)|| testLine8.Equals(line5));

            //2
            SL.SLIntersectingLines.Clear();
            SL.X = 110;
            SL.SLIntersectingLines.Add(testLine1);
            SL.SLIntersectingLines.Add(testLine2);
            SL.SLIntersectingLines.Add(testLine3);
            SL.SLIntersectingLines.Add(testLine4);
            SL.SLIntersectingLines.Add(testLine5);
            SL.SLIntersectingLines.Add(testLine6);
            SL.SLIntersectingLines.Add(testLine7);


            line0 = SL.SLIntersectingLines.ElementAt(0);
            line1 = SL.SLIntersectingLines.ElementAt(1);
            line2 = SL.SLIntersectingLines.ElementAt(2);
            line3 = SL.SLIntersectingLines.ElementAt(3);
            line4 = SL.SLIntersectingLines.ElementAt(4);
            line5 = SL.SLIntersectingLines.ElementAt(5);
            ILine line6 = SL.SLIntersectingLines.ElementAt(6);

            Assert.AreEqual(testLine5, line0);
            Assert.AreEqual(testLine7, line1);
            Assert.AreEqual(testLine1, line2);
            Assert.AreEqual(testLine6, line3);
            Assert.IsTrue(testLine2.Equals(line4) || testLine3.Equals(line4));
            Assert.AreEqual(testLine4, line6);

        }




        [TestMethod]
        public void GetNearestLinesTest()
        {
            SweepLine SL = new SweepLine(TestingColl1);

            SL.X = 130;
            SL.SLIntersectingLines.Add(testLine2);
            SL.SLIntersectingLines.Add(testLine3);
            SL.SLIntersectingLines.Add(testLine4);
            SL.SLIntersectingLines.Add(testLine5);
            SL.SLIntersectingLines.Add(testLine7);
            SL.SLIntersectingLines.Add(testLine8);
            SL.SLIntersectingLines.Add(testLine9);

            List<ILine> highestLines = new List<ILine>();
            List<ILine> lowestLines = new List<ILine>();

            List<ILine> aboveLines = SL.GetNearestLines(new ILine[] { testLine7 }, true);
            Assert.AreEqual(1, aboveLines.Count);
            ILine aboveLine = aboveLines.First();
            Assert.AreEqual(testLine9, aboveLine);

            aboveLines = SL.GetNearestLines(new ILine[] { testLine7, testLine9}, true, highestLines);
            Assert.AreEqual(2, aboveLines.Count);
            Assert.IsTrue(aboveLines.Contains(testLine2) && aboveLines.Contains(testLine3));
            Assert.AreEqual(1, highestLines.Count);
            Assert.IsTrue(highestLines.Contains(testLine9));


            aboveLines = SL.GetNearestLines(new ILine[] { testLine2, testLine3 }, true, highestLines);
            Assert.AreEqual(2, aboveLines.Count);
            Assert.IsTrue(aboveLines.Contains(testLine4) && aboveLines.Contains(testLine8));
            Assert.AreEqual(2, highestLines.Count);
            Assert.IsTrue(highestLines.Contains(testLine2) && highestLines.Contains(testLine3));



            List<ILine> belowLines = SL.GetNearestLines(new ILine[] { testLine7 }, false);
            Assert.AreEqual(1, belowLines.Count);
            ILine belowLine = belowLines.First();
            Assert.AreEqual(testLine5, belowLine);

            belowLines = SL.GetNearestLines(new ILine[] { testLine2, testLine3, testLine4, testLine8 }, false, lowestLines);
            Assert.AreEqual(1, belowLines.Count);
            Assert.IsTrue(belowLines.Contains(testLine9));
            Assert.AreEqual(2, lowestLines.Count);
            Assert.IsTrue(lowestLines.Contains(testLine2)&& lowestLines.Contains(testLine3));

        }


        [TestMethod]
        public void RightEndpointsProcessingTest()
        {
            SweepLine SL = new SweepLine(TestingColl1);

            SL.X = 110;
            SL.SLIntersectingLines.Add(testLine1);
            SL.SLIntersectingLines.Add(testLine2);
            SL.SLIntersectingLines.Add(testLine3);
            SL.SLIntersectingLines.Add(testLine4);
            SL.SLIntersectingLines.Add(testLine5);
            SL.SLIntersectingLines.Add(testLine6);

            SLEvent e = new SLEvent(110, 50);
            e.LeftEndPtLines.Add(testLine7);
            e.RightEndPtLines.Add(testLine1);
            e.IntersectingLines.Add(testLine1);
            e.IntersectingLines.Add(testLine6);


            SL.RightEndpointsProcessing(e);

            Assert.IsTrue(!SL.SLIntersectingLines.Contains(testLine1));

        }


        [TestMethod]
        public void LeftEndpointsProcessingTest()
        {
            SweepLine SL = new SweepLine(TestingColl1);

            SL.X = 125;
            SL.SLIntersectingLines.Add(testLine2);
            SL.SLIntersectingLines.Add(testLine3);
            SL.SLIntersectingLines.Add(testLine4);
            SL.SLIntersectingLines.Add(testLine5);
            SL.SLIntersectingLines.Add(testLine7);
            SL.SLIntersectingLines.Add(testLine8);

            SLEvent e = new SLEvent(125, 40);
            e.LeftEndPtLines.Add(testLine9);

            SL.LeftEndpointsProcessing(e);

            Assert.IsTrue(SL.SLIntersectingLines.Contains(testLine9));

            SLEvent foundIntersection = SL.EQ.FirstOrDefault(ev => ev.X == 125 && ev.Y == 40);
            Assert.IsNotNull(foundIntersection);
            Assert.IsTrue(foundIntersection.IntersectingLines.Contains(testLine7) && foundIntersection.IntersectingLines.Contains(testLine9));
            
        }




        [TestMethod]
        public void IntersectionPointsProcessingTest1()
        {
            SweepLine SL = new SweepLine(TestingColl1);

            SL.X = 110;
            SL.SLIntersectingLines.Add(testLine1);
            SL.SLIntersectingLines.Add(testLine2);
            SL.SLIntersectingLines.Add(testLine3);
            SL.SLIntersectingLines.Add(testLine4);
            SL.SLIntersectingLines.Add(testLine5);
            SL.SLIntersectingLines.Add(testLine6);

            SLEvent e = new SLEvent(110, 50);
            e.LeftEndPtLines.Add(testLine7);
            e.RightEndPtLines.Add(testLine1);
            e.IntersectingLines.Add(testLine1);
            e.IntersectingLines.Add(testLine6);


            SL.IntersectionPointsProcessing(e);

            

            SLEvent foundIntersection = SL.EQ.FirstOrDefault(ev => ev.X == 115 && ev.Y == 60);
            Assert.IsNotNull(foundIntersection);
        }



        [TestMethod]
        public void IntersectionPointsProcessingTest2()
        {
            SweepLine SL = new SweepLine(TestingColl1);

            SL.X = 134;
            SL.SLIntersectingLines.Add(testLine2);
            SL.SLIntersectingLines.Add(testLine3);
            SL.SLIntersectingLines.Add(testLine4);
            SL.SLIntersectingLines.Add(testLine5);
            SL.SLIntersectingLines.Add(testLine7);
            SL.SLIntersectingLines.Add(testLine8);
            SL.SLIntersectingLines.Add(testLine9);

            SLEvent e = new SLEvent(134, 60);
            e.IntersectingLines.Add(testLine2);
            e.IntersectingLines.Add(testLine3);
            e.IntersectingLines.Add(testLine4);
            e.IntersectingLines.Add(testLine8);


            SL.IntersectionPointsProcessing(e);



            SLEvent foundIntersection = SL.EQ.FirstOrDefault(ev => Math.Round( ev.X) == 156 && Math.Round(ev.Y) == 44);
            Assert.IsNotNull(foundIntersection);
        }


    }
}
