using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BentlyOttman.Tests
{
    [TestClass]
    public class MainTests
    {

        class TestLine : ILine
        {
            public double X1 { get; set; }
            public double Y1 { get; set; }

            public double X2 { get; set; }
            public double Y2 { get; set; }
        }

        [TestMethod]
        public void MainTest()
        {
            //arrange

            TestLine testLine1 = new TestLine() { X1 = 50, Y1 = 30, X2 = 110, Y2 = 50 };//1
            TestLine testLine2 = new TestLine() { X1 = 190, Y1 = 60, X2 = 70, Y2 = 60 };//2
            TestLine testLine3 = new TestLine() { X1 = 190, Y1 = 60, X2 = 70, Y2 = 60 };//3
            TestLine testLine4 = new TestLine() { X1 = 155, Y1 = 45, X2 = 85, Y2 = 95 };//4
            TestLine testLine5 = new TestLine() { X1 = 140, Y1 = 30, X2 = 100, Y2 = 20 };//5
            TestLine testLine6 = new TestLine() { X1 = 100, Y1 = 30, X2 = 120, Y2 = 70 };//6
            TestLine testLine7 = new TestLine() { X1 = 110, Y1 = 50, X2 = 140, Y2 = 30 };//7
            TestLine testLine8 = new TestLine() { X1 = 190, Y1 = 20, X2 = 120, Y2 = 70 };//8
            TestLine testLine9 = new TestLine() { X1 = 125, Y1 = 40, X2 = 200, Y2 = 50 };//9
            TestLine testLine10 = new TestLine() { X1 = 170, Y1 = 90, X2 = 170, Y2 = 20 };//10


            List<ILine> lines = new List<ILine>()
            {
                testLine1,testLine2,testLine3,testLine4,testLine5,
                testLine6,testLine7,testLine8,testLine9,testLine10,
            };

            //act
            List<IntersectionPoint> actual = Main.FindIntersections(lines);

            //assert
            Assert.IsNotNull(actual);

            IntersectionPoint point = actual.Find(p => AlmostEquals(p.X, 70) && AlmostEquals(p.Y, 60));//1
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine2));
            Assert.IsTrue(point.Lines.Contains(testLine3));

            point = actual.Find(p => AlmostEquals(p.X, 110) && AlmostEquals(p.Y, 50));//2
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine1));
            Assert.IsTrue(point.Lines.Contains(testLine6));
            Assert.IsTrue(point.Lines.Contains(testLine7));

            point = actual.Find(p => AlmostEquals(p.X, 115) && AlmostEquals(p.Y, 60));//3
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine2));
            Assert.IsTrue(point.Lines.Contains(testLine3));
            Assert.IsTrue(point.Lines.Contains(testLine6));

            point = actual.Find(p => AlmostEquals(p.X, 120) && AlmostEquals(p.Y, 70));//4
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine4));
            Assert.IsTrue(point.Lines.Contains(testLine6));
            Assert.IsTrue(point.Lines.Contains(testLine8));


            point = actual.Find(p => AlmostEquals(p.X, 125) && AlmostEquals(p.Y, 40));//5
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine7));
            Assert.IsTrue(point.Lines.Contains(testLine9));


            point = actual.Find(p => AlmostEquals(p.X, 134) && AlmostEquals(p.Y, 60));//6
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine2));
            Assert.IsTrue(point.Lines.Contains(testLine3));
            Assert.IsTrue(point.Lines.Contains(testLine4));
            Assert.IsTrue(point.Lines.Contains(testLine8));


            point = actual.Find(p => AlmostEquals(p.X, 140) && AlmostEquals(p.Y, 30));//7
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine5));
            Assert.IsTrue(point.Lines.Contains(testLine7));


            point = actual.Find(p => AlmostEquals(p.X, 156.17977528) && AlmostEquals(p.Y, 44.15730337));//8
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine8));
            Assert.IsTrue(point.Lines.Contains(testLine9));


            point = actual.Find(p => AlmostEquals(p.X, 170) && AlmostEquals(p.Y, 34.28571429));//9
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine8));
            Assert.IsTrue(point.Lines.Contains(testLine10));


            point = actual.Find(p => AlmostEquals(p.X, 170) && AlmostEquals(p.Y, 46));//10
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine9));
            Assert.IsTrue(point.Lines.Contains(testLine10));


            point = actual.Find(p => AlmostEquals(p.X, 170) && AlmostEquals(p.Y, 60));//11
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine2));
            Assert.IsTrue(point.Lines.Contains(testLine3));
            Assert.IsTrue(point.Lines.Contains(testLine10));

            point = actual.Find(p => AlmostEquals(p.X, 190) && AlmostEquals(p.Y, 60));//12
            Assert.IsNotNull(point);
            Assert.IsTrue(point.Lines.Contains(testLine2));
            Assert.IsTrue(point.Lines.Contains(testLine3));


        }

        private bool AlmostEquals(double a, double b)
        {
            return Math.Abs(a - b) < 0.00000001;
        }
    }
}
