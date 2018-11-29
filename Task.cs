using System;
using System.Drawing;

namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// TODO unknown, josh what is this?
    /// </summary>
    public class Task : IComparable
    {
        public int ActionId { get; }
        public Point Location { get; }
        //TODO currently always 0, does not yet work
        private int priority;

        public Task(int action, Point location)
        {
            Location = location;
            ActionId = action;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Task))
                throw new ArgumentException();

            var other = (Task) obj;
            
            if (other.priority > priority)
                return -1;
            
            return other.priority == priority ? 0 : 1;
        }
    }
}