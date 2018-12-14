using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DwarfCastles
{
    /// <summary>
    /// naive implementation, should be based on a priority heap or min heap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private readonly List<T> items;
        private bool sorted;
        public int Count => items.Count;

        public PriorityQueue()
        {
            items = new List<T>();
        }

        public void Enqueue(T t)
        {
            items.Add(t);
            sorted = false;
        }

        public T Dequeue()
        {
            if (!sorted)
            {
                items.Sort();
                sorted = true;
            }

            var result = items.First();
            items.Remove(result);
            
            return result;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}