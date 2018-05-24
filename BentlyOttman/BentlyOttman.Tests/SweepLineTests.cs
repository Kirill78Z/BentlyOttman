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
        [Timeout(1000)]
        public void TraverseEventQueueTest()
        {
            SL.TraverseEventQueue();
            Assert.IsTrue(SL.EQ.Count == 0);
        }


        [TestMethod]
        public void AboveBelowComparerTest()
        {
            SL.IntersectingLines.Clear();
            SL.X = 130;
            SL.IntersectingLines.Add(testLine2);
            SL.IntersectingLines.Add(testLine4);
            SL.IntersectingLines.Add(testLine5);
            SL.IntersectingLines.Add(testLine7);
            SL.IntersectingLines.Add(testLine9);

            ILine line0 = SL.IntersectingLines.ElementAt(0);
            ILine line1 = SL.IntersectingLines.ElementAt(1);
            ILine line2 = SL.IntersectingLines.ElementAt(2);
            ILine line3 = SL.IntersectingLines.ElementAt(3);
            ILine line4 = SL.IntersectingLines.ElementAt(4);

            Assert.AreEqual(testLine5, line0);
            Assert.AreEqual(testLine7, line1);
            Assert.AreEqual(testLine9, line2);
            Assert.AreEqual(testLine2, line3);
            Assert.AreEqual(testLine4, line4);

        }




        [TestMethod]
        public void GetAboveBelowLineTest()
        {
            SL.IntersectingLines.Clear();
            SL.X = 130;
            SL.IntersectingLines.Add(testLine2);
            SL.IntersectingLines.Add(testLine3);
            SL.IntersectingLines.Add(testLine4);
            SL.IntersectingLines.Add(testLine5);
            SL.IntersectingLines.Add(testLine7);
            SL.IntersectingLines.Add(testLine8);
            SL.IntersectingLines.Add(testLine9);


            ILine aboveLine = SL.GetAboveLine(testLine7);
            Assert.AreEqual(testLine9, aboveLine);

            ILine belowLine = SL.GetBelowLine(testLine7);
            Assert.AreEqual(testLine5, belowLine);
        }
    }
}
