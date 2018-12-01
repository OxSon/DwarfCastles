using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DwarfCastles
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
        private Actor Actor { get; }

        //TODO currently always 0, does not yet work
        private int priority;

        public Task(int action, Point location)
        {
            Location = location;
            ActionId = action;
        }

        public IEnumerable<Point> GenTravelPath()
        {
            if (!Actor.CanMove())
                return null;

            var q = new Queue<Point>();
            q.Enqueue(Location);

            var origin = new Dictionary<Point, Point?> {{Location, null}};

            while (q.Count != 0)
            {
                var current = q.Dequeue();
                
                foreach (var child in Actor.Map.AdjacentPoints(current))
                {
                    if (!origin.ContainsKey(child))
                    {
                        origin.Add(child, current);
                    }

                    if (child == Location)
                        return ToPath(origin, Location);
                }
            }

            Logger.Log("failed to find goal while pathing");
            
            throw new EvaluateException();
        }

        private IEnumerable<Point> ToPath(Dictionary<Point, Point?> mappings, Point goal)
        {
            var current = goal;
            var path = new List<Point>(){goal};

            while (mappings.TryGetValue(current, out var next))
            {
                if (next != null)
                    path.Add((Point) next);
            }

            path.Reverse();

            return from p in path select p;
        }


        /// <summary>
        /// used to see which of n points is closest to another point, without needing to know each exact distance
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns>the absolute value of the square of the distance from origin to destination </returns>
        private static int DistanceHeuristic(Point origin, Point destination)
        {
            return Math.Abs((int) Math.Round(Math.Pow(destination.X - origin.X, 2) +
                                             Math.Pow(destination.Y - origin.Y, 2)));
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