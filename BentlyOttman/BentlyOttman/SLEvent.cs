using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BentlyOttman
{

    //public enum EventType
    //{
    //    LeftEndpoint,
    //    RightEndpoint,
    //    IntersectionPoint
    //}

    public class SLEvent : IComparable<SLEvent>
    {
        public double X { get; set; }
        public double Y { get; set; }

        //public EventType EventType { get; set; }


        //public bool IsIntersectionPt { get; set; }

        /// <summary>
        /// Линии, для которых эта точка является крайней левой
        /// </summary>
        internal HashSet<ILine> LeftEndPtLines { get; set; } = new HashSet<ILine>();


        /// <summary>
        /// Линии, для которых эта точка является крайней правой
        /// </summary>
        internal HashSet<ILine> RightEndPtLines { get; set; } = new HashSet<ILine>();


        /// <summary>
        /// Список пересекающихся линий если это точка пересечения
        /// </summary>
        internal HashSet<ILine> IntersectingLines { get; set; } = new HashSet<ILine>();

        /// <summary>
        /// Вертикальные пересекающиеся линии если это точка пересечения
        /// </summary>
        internal HashSet<ILine> VerticalIntersectingLines { get; set; } = new HashSet<ILine>();


        public IEnumerable<ILine> IntersectionsAtPoint { get
            { return LeftEndPtLines.Union(RightEndPtLines).Union(IntersectingLines).Union(VerticalIntersectingLines); }
        }

        /// <summary>
        /// Линия, которой пренадлежит крайняя точка
        /// </summary>
        //internal ILine Line { get; set; }

        //internal bool IsRight { get; set; }

        public SLEvent(double x, double y)
        {
            X = x;
            Y = y;
        }

        //public override int GetHashCode()
        //{
        //    Tuple<double, double> t = Tuple.Create(X, Y);
        //    return t.GetHashCode();
        //}

        public int CompareTo(SLEvent other)
        {
            int comparison = this.X.CompareTo(other.X);
            if (comparison != 0)
            {
                return comparison;
            }
            else
            {
                return this.Y.CompareTo(other.Y);
            }
        }
    }
}
