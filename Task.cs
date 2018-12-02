using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    /// special case of a Job that is atomic and cannot break down further
    /// </summary>
    public class Task : Job, IComparable
    {
        //TODO 
//        public int ActionId { get; }
        public double WorkRequired { get; protected set; }

        //TODO currently always 0, does not yet work
        private int priority;

        public Task(Actor actor,Point location,  double workRequired ) : base(location, actor)
        {
//            ActionId = action;
            Actor = actor;
            WorkRequired = workRequired;
            SubJobs.Enqueue(this);
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