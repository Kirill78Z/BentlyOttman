using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("BentlyOttman.Tests")]

namespace BentlyOttman
{
    internal class SweepLine
    {
        /// <summary>
        /// Текущее положение сканирующей линии
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Коллекция отрезков пересекаемых сканирующей линией, упорядоченная по возрастанию
        /// координаты Y точки пересечения сканирующей линии и отрезка
        /// </summary>
        public SortedSet<ILine> IntersectingLines { get; private set; }

        private EventQueue EQ;

        public SweepLine(IEnumerable<ILine> lines)
        {
            IntersectingLines = new SortedSet<ILine>(new AboveBelowComparer(this));

            EQ = new EventQueue();

            EQ.Populate(lines);
            
        }



        public List<SLEvent> TraverseEventQueue()
        {
            List<SLEvent> output = new List<SLEvent>();
            while (EQ.Count > 0)
            {
                SLEvent e = EQ.First();

                GoToEvent(e);

                EQ.Remove(e);
            }

            return output;
            
        }

        private void GoToEvent(SLEvent e)
        {
            this.X = e.X;
            //switch (e.EventType)
            //{
            //    case EventType.LeftEndpoint:
            //        LeftEndpointEventProcessing(e);
            //        break;
            //    case EventType.RightEndpoint:
            //        RightEndpointEventProcessing(e);
            //        break;
            //    default :
            //        IntersectionPointProcessing(e);
            //        break;
            //}

        }

        private void LeftEndpointEventProcessing(SLEvent e)
        {

        }

        private void RightEndpointEventProcessing(SLEvent e)
        {

        }

        private void IntersectionPointProcessing(SLEvent e)
        {

        }



        public void SwapLines(ILine line1, ILine line2)
        {

        }





        private class AboveBelowComparer : IComparer<ILine>
        {
            private SweepLine sl;

            public int Compare(ILine line1, ILine line2)
            {
                double currX = sl.X;
                //(x - x_1 )/(x_2 - x_1 ) = (y - y_1 )/(y_2 - y_1) - уравнение отрезка
                //координата Y первой линии 
                double y1 = (currX - line1.Pt1.X) * (line1.Pt2.Y - line1.Pt1.Y) / (line1.Pt2.X - line1.Pt1.X) - line1.Pt1.Y;
                //координата Y второй линии 
                double y2 = (currX - line2.Pt1.X) * (line2.Pt2.Y - line2.Pt1.Y) / (line2.Pt2.X - line2.Pt1.X) - line2.Pt1.Y;

                return y1.CompareTo(y2);

            }

            public AboveBelowComparer(SweepLine sl)
            {
                this.sl = sl;
            }
        }
    }
}
