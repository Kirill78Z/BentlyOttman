using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BentlyOttman
{
    /// <summary>
    /// Интерфейс для объектов-линий.
    /// Если у двух линий одинаковый хешкод, то в алгоритме BentleyOttman будет обсчитана только одна из них.
    /// </summary>
    public interface ILine
    {
        SLEvent Pt1 { get; set; }

        SLEvent Pt2 { get; set; }

    }

}
