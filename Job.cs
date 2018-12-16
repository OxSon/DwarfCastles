using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles
{
    public abstract class Job
    {
        protected Point Location { get; set; }

        protected Queue<Job> SubJobs { get; }

        protected Actor Owner { get; set; }

        public bool Completed;

        protected Job()
        {
            SubJobs = new Queue<Job>();
        }

        /// <summary>
        /// This method is called when a job is taken from
        /// the queue to be worked on by an actor
        /// </summary>
        /// <param name="a"></param>
        public virtual void TakeOwnership(Actor a)
        {
            Owner = a;
        }

        /// <summary>
        /// This method is used to release a job and
        /// making it ready for a new actor to take it
        /// </summary>
        public virtual void ReleaseOwnership()
        {
        }

        /// <summary>
        /// This method is called whenever the owning actor
        /// is at the location and ready to do the work
        /// </summary>
        public virtual void Work()
        {
        }

        protected virtual void Finish()
        {
            Completed = true;
        }

        public virtual Point GetLocation()
        {
            return Location;
        }

        public bool NextToLocation(Point p)
        {
            if (p.X == GetLocation().X && p.Y == GetLocation().Y)
            {
                return true;
            }
            return Owner.Map.AdjacentPoints(GetLocation()).Any(x => x.X == p.X && x.Y == p.Y);
        }

        public IEnumerable<Point> GenTravelPath()
        {
            Logger.Log($"Job.GenTravelPath: Entered to Location ({Location.X}, {Location.Y})");
            Logger.Log(Owner.Map.Impassables[GetLocation().X, GetLocation().Y] + " is the value of passable for the location");
            if (!Owner.CanMove())
            {
                Logger.Log("Job.GenTravelPath: Returning: Can't Move");
                return null;
            }

            var q = new Queue<Point>();
            q.Enqueue(Owner.Pos);

            var origin = new Dictionary<Point, Point?> {{Owner.Pos, null}};

            Point current;

            while (q.Count != 0)
            {
                current = q.Dequeue();
                Logger.Log($"({current.X}, {current.Y}) Being searched");

                foreach (var child in Owner.Map.AdjacentPoints(current))
                {
                    if (!origin.ContainsKey(child))
                    {
                        q.Enqueue(child);
                        origin.Add(child, current);
                    }
                    else
                    {
                        Logger.Log($"Refusing to add ({child.X}, {child.Y})");
                    }

                    if (child == GetLocation() || Owner.Map.Impassables[GetLocation().X, GetLocation().Y] && NextToLocation(child))
                    {
                        Logger.Log("Job.GenTravelPath: Returning");
                        return ToPath(origin, child);
                    }
                }
            }

            Logger.Log($"Pathing failed: last node processed: {current}; goal: {GetLocation()}");

            return null;
        }

        private IEnumerable<Point> ToPath(Dictionary<Point, Point?> mappings, Point goal)
        {
            Logger.Log("Task.ToPath : entering");
            var current = goal;
            var path = new List<Point> {goal};

            while (current != Owner.Pos)
            {
                path.Add(current);
                if (mappings.TryGetValue(current, out var result) && result != null)
                    current = (Point) result;
                else
                    Logger.Log("null value in ToPath");
            }

            path.Reverse();

            Logger.Log("Task.ToPath : returning");
            return from p in path select p;
        }
    }
}