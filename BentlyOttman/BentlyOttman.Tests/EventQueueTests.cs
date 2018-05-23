using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BentlyOttman.Tests.TestInput;

namespace BentlyOttman.Tests
{
    [TestClass]
    public class EventQueueTests
    {

        //SweepLine SL;
        EventQueue EQ;

        public EventQueueTests()
        {

            List<ILine> lines = TestingColl1;

            EQ = new EventQueue();
            EQ.Populate(lines);

            //SL = new SweepLine(lines);
        }

        /// <summary>
        /// Проверяет правильность заполнения очереди событий крайними точками отрезков
        /// </summary>
        [TestMethod]
        public void PopulateAddEventToQueueTest()
        {
            //Получить очередь событий
            //PrivateObject prSL = new PrivateObject(SL);
            //EventQueue EQ = (EventQueue)prSL.GetField("EQ");

            //Для статического члена
            //PrivateType pt = new PrivateType(typeof(SweepLine));
            //SortedSet<SLEvent> EQ = (SortedSet<SLEvent>)pt.InvokeStatic("EQArrange", new object[] { lines });

            //assert
            //Проверить что условие сортировки соблюдается
            SLEvent prevEvent = null;

            foreach (SLEvent currEvent in EQ)
            {
                if (prevEvent != null)
                {
                    Assert.IsTrue(currEvent.X > prevEvent.X || (currEvent.X == prevEvent.X && currEvent.Y > prevEvent.Y));
                }
                prevEvent = currEvent;
            }

            //Проверить что данные о линиях привязаны к событиям правильно
            SLEvent pt2RightEndPts = new SLEvent(140, 30);
            pt2RightEndPts = EQ.FirstOrDefault(pt => pt.CompareTo(pt2RightEndPts) == 0);
            Assert.IsNotNull(pt2RightEndPts);
            Assert.IsTrue(pt2RightEndPts.RightEndPtLines.Count > 1);

            SLEvent ptLeftAndRightEndPts = new SLEvent(110, 50);
            ptLeftAndRightEndPts = EQ.FirstOrDefault(pt => pt.CompareTo(ptLeftAndRightEndPts) == 0);
            Assert.IsNotNull(ptLeftAndRightEndPts);
            Assert.IsTrue(ptLeftAndRightEndPts.LeftEndPtLines.Count > 0
                && ptLeftAndRightEndPts.RightEndPtLines.Count > 0);
        }



    }
}
