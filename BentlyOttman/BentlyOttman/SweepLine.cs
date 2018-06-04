using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static BentlyOttman.IntersectionCompute;

[assembly: InternalsVisibleTo("BentlyOttman.Tests")]

namespace BentlyOttman
{

    //Главные проблемы:
    //  - SortedSet не хранит несколько объектов, для которых Compare возвращает 0.
    //  Такое возникет в тех случаях, когда пытаемся добавить новую линию,
    //  а в наборе уже есть линия, которая при данном значении X имеет ту же координату Y.
    //  Если эти линии не параллельны, то можно дать приращение X и выполнять вставку,
    //  однако нужно хранить так же и параллельные линии
    //  - По какой-то причине возникают повторные попадания в одни и те же события.
    //  Этого не долно быть по логике алгоритма. Нужно разобраться с этим




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
        internal SortedSet<ILine> SLIntersectingLines { get; private set; }
        internal AboveBelowComparer AboveBelowComparer1 { get; private set; }
        internal EventQueue EQ;

        public SweepLine(IEnumerable<ILine> lines)
        {
            AboveBelowComparer1 = new AboveBelowComparer(this);
            SLIntersectingLines = new SortedSet<ILine>(AboveBelowComparer1);

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
            AboveBelowComparer1.IntersectingLinesAtCurrentEvent
                = new HashSet<ILine>(e.LeftEndPtLines.Concat(e.RightEndPtLines.Concat(e.IntersectingLines)));


            RightEndpointsProcessing(e);
            LeftEndpointsProcessing(e);
            IntersectionPointsProcessing(e);


            AboveBelowComparer1.IntersectingLinesAtCurrentEvent = null;
            //Условия для добавления события в output:
            //Если общее количество объектов в коллекциях RightEndPtLines и LeftEndPtLines больше 1 (линии стыкуются)
            //или
            //IntersectingLines не пусто (линии пересекаются)
            if (e.LeftEndPtLines.Count + e.RightEndPtLines.Count > 1
                || e.IntersectingLines.Count > 0)
            {
                output.Add(e);
            }

        }



        internal void RightEndpointsProcessing(SLEvent e)
        {
            HashSet<ILine> rightEndpointLines = new HashSet<ILine>(e.RightEndPtLines);
            //Если в наборе есть вертикальные линии, то их нужно убрать
            rightEndpointLines.RemoveWhere(l => l.Pt1.X == l.Pt2.X);

            if (rightEndpointLines.Count > 0)
            {
                //aboveBelowComparer.IntersectingLinesAtCurrentEvent = rightEndpointLines;
                AboveBelowComparer1.Increment = -1;
                List<ILine> aboveLines = GetNearestLines(rightEndpointLines, true);
                List<ILine> belowLines = GetNearestLines(rightEndpointLines, false);
                //aboveBelowComparer.IntersectingLinesAtCurrentEvent = null;
                AboveBelowComparer1.Increment = 1;
                foreach (ILine rpl in rightEndpointLines)
                {
                    SLIntersectingLines.Remove(rpl);
                }

                foreach (ILine aboveLine in aboveLines)
                {
                    foreach (ILine belowLine in belowLines)
                    {
                        AddIntersectionEvent(aboveLine, belowLine);
                    }
                }


            }

        }

        internal void LeftEndpointsProcessing(SLEvent e)
        {
            HashSet<ILine> leftEndpointLines = new HashSet<ILine>(e.LeftEndPtLines);
            List<ILine> vertLines = new List<ILine>();
            foreach (ILine lpl in leftEndpointLines)
            {
                if (lpl.Pt1.X != lpl.Pt2.X)//В набор сканирующей линии не добавляются вертикальные линии!
                    SLIntersectingLines.Add(lpl);
                else
                {
                    //Вместо этого
                    VerticalLineProcessing(lpl);
                    vertLines.Add(lpl);
                }
            }

            //Вертикальные линии не подлежат дальнейшей обработке в этом методе
            foreach (ILine vertLine in vertLines)
            {
                leftEndpointLines.Remove(vertLine);
            }


            if (leftEndpointLines.Count > 0)
            {
                List<ILine> aboveLines = GetNearestLines(leftEndpointLines, true);
                List<ILine> belowLines = GetNearestLines(leftEndpointLines, false);

                foreach (ILine lpl in leftEndpointLines)
                {
                    foreach (ILine aboveLine in aboveLines)
                    {
                        AddIntersectionEvent(aboveLine, lpl);
                    }
                    foreach (ILine belowLine in belowLines)
                    {
                        AddIntersectionEvent(belowLine, lpl);
                    }

                }
            }

        }


        internal void VerticalLineProcessing(ILine vertLine)
        {
            //вертикальная линия должна быть проверена на пересечение со всеми в наборе сканирующей линии
            foreach (ILine line in SLIntersectingLines)
            {
                if (AreIntersecting(vertLine, line))
                {
                    SLEvent intersectionPt = GetIntersectionPt(vertLine.Pt1.X, line);
                    //Найденные точки пересечений должны попадать в очередь событий,
                    intersectionPt = EQ.AddEventToQueueAndGetActual(intersectionPt);
                    intersectionPt.IntersectingLines.Add(line);
                    //но сама вертикальная линия в объекте точки должна храниться отдельно
                    //от других пересекающихся линий в VerticalIntersectingLines
                    //и таким образом не участвует в процедуре IntersectionPointsProcessing 
                    intersectionPt.VerticalIntersectingLines.Add(vertLine);
                }

            }

        }

        internal void IntersectionPointsProcessing(SLEvent e)
        {
            //При пересечении вертикальной линии с невертикальной могут возникать ситуации, когда
            //в наборе IntersectingLines только одна линия. В этом случае процедура не выполняется

            if (e.IntersectingLines.Count > 1)
            {

                //Если линии горизонтальные, то можкт быть несколько highestLine или lowestLine
                //в наборе пересекающихся

                List<ILine> highestLines = null;
                List<ILine> lowestLines = null;
                List<ILine> aboveLines = null;
                List<ILine> belowLines = null;

                UpdateIntersectingLinePositions(out highestLines, out lowestLines,
                    out aboveLines, out belowLines, e.IntersectingLines);

                //ILine aboveLine 
                //ILine belowLine 

                //Здесь нужно проверить только наиболее высокую из пересекающихся линий с соседней верхней и
                //наиболее низкую из пересекающихся линий с соседней нижней
                //С учетом того, что в одной координате Y могут пеерсекаться несколько линий, попарно проверять каждую с каждой

                foreach (ILine aboveLine in aboveLines)
                {
                    foreach (ILine highestLine in highestLines)
                    {
                        AddIntersectionEvent(aboveLine, highestLine);
                    }
                }
                foreach (ILine belowLine in belowLines)
                {
                    foreach (ILine lowestLine in lowestLines)
                    {
                        AddIntersectionEvent(belowLine, lowestLine);
                    }
                }

                //AddIntersectionEvent(aboveLine, highestLine);
                //AddIntersectionEvent(belowLine, lowestLine);



            }

        }


        /// <summary>
        /// Получение линии (или нескольких линий с одинаковой координатой Y в текущей точке),
        /// которая находится сразу над или под всеми переданными
        /// </summary>
        /// <param name="inputLines"></param>
        /// <param name="above"></param>
        /// <param name="borderLines">Линии, которые находятся на границе до нахождения искомых линий.
        /// Данные линии все имеют одинаковую координату Y в текущем положении сканирующей линии</param>
        /// <returns></returns>
        internal List<ILine> GetNearestLines(IEnumerable<ILine> inputLines, bool above,
            List<ILine> borderLines = null)
        {
            List<ILine> nearestLines = new List<ILine>();

            IEnumerable<ILine> collToIterate = SLIntersectingLines;

            if (!above)
                collToIterate = SLIntersectingLines.Reverse();

            if (borderLines != null)
                borderLines.Clear();

            bool oneLineFound = false;
            foreach (ILine line in collToIterate)
            {
                if (!oneLineFound)
                {
                    Func<ILine, bool> comparisonFunc = inputLine => AboveBelowComparer1.CompareByCurrentY(line, inputLine) > 0;
                    //{
                    //    int geomComparison = AboveBelowComparer1.CompareByCurrentY(line, inputLine);
                    //    if (geomComparison==0)
                    //    {
                    //        AboveBelowComparer1.IntersectingLinesAtCurrentEvent.Add();
                    //        geomComparison = AboveBelowComparer1.CompareByCurrentY(line, inputLine);
                    //    }
                    //};
                    if (!above)
                        comparisonFunc = inputLine => AboveBelowComparer1.CompareByCurrentY(line, inputLine) < 0;


                    oneLineFound = inputLines.All(comparisonFunc);
                    if (oneLineFound)
                        nearestLines.Add(line);

                    if (!oneLineFound && borderLines != null)//Если нужно заполнять граничные линии
                    {
                        if (borderLines.Count > 0
                            && AboveBelowComparer1.CompareByCurrentY(borderLines.Last(), line) != 0)
                        {
                            borderLines.Clear();//Очистить borderLines если Y не равны
                        }
                        borderLines.Add(line);
                    }

                }
                else//Если одна линия удовлетворяющая условию найдена, то нужно проверить следующую на равенство Y
                if (AboveBelowComparer1.CompareByCurrentY(nearestLines.First(), line) == 0)
                {
                    nearestLines.Add(line);
                }
                else//Если ни одно условие не выполняется то прервать цикл
                {
                    break;
                }

            }
            return nearestLines;


            //return SLIntersectingLines.FirstOrDefault(intersectingLine =>
            //inputLines.All(inputLine =>
            //aboveBelowComparer.Compare(intersectingLine, inputLine) > 0)
            //);
        }

        /// <summary>
        /// Получение линии, которая находится под всеми переданными
        /// </summary>
        /// <param name="inputLines"></param>
        /// <returns></returns>
        //internal ILine GetBelowLine(IEnumerable<ILine> inputLines)
        //{
        //    return SLIntersectingLines.FirstOrDefault(intersectingLine =>
        //    inputLines.All(inputLine =>
        //    aboveBelowComparer.Compare(intersectingLine, inputLine) < 0)
        //    );
        //}


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
                //Для избежания бесконечного цикла добавляем точки пересечения в очередь только если они не меньше по X
                if (intersectionEvent.X>=X)
                {
                    intersectionEvent = EQ.AddEventToQueueAndGetActual(intersectionEvent);
                    intersectionEvent.IntersectingLines.Add(line1);
                    intersectionEvent.IntersectingLines.Add(line2);
                }
            }
        }

        /// <summary>
        /// Перестановка линий в наборе сканирующей прямой при проходе через точку их пересечения
        /// </summary>
        /// <param name="lines"></param>
        public void UpdateIntersectingLinePositions(
            out List<ILine> highestLines, out List<ILine> lowestLines,
            out List<ILine> aboveLines, out List<ILine> belowLines,
            HashSet<ILine> lines)
        {


            //Убрать линии, пересекающиеся в текущем событии, из набора сканирующей линии
            foreach (ILine line in lines)
            {
                SLIntersectingLines.Remove(line);
            }

            
            //AboveBelowComparer при сравнении между собой линий, которые пересекаются в текущем событии дает приращение по X
            //Благодаря этому актуализируется их взаимное расположение
            foreach (ILine line in lines)
            {
                SLIntersectingLines.Add(line);
            }
            highestLines = new List<ILine>();
            lowestLines = new List<ILine>();
            aboveLines = GetNearestLines(lines, true, highestLines);
            belowLines = GetNearestLines(lines, false, lowestLines);
            
        }


        //private void AddToSL(ILine line)
        //{
        //    aboveBelowComparer.IgnoreEqualityToAddEqualElements = true;
        //    SLIntersectingLines.Add(line);
        //    aboveBelowComparer.IgnoreEqualityToAddEqualElements = false;
        //}


        internal class AboveBelowComparer : IComparer<ILine>
        {
            private SweepLine sl;

            /// <summary>
            /// Набор пересекающихся линий. Данный набор заполняется в других методах.
            /// Для актуализации положения линий при переходе через точку их пересечения
            /// при сравнении пересекающихся линий между собой давать приращение положению сканирующей линии
            /// </summary>
            public HashSet<ILine> IntersectingLinesAtCurrentEvent { get; set; }

            public double Increment { get; set; } = 1;


            public int Compare(ILine line1, ILine line2)
            {
                int geometryComparison = CompareByCurrentY(line1, line2);

                //Если линии параллельны и накладываются друг на друга,
                //то должна быть возможность добавлять и удалять их в SortedSet независимо друг от друга.
                //Поэтому для таких линий применяется сравнение по HashCode
                return geometryComparison!=0 ? geometryComparison : line1.GetHashCode().CompareTo(line2.GetHashCode());
            }

            public int CompareByCurrentY(ILine line1, ILine line2)
            {
                double currX = sl.X;
                if (
                    IntersectingLinesAtCurrentEvent != null
                    && IntersectingLinesAtCurrentEvent.Contains(line1)
                    && IntersectingLinesAtCurrentEvent.Contains(line2)
                    )
                {
                    //Дать приращение X для перерасстановки линий в точке пересечения 
                    currX += Increment;
                }

                //Вертикальная линия не должна попадать в набор!!!
                double den1 = line1.Pt2.X - line1.Pt1.X;
                double den2 = line2.Pt2.X - line2.Pt1.X;
                if (den1 != 0 && den2 != 0)
                {
                    //координата Y первой линии 
                    double y1 = (currX - line1.Pt1.X) * (line1.Pt2.Y - line1.Pt1.Y) / den1 + line1.Pt1.Y;
                    //координата Y второй линии 
                    double y2 = (currX - line2.Pt1.X) * (line2.Pt2.Y - line2.Pt1.Y) / den2 + line2.Pt1.Y;

                    //Нужно сравнивать приблизительно из-за погрешности в вычислениях
                    int comparisonResult = Math.Round(y1, 10).CompareTo(Math.Round(y2, 10));

                    return comparisonResult;

                }
                else
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
