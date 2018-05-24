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
        internal double X { get; set; }

        /// <summary>
        /// Коллекция отрезков пересекаемых сканирующей линией, упорядоченная по возрастанию
        /// координаты Y точки пересечения сканирующей линии и отрезка
        /// </summary>
        internal SortedSet<ILine> IntersectingLines { get; private set; }
        private AboveBelowComparer aboveBelowComparer;
        internal EventQueue EQ;

        public SweepLine(IEnumerable<ILine> lines)
        {
            aboveBelowComparer = new AboveBelowComparer(this);
            IntersectingLines = new SortedSet<ILine>(aboveBelowComparer);

            EQ = new EventQueue();

            EQ.Populate(lines);

        }


        private List<SLEvent> output;
        internal List<SLEvent> TraverseEventQueue()
        {
            output = new List<SLEvent>();
            
            while (EQ.Count > 0)
            {
                SLEvent e = EQ.First();

                GoToEvent(e);

                EQ.Remove(e);
            }

            return output;

        }

        internal void GoToEvent(SLEvent e)
        {
            this.X = e.X;
            //В одной точке могут быть как конечные точки отрезков, так и их пересечения
            RightEndpointsProcessing(e);
            LeftEndpointsProcessing(e);
            IntersectionPointsProcessing(e);
            //Условия для добавления события в output:
            //Если общее количество объектов в коллекциях RightEndPtLines и LeftEndPtLines больше 1 (линии стыкуются)
            //или
            //IntersectingLines не пусто (линии пересекаются)
            if (e.LeftEndPtLines.Count+e.LeftEndPtLines.Count>1
                ||e.IntersectingLines.Count>0)
            {
                output.Add(e);
            }

        }



        private void RightEndpointsProcessing(SLEvent e)
        {
            if (e.RightEndPtLines.Count > 0)
            {
                ILine[] rightEndpointLines = e.RightEndPtLines.ToArray();
                ILine aboveLine = GetAboveLine(rightEndpointLines);
                ILine belowLine = GetBelowLine(rightEndpointLines);
                foreach (ILine rpl in rightEndpointLines)
                {
                    IntersectingLines.Remove(rpl);
                }
                AddIntersectionEvent(aboveLine, belowLine);
            }

        }

        private void LeftEndpointsProcessing(SLEvent e)
        {
            if (e.LeftEndPtLines.Count > 0)
            {
                ILine[] leftEndpointLines = e.LeftEndPtLines.ToArray();
                foreach (ILine lpl in leftEndpointLines)
                {
                    if (lpl.Pt1.X != lpl.Pt2.X)//В набор сканирующей линии не добавляются вертикальные линии!
                        IntersectingLines.Add(lpl);
                    else
                    {
                        //Вместо этого
                        //вертикальная линия должна быть проверена на пересечение со всеми в наборе сканирующей линии
                        //Найденные точки пересечений должны попадать в очередь событий,
                        //но сама вертикальная линия в объекте точки должна храниться отдельно от других пересекающихся линий
                        //и таким образом не участвует в процедуре IntersectionPointsProcessing 


                    }
                }
                ILine aboveLine = GetAboveLine(leftEndpointLines);
                ILine belowLine = GetBelowLine(leftEndpointLines);

                foreach (ILine lpl in leftEndpointLines)
                {
                    AddIntersectionEvent(aboveLine, lpl);
                    AddIntersectionEvent(belowLine, lpl);
                }
            }
   
        }

        private void IntersectionPointsProcessing(SLEvent e)
        {
            //При пересечении вертикальной линии с невертикальной могут возникать ситуации, когда
            //в наборе IntersectingLines только одна линия. В этом случае процедура не выполняется

            if (e.IntersectingLines.Count>1)
            {
                //ILine[] intersectingLines = e.IntersectingLines.ToArray();

                ILine highestLine = null;
                ILine lowestLine = null;

                UpdateIntersectingLinePositions(out highestLine, out lowestLine, e.IntersectingLines);

                ILine aboveLine = GetAboveLine(highestLine);
                ILine belowLine = GetBelowLine(lowestLine);

                //Здесь нужно проверить только наиболее высокую из пересекающихся линий с соседней верхней и
                //наиболее низкую из пересекающихся линий с соседней нижней
                AddIntersectionEvent(aboveLine, highestLine);
                AddIntersectionEvent(belowLine, lowestLine);
            }
            
        }

        /// <summary>
        /// Получение линии, которая находится над всеми переданными
        /// </summary>
        /// <param name="inputLines"></param>
        /// <returns></returns>
        internal ILine GetAboveLine(params ILine[] inputLines)
        {
            return IntersectingLines.FirstOrDefault(intersectingLine =>
            inputLines.All(inputLine =>
            aboveBelowComparer.Compare(intersectingLine, inputLine) > 0)
            );
        }

        /// <summary>
        /// Получение линии, которая находится под всеми переданными
        /// </summary>
        /// <param name="inputLines"></param>
        /// <returns></returns>
        internal ILine GetBelowLine(params ILine[] inputLines)
        {
            return IntersectingLines.FirstOrDefault(intersectingLine =>
            inputLines.All(inputLine =>
            aboveBelowComparer.Compare(intersectingLine, inputLine) < 0)
            );
        }


        /// <summary>
        /// Проверить на пересечение 2 линии и добавить в очередь событие пересечения
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        private void AddIntersectionEvent(ILine line1, ILine line2)
        {
            if (line1 != null && line2 != null && IntersectionCompute.AreIntersecting(line1, line2))
            {
                SLEvent intersectionEvent = IntersectionCompute.GetIntersectionPt(line1, line2);
                intersectionEvent = EQ.AddEventToQueueAndGetActual(intersectionEvent);
                intersectionEvent.IntersectingLines.Add(line1);
                intersectionEvent.IntersectingLines.Add(line2);
            }
        }

        /// <summary>
        /// Перестановка линий в наборе сканирующей прямой при проходе через точку их пересечения
        /// </summary>
        /// <param name="lines"></param>
        public void UpdateIntersectingLinePositions(out ILine highestLine, out ILine lowestLine, HashSet<ILine> lines)
        {
            highestLine = null;
            lowestLine = null;

            //Убрать линии, пересекающиеся в текущем событии, из набора сканирующей линии
            foreach (ILine line in lines)
            {
                IntersectingLines.Remove(line);
            }

            aboveBelowComparer.IntersectingLinesAtCurrentEvent = lines;
            //AboveBelowComparer при сравнении между собой линий, которые пересекаются в текущем событии дает прирощение по X
            //Благодаря этому актуализируется их взаимное расположение
            foreach (ILine line in lines)
            {
                IntersectingLines.Add(line);
            }


            aboveBelowComparer.IntersectingLinesAtCurrentEvent = null;
        }





        private class AboveBelowComparer : IComparer<ILine>
        {
            private SweepLine sl;

            /// <summary>
            /// TODO
            /// Набор пересекающихся линий. Данный набор заполняется в методе UpdateIntersectingLinePositions
            /// Для актуализации положения линий при переходе через точку их пересечения
            /// при сравнении пересекающихся линий между собой давать приращение положению сканирующей линии
            /// </summary>
            public HashSet<ILine> IntersectingLinesAtCurrentEvent { get; set; }

            public int Compare(ILine line1, ILine line2)
            {
                double currX = sl.X;
                //(x - x_1 )/(x_2 - x_1 ) = (y - y_1 )/(y_2 - y_1) - уравнение отрезка

                //Вертикальная линия не должна попадать в набор!!!
                double den1 = line1.Pt2.X - line1.Pt1.X;
                double den2 = line2.Pt2.X - line2.Pt1.X;
                if (den1!=0&& den2!=0)
                {
                    //координата Y первой линии 
                    double y1 = (currX - line1.Pt1.X) * (line1.Pt2.Y - line1.Pt1.Y) / den1 + line1.Pt1.Y;
                    //координата Y второй линии 
                    double y2 = (currX - line2.Pt1.X) * (line2.Pt2.Y - line2.Pt1.Y) / den2 + line2.Pt1.Y;

                    return y1.CompareTo(y2);
                }else
                {
                    throw new Exception("В набор сканирующей линии попала вертикальная прямая");
                }
                

            }

            public AboveBelowComparer(SweepLine sl)
            {
                this.sl = sl;
            }
        }
    }
}
