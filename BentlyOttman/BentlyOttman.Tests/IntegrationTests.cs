using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BentlyOttman.Tests.TestInput;

namespace BentlyOttman.Tests
{

    [TestClass]
    public class IntegrationTests
    {
        
        [TestMethod]
        public void IntegrationTest()
        {
            //arrange
            List<ILine> lines = TestingColl1;

            //act
            List<SLEvent> actual = MainProcedures.FindIntersections(lines);

            //assert
            Assert.IsNotNull(actual);

            //Assert.AreEqual(12, actual.Count);

            //Проверяем точки пересечения
            SLEvent point = actual.Find(p => AlmostEquals(p.X, 70) && AlmostEquals(p.Y, 60));//1
            Assert.IsNotNull(point);
            IEnumerable<ILine> intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(2, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine2));
            Assert.IsTrue(intersectingLines.Contains(testLine3));

            point = actual.Find(p => AlmostEquals(p.X, 110) && AlmostEquals(p.Y, 50));//2
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(3, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine1));
            Assert.IsTrue(intersectingLines.Contains(testLine6));
            Assert.IsTrue(intersectingLines.Contains(testLine7));

            point = actual.Find(p => AlmostEquals(p.X, 115) && AlmostEquals(p.Y, 60));//3
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(3, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine2));
            Assert.IsTrue(intersectingLines.Contains(testLine3));
            Assert.IsTrue(intersectingLines.Contains(testLine6));

            point = actual.Find(p => AlmostEquals(p.X, 120) && AlmostEquals(p.Y, 70));//4
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(3, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine4));
            Assert.IsTrue(intersectingLines.Contains(testLine6));
            Assert.IsTrue(intersectingLines.Contains(testLine8));


            point = actual.Find(p => AlmostEquals(p.X, 125) && AlmostEquals(p.Y, 40));//5
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(2, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine7));
            Assert.IsTrue(intersectingLines.Contains(testLine9));


            point = actual.Find(p => AlmostEquals(p.X, 134) && AlmostEquals(p.Y, 60));//6
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(4, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine2));
            Assert.IsTrue(intersectingLines.Contains(testLine3));
            Assert.IsTrue(intersectingLines.Contains(testLine4));
            Assert.IsTrue(intersectingLines.Contains(testLine8));


            point = actual.Find(p => AlmostEquals(p.X, 140) && AlmostEquals(p.Y, 30));//7
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(2, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine5));
            Assert.IsTrue(intersectingLines.Contains(testLine7));


            point = actual.Find(p => AlmostEquals(p.X, 156.17977528) && AlmostEquals(p.Y, 44.15730337));//8
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(2, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine8));
            Assert.IsTrue(intersectingLines.Contains(testLine9));


            point = actual.Find(p => AlmostEquals(p.X, 170) && AlmostEquals(p.Y, 34.28571429));//9
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(2, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine8));
            Assert.IsTrue(intersectingLines.Contains(testLine10));


            point = actual.Find(p => AlmostEquals(p.X, 170) && AlmostEquals(p.Y, 46));//10
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(2, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine9));
            Assert.IsTrue(intersectingLines.Contains(testLine10));


            point = actual.Find(p => AlmostEquals(p.X, 170) && AlmostEquals(p.Y, 60));//11
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(3, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine2));
            Assert.IsTrue(intersectingLines.Contains(testLine3));
            Assert.IsTrue(intersectingLines.Contains(testLine10));

            point = actual.Find(p => AlmostEquals(p.X, 190) && AlmostEquals(p.Y, 60));//12
            Assert.IsNotNull(point);
            intersectingLines = point.IntersectionsAtPoint;
            Assert.AreEqual(2, intersectingLines.Count());
            Assert.IsTrue(intersectingLines.Contains(testLine2));
            Assert.IsTrue(intersectingLines.Contains(testLine3));


        }

        private bool AlmostEquals(double a, double b)
        {
            return Math.Abs(a - b) < 0.000001;
        }
    }
}
