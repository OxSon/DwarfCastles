using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace DwarfFortress
{
    /// <inheritdoc/>
    public class Actor : Entity
    {
        private Queue<Point> currentTravelPath;
        private static int counter;
        //TODO Josh what is this? needs more functionality I assume?
        public IEnumerable<Task> Tasks { get; }
        public Map map { get; } //current map Actor is on
        
        public Actor(string name, Point pos, char ascii,
            ConsoleColor backgroundColor = ConsoleColor.Black,
            ConsoleColor foregroundColor = ConsoleColor.White) :
            base(name, pos, ascii, backgroundColor, foregroundColor) { }

        public void Update()
        {
            if (Tasks.First().Location.Equals(Pos)) return;
            
            //recheck our pathing every 5 moves, or if we don't currently have a path
            if (currentTravelPath == null || counter > 4)
            {
                currentTravelPath = new Queue<Point>(GenTravelPath(map.impassables, Tasks.First()));
                counter = 0;
            }
            else
                counter++;

            Pos = currentTravelPath.Dequeue();
        }
        
        private IEnumerable<Point> GenTravelPath(bool[,] impassables, Task task)
        {
            if (!CanMove(impassables)) return null;

            var points = new Queue<Point>();
            points.Enqueue(Pos);

            //TODO unnecessary?
            // var visited = new bool[map.Size.Y, map.Size.X];
            
            var cameFrom = new Dictionary<Point, Point>(); //used to construct our path at the end
            cameFrom.Add(points.Peek(), points.Peek()); //indicate that our starting node "came" from nowhere

            Point current;
            //generate list of paths
            while (points.Count > 0)
            {
                current = points.Dequeue();
                
                foreach (var point in AdjacentPoints(current))
                {
                    if (!cameFrom.ContainsKey(point))
                    {
                        points.Enqueue(point);
                        cameFrom.Add(point, current);
                    }
                }
            }
            
            var path = new Stack<Point>();
            current = task.Location;

            while (current != Pos)
            {
                path.Push(current);
                cameFrom.TryGetValue(current, out current);
            }
            
            return path.ToList();
        }
        
        private bool CanMove(bool[,] impassables)
        {
            var adjacents = new[]
            {
                    new Point(Pos.X - 1, Pos.Y),
                    new Point(Pos.X + 1, Pos.Y),
                    new Point(Pos.X, Pos.Y - 1),
                    new Point(Pos.X, Pos.Y + 1)
            };

            return adjacents.Any(s => map.InBounds(s) && !impassables[s.X, s.Y]);
        }

        private IEnumerable<Point> AdjacentPoints(Point origin)
        {
            var rawAdjacents = new[]
            {
                    new Point(origin.X - 1, origin.Y),
                    new Point(origin.X + 1, origin.Y),
                    new Point(origin.X, origin.Y - 1),
                    new Point(origin.X, origin.Y + 1)
            };

            return rawAdjacents.Where(point => map.InBounds(point)).ToList();
        }

        /// <summary>
        /// used to see which of n points is closest to another point, without needing to know each exact distance
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns>the absolute value of the square of the distance from origin to destination </returns>
        private static int RelativeDistanceTo(Point origin, Point destination)
        {
            return Math.Abs((int) Math.Round(Math.Sqrt(destination.X - origin.X) + Math.Sqrt(destination.Y - origin.Y)));
        }

    }
}