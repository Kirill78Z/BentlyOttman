using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BentlyOttman
{
    /// <summary>
    /// http://geomalgorithms.com/a09-_intersect-3.html
    /// </summary>
    public static class MainProcedures
    {
        public static List<SLEvent>
            FindIntersections(IEnumerable<ILine> lines)
        {
            //Сканирующая линия
            SweepLine SL = new SweepLine(lines);
            return SL.TraverseEventQueue();
        }

    }
}
