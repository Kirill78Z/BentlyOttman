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
        private AboveBelowComparer aboveBelowComparer;
        internal EventQueue EQ;

        public SweepLine(IEnumerable<ILine> lines)
        {
            aboveBelowComparer = new AboveBelowComparer(this);
            SLIntersectingLines = new SortedSet<ILine>(aboveBelowComparer);

            EQ = new EventQueue();

            EQ.Populate(lines);

        }


        private List<SLEvent> output;
        internal List<SLEvent> TraverseEventQueue()
        {
            output = new List<SLEvent>();
            EQ.VisitedEvents.Clear();
            while (EQ.Count > 0)
            {
                SLEvent e = EQ.First();

                int eventKey = EQ.GetKey(e);
                if (!EQ.VisitedEvents.Contains(eventKey))
                {
                    GoToEvent(e);
                    EQ.VisitedEvents.Add(eventKey);
                }
                    

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
            if (e.LeftEndPtLines.Count + e.RightEndPtLines.Count > 1
                || e.IntersectingLines.Count > 0)
            {
                output.Add(e);
            }

        }



        private void RightEndpointsProcessing(SLEvent e)
        {
            HashSet<ILine> rightEndpointLines = new HashSet<ILine>(e.RightEndPtLines);
            //Если в наборе есть вертикальные линии, то их нужно убрать
            rightEndpointLines.RemoveWhere(l => l.Pt1.X == l.Pt2.X);

            if (rightEndpointLines.Count > 0)
            {
                List<ILine> aboveLines = GetNearestLines(rightEndpointLines, true);
                List<ILine> belowLines = GetNearestLines(rightEndpointLines, false);
                foreach (ILine rpl in rightEndpointLines)
                {
                    aboveBelowComparer.ComparisonToDelete = true;
                    SLIntersectingLines.Remove(rpl);
                    aboveBelowComparer.ComparisonToDelete = false;
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

        private void LeftEndpointsProcessing(SLEvent e)
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


        private void VerticalLineProcessing(ILine vertLine)
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

        private void IntersectionPointsProcessing(SLEvent e)
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
        /// которая находится над всеми переданными
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

            bool oneLineFounded = false;
            foreach (ILine line in collToIterate)
            {
                if (!oneLineFounded)
                {
                    Func<ILine, bool> comparisonFunc = inputLine => aboveBelowComparer.Compare1(line, inputLine) > 0;
                    if (!above)
                        comparisonFunc = inputLine => aboveBelowComparer.Compare1(line, inputLine) < 0;


                    oneLineFounded = inputLines.All(comparisonFunc);
                    if(oneLineFounded)
                        nearestLines.Add(line);

                    if (!oneLineFounded && borderLines != null)//Если нужно заполнять граничные линии
                    {
                        if (borderLines.Count > 0
                            && aboveBelowComparer.Compare1(borderLines.Last(), line) != 0)
                        {
                            borderLines.Clear();//Очистить borderLines если Y не равны
                        }
                        borderLines.Add(line);
                    }

                }
                else//Если одна линия удовлетворяющая условию найдена, то нужно проверить следующую на равенство Y
                if (aboveBelowComparer.Compare1(nearestLines.First(), line) == 0)
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
                intersectionEvent = EQ.AddEventToQueueAndGetActual(intersectionEvent);
                intersectionEvent.IntersectingLines.Add(line1);
                intersectionEvent.IntersectingLines.Add(line2);
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

            aboveBelowComparer.IntersectingLinesAtCurrentEvent = lines;
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

            aboveBelowComparer.IntersectingLinesAtCurrentEvent = null;
        }


        private class AboveBelowComparer : IComparer<ILine>
        {
            private SweepLine sl;

            /// <summary>
            /// Набор пересекающихся линий. Данный набор заполняется в методе UpdateIntersectingLinePositions
            /// Для актуализации положения линий при переходе через точку их пересечения
            /// при сравнении пересекающихся линий между собой давать приращение положению сканирующей линии
            /// </summary>
            public HashSet<ILine> IntersectingLinesAtCurrentEvent { get; set; }

            /// <summary>
            /// В те моменты, когда нужно удалить объект из SortedSet Compare должен возвращать равенство объектов
            /// для того чтобы удалить конкретный объект
            /// </summary>
            public bool ComparisonToDelete { get; set; }
            public int Compare(ILine line1, ILine line2)
            {

                int yComparison = Compare1(line1,line2);

                //Набор линий, пересекаемых секущей должен хранить линии которые накладываются друг на друга
                //Для таких линий y1 = y2. Чтобы SortedSet сохранил оба значения нужно возвращать не  ноль, а произвольное значение
                return yComparison != 0 || ComparisonToDelete ? yComparison : 1;
            }

            /// <summary>
            /// Сравнение, которое может показывать равенство объектов
            /// Не подходит для заполнения SortedSet, так как SortedSet не хранит несколько равных объектов
            /// </summary>
            /// <param name="line1"></param>
            /// <param name="line2"></param>
            /// <returns></returns>
            public int Compare1(ILine line1, ILine line2)
            {
                int yComparison = 0;
                if (IntersectingLinesAtCurrentEvent != null
                    && IntersectingLinesAtCurrentEvent.Contains(line1)
                    && IntersectingLinesAtCurrentEvent.Contains(line2))
                {
                    yComparison = Compare2(line1, line2, true);
                }
                else
                {
                    yComparison = Compare2(line1, line2);

                    //Если сравнение без приращения X показало равенство, то повторить сравнение с приращением в любом случае
                    //Необходимо для случаев т-образных пересечений
                    if (yComparison == 0)
                    {
                        yComparison = Compare2(line1, line2, true);
                    }
                }


                
                return yComparison;
            }


            public int Compare2(ILine line1, ILine line2, bool incrementX = false)
            {
                double currX = sl.X;
                if (incrementX)
                {
                    currX += 1;
                }

                //(x - x_1 )/(x_2 - x_1 ) = (y - y_1 )/(y_2 - y_1) - уравнение прямой

                //Вертикальная линия не должна попадать в набор!!!
                double den1 = line1.Pt2.X - line1.Pt1.X;
                double den2 = line2.Pt2.X - line2.Pt1.X;
                if (den1 != 0 && den2 != 0)
                {
                    //координата Y первой линии 
                    double y1 = (currX - line1.Pt1.X) * (line1.Pt2.Y - line1.Pt1.Y) / den1 + line1.Pt1.Y;
                    //координата Y второй линии 
                    double y2 = (currX - line2.Pt1.X) * (line2.Pt2.Y - line2.Pt1.Y) / den2 + line2.Pt1.Y;

                    return y1.CompareTo(y2);

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
