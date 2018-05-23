using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BentlyOttman.Tests
{
    public static class TestInput
    {
        public static TestLine testLine1 = new TestLine() { Pt1 = new SLEvent(50, 30), Pt2 = new SLEvent(110, 50) };//1
        public static TestLine testLine2 = new TestLine() { Pt1 = new SLEvent(190, 60), Pt2 = new SLEvent(70, 60) };//2
        public static TestLine testLine3 = new TestLine() { Pt1 = new SLEvent(190, 60), Pt2 = new SLEvent(70, 60) };//3
        public static TestLine testLine4 = new TestLine() { Pt1 = new SLEvent(155, 45), Pt2 = new SLEvent(85, 95) };//4
        public static TestLine testLine5 = new TestLine() { Pt1 = new SLEvent(140, 30), Pt2 = new SLEvent(100, 20) };//5
        public static TestLine testLine6 = new TestLine() { Pt1 = new SLEvent(100, 30), Pt2 = new SLEvent(120, 70) };//6
        public static TestLine testLine7 = new TestLine() { Pt1 = new SLEvent(110, 50), Pt2 = new SLEvent(140, 30) };//7
        public static TestLine testLine8 = new TestLine() { Pt1 = new SLEvent(190, 20), Pt2 = new SLEvent(120, 70) };//8
        public static TestLine testLine9 = new TestLine() { Pt1 = new SLEvent(125, 40), Pt2 = new SLEvent(200, 50) };//9
        public static TestLine testLine10 = new TestLine() { Pt1 = new SLEvent(170, 90), Pt2 = new SLEvent(170, 20) };//10



        public static List<ILine> TestingColl1 { get; } = new List<ILine>()
            {
                testLine1,testLine2,testLine3,testLine4,testLine5,
                testLine6,testLine7,testLine8,testLine9,testLine10,
            };
    }
}
