using System;
using System.Collections.Generic;
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
        /// Проверяет обход 
        /// </summary>
        [TestMethod]
        public void TraverseEventQueueTest()
        {
            SL.TraverseEventQueue();
        }
    }
}
