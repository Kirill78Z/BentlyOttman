using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BentlyOttman.Tests.TestInput;

namespace BentlyOttman.Tests
{
    [TestClass]
    public class IntersectionComputeTest
    {
        [TestMethod]
        public void IsLeftTest()
        {
            Assert.IsTrue(IntersectionCompute.IsLeft(testLine6, new SLEvent(70, 60)) > 0);
            Assert.IsTrue(IntersectionCompute.IsLeft(testLine6, new SLEvent(125, 40)) < 0);
            Assert.IsTrue(IntersectionCompute.IsLeft(testLine6, new SLEvent(110, 50)) == 0);
        }

        [TestMethod]
        public void AreIntersectingTest()
        {
            Assert.IsTrue(IntersectionCompute.AreIntersecting(testLine6, testLine2));
            Assert.IsTrue(IntersectionCompute.AreIntersecting(testLine9, testLine8));
            Assert.IsTrue(IntersectionCompute.AreIntersecting(testLine7, testLine5));
            Assert.IsTrue(IntersectionCompute.AreIntersecting(testLine6, testLine8));
            Assert.IsTrue(!IntersectionCompute.AreIntersecting(testLine6, testLine9));
            Assert.IsTrue(!IntersectionCompute.AreIntersecting(testLine2, testLine3));
            Assert.IsTrue(!IntersectionCompute.AreIntersecting(testLine4, testLine8));
        }

        [TestMethod]
        public void GetIntersectionPtTest()
        {
            SLEvent intersectionPt = IntersectionCompute.GetIntersectionPt(testLine4, testLine2);
            Assert.AreEqual(134, intersectionPt.X);
            Assert.AreEqual(60, intersectionPt.Y);

            intersectionPt = IntersectionCompute.GetIntersectionPt(testLine7, testLine9);
            Assert.AreEqual(125, intersectionPt.X);
            Assert.AreEqual(40, intersectionPt.Y);

            intersectionPt = IntersectionCompute.GetIntersectionPt(testLine9, testLine10);
            Assert.AreEqual(170, intersectionPt.X);
            Assert.AreEqual(46, intersectionPt.Y);
        }
    }
}
