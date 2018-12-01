using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles
{
    public abstract class Job
    {
        public Point Location { get; protected set; }
        protected Queue<Job> SubJobs { get; }
        public double WorkRequired { get; protected set; }

        public Actor Actor { get; set; }

        protected Job(Point location, Actor actor)
        {
            Actor = actor;
            Location = location;
            SubJobs = new Queue<Job>();
        }

        public virtual void Work()
        {
            foreach (var job in SubJobs)
            {
                job.Work();
            }
        }

        public virtual void Finish()
        {
            WorkRequired = 0;
        }
        
        public IEnumerable<Point> GenTravelPath()
        {
//            Logger.Log("Task.GenTravelPath: ");
            Logger.Log(Actor.Map.Impassables[Location.X, Location.Y] + " IS THE VALUE OF IMPASSABLE FOR THE LOCATION");
            Logger.Log("Task.GenTravelPath: Entered");
            if (!Actor.CanMove())
            {
                Logger.Log("Task.GenTravelPath: Returning: Can't Move");
                return null;
            }

            var q = new Queue<Point>();
            q.Enqueue(Actor.Pos);

            var origin = new Dictionary<Point, Point?> {{Actor.Pos, null}};

            Point current;
            
            while (q.Count != 0)
            {
                current = q.Dequeue();
                
                foreach (var child in Actor.Map.AdjacentPoints(current))
                {
                    if (!origin.ContainsKey(child))
                    {
                        q.Enqueue(child);
                        origin.Add(child, current);
                    }

//                    Logger.Log($"Comparing {child} to {Location} -> {child == Location}");

                    if (child == Location)
                    {
                        Logger.Log("Task.GenTravelPath: Returning");
                        return ToPath(origin, Location);
                    }
                }
            }

//            Logger.Log($"Testing: (2, 3) == (2, 3) ? : {new Point(2, 3) == new Point(2, 3)}");
            Logger.Log($"Pathing failed: last node processed: {current}; goal: {Location}");

            return null;
        }

        private IEnumerable<Point> ToPath(Dictionary<Point, Point?> mappings, Point goal)
        {
            Logger.Log("Task.ToPath : entering");
            var current = goal;
            var path = new List<Point> {goal};

            while (current != Actor.Pos)
            {
                path.Add(current);
                Point? result;
                if (mappings.TryGetValue(current, out result) && result != null)
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