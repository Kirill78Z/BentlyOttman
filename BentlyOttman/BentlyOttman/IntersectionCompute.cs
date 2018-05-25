using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BentlyOttman
{
    public static class IntersectionCompute
    {
        /// <summary>
        /// >0 for pt left of the line
        /// =0 for pt on the line
        /// <0 for pt right of the line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static double IsLeft(ILine line, SLEvent pt)
        {
            return ((line.Pt2.X - line.Pt1.X) * (pt.Y - line.Pt1.Y) - (pt.X - line.Pt1.X) * (line.Pt2.Y - line.Pt1.Y));
        }

        /// <summary>
        /// Проверка двух отрезков на наличие пересечения
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool AreIntersecting(ILine line1, ILine line2)
        {
            //Если линии стыкуются по одному из концов, то данный метод возвращает false
            //Стыкующиеся линии все равно попадают в output алгоритма
            if (
                line1.Pt1.CompareTo(line2.Pt1)==0
                || line1.Pt2.CompareTo(line2.Pt1) == 0
                || line1.Pt1.CompareTo(line2.Pt2) == 0
                || line1.Pt2.CompareTo(line2.Pt2) == 0
                )
            {
                return false;
            }

            int p3IsLeft = Math.Sign(IsLeft(line1, line2.Pt1));
            int p4IsLeft = Math.Sign(IsLeft(line1, line2.Pt2));
            int p1IsLeft = Math.Sign(IsLeft(line2, line1.Pt1));
            int p2IsLeft = Math.Sign(IsLeft(line2, line1.Pt2));

            if ((p3IsLeft == 0 && p4IsLeft == 0) || (p1IsLeft == 0 && p2IsLeft == 0))//Отрезки лежат на одной прямой
            {
                //В этом случае считаем, что пересечений нет
                return false;
            }

            return
            p3IsLeft != p4IsLeft//Точки второй линии находятся по разные стороны от первой
            && p1IsLeft != p2IsLeft//Точки первой линии находятся по разные стороны от второй
            ;
        }

        /// <summary>
        /// Расчет точки пересечения двух отрезков
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static SLEvent GetIntersectionPt(ILine line1, ILine line2)
        {

            double x1_x2 = line1.Pt1.X - line1.Pt2.X;
            double y3_y4 = line2.Pt1.Y - line2.Pt2.Y;
            double y1_y2 = line1.Pt1.Y - line1.Pt2.Y;
            double x3_x4 = line2.Pt1.X - line2.Pt2.X;
            double denominator = x1_x2 * y3_y4 - y1_y2 * x3_x4;
            if (denominator != 0)
            {
                double x1y2_y1x2 = line1.Pt1.X * line1.Pt2.Y - line1.Pt1.Y * line1.Pt2.X;
                double x3y4_y3x4 = line2.Pt1.X * line2.Pt2.Y - line2.Pt1.Y * line2.Pt2.X;
                double intersectX = (x1y2_y1x2 * x3_x4 - x1_x2 * x3y4_y3x4)
                    / denominator;
                double intersectY = (x1y2_y1x2 * y3_y4 - y1_y2 * x3y4_y3x4)
                    / denominator;
                return new SLEvent(intersectX, intersectY);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Перегрузка для нахождения пересечения с вертикальной линией
        /// </summary>
        /// <param name="Y1"></param>
        /// <param name="Y2"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static SLEvent GetIntersectionPt(double line1X, ILine line2)
        {
            //(x - x_1 )/(x_2 - x_1 ) = (y - y_1 )/(y_2 - y_1) - уравнение отрезка
            double den = line2.Pt2.X - line2.Pt1.X;
            if (den!=0)
            {
                double intersectionY = (line1X - line2.Pt1.X) * (line2.Pt2.Y - line2.Pt1.Y) / den + line2.Pt1.Y;
                return new SLEvent(line1X, intersectionY);
            }
            else
            {
                return null;
            }

        }
    }
}
