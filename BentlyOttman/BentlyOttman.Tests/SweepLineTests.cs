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

        SweepLine SL;

        public SweepLineTests()
        {

            List<ILine> lines = TestingColl1;

            SL = new SweepLine(lines);
        }

        /// <summary>
        /// Проверяет обход очереди событий
        /// То что не будет бесконечного цикла
        /// https://stackoverflow.com/questions/12145510/how-to-fail-a-test-that-is-stuck-in-an-infinite-loop
        /// </summary>
        [TestMethod]
        //[Timeout(1000)]
        public void TraverseEventQueueTest()
        {
            SL.TraverseEventQueue();
            Assert.IsTrue(SL.EQ.Count == 0);
            Assert.AreEqual(21, SL.EQ.VisitedEvents.Count);
        }


        [TestMethod]
        public void AboveBelowComparerTest()
        {
            SL.SLIntersectingLines.Clear();
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
            ILine line4 = SL.SLIntersectingLines.ElementAt(5);

            Assert.AreEqual(testLine5, line0);
            Assert.AreEqual(testLine7, line1);
            Assert.AreEqual(testLine9, line2);
            Assert.IsTrue( testLine2.Equals( line3)|| testLine3.Equals(line3));
            Assert.IsTrue(testLine4.Equals(line4)|| testLine8.Equals(line4));

        }




        [TestMethod]
        public void GetNearestLinesTest()
        {
            SL.SLIntersectingLines.Clear();
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



            List<ILine> belowLines = SL.GetNearestLines(new ILine[] { testLine7 }, false);
            Assert.AreEqual(1, belowLines.Count);
            ILine belowLine = belowLines.First();
            Assert.AreEqual(testLine5, belowLine);




        }
    }
}
