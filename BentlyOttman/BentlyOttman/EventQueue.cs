using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BentlyOttman
{
    /// <summary>
    /// Очередь событий
    /// </summary>
    internal class EventQueue : IEnumerable<SLEvent>
    {
        /// <summary>
        /// Собственно очередь событий
        /// </summary>
        private SortedSet<SLEvent> EQ;



        /// <summary>
        /// Dictionary событий. Хранит те же значения что очередь
        /// В качестве ключа используется хэшкод кортежа координат
        /// Нужен для быстрого поиска события по координатам
        /// </summary>
        private Dictionary<int, SLEvent> eventDictionary;
        /// <summary>
        /// Получение ключа для словаря
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private int GetKey(SLEvent e)
        {
            return (new Tuple<double, double>(e.X, e.Y)).GetHashCode();
        }

        public EventQueue()
        {
            EQ = new SortedSet<SLEvent>();
            eventDictionary = new Dictionary<int, SLEvent>();
            
        }


        /// <summary>
        /// Заполнение очереди событий начальными значениями
        /// Поместить вершины линий в дерево 	Red-black tree
        /// Точки отсортированы по возрастанию X. Если X одинаковые, то по возрастанию Y
        /// http://c-sharp-snippets.blogspot.ru/2010/03/runtime-complexity-of-net-generic.html
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public void Populate(IEnumerable<ILine> lines)
        {
            foreach (ILine line in lines)
            {
                int vertsComparison = line.Pt1.CompareTo(line.Pt2);
                if (vertsComparison != 0)//Если линия вырождается в точку, то ее не рассматривать
                {
                    SLEvent pt1ActualEvent = AddEventToQueue(line.Pt1);
                    if (vertsComparison > 0)
                    {
                        pt1ActualEvent.RightEndPtLines.Add(line);
                    }
                    else
                    {
                        pt1ActualEvent.LeftEndPtLines.Add(line);
                    }

                    SLEvent pt2ActualEvent = AddEventToQueue(line.Pt2);
                    if (vertsComparison < 0)
                    {
                        pt2ActualEvent.RightEndPtLines.Add(line);
                    }
                    else
                    {
                        pt2ActualEvent.LeftEndPtLines.Add(line);
                    }


                }
            }


        }


        /// <summary>
        /// Добавление нового события в очередь если еще не добавлено
        /// Событие хранится так же в Dictionary, синхронизированном с очередью
        /// </summary>
        /// <param name="eventToAdd"></param>
        /// <returns>ссылка на значение содержащееся в очереди</returns>
        public SLEvent AddEventToQueue(SLEvent eventToAdd)
        {
            SLEvent actualEvent = null;
            int key = GetKey(eventToAdd);
            eventDictionary.TryGetValue(key, out actualEvent);
            if (actualEvent == null)
            {
                actualEvent = eventToAdd;
                eventDictionary.Add(key, eventToAdd);
                EQ.Add(eventToAdd);
            }
            return actualEvent;
        }


        




        public int Count { get { return EQ.Count; } }

        public bool Remove(SLEvent e)
        {
            bool r1 = EQ.Remove(e);
            bool r2 = eventDictionary.Remove(GetKey(e));
            return r1 && r2;
        }

        public IEnumerator<SLEvent> GetEnumerator()
        {
            return EQ.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EQ.GetEnumerator();
        }
    }
}
