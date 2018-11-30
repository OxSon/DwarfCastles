using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DwarfCastles;

namespace DwarfFortress
{
    /// <inheritdoc/>
    public class Actor : Entity
    {
        private Queue<Point> currentTravelPath;
        private static int counter;
        public PriorityQueue<Task> Tasks { get; }
        public Map Map { get; } //current map Actor is on

        public Actor(string name, Point pos, char ascii, Map map,
            ConsoleColor backgroundColor = ConsoleColor.Black,
            ConsoleColor foregroundColor = ConsoleColor.White) :
            base(name, pos, ascii, backgroundColor, foregroundColor)
        {
            Map = map;
            Tasks = new PriorityQueue<Task>();
        }

        public void Update()
        {
            Logger.Log("Update Method for Actor");
            Logger.Log("Task Count: " + Tasks.Count);
            if (Tasks.Count == 0 || Tasks.First().Location.Equals(Pos)) return;
            Logger.Log("Next step");
            //recheck our pathing every 5 moves, or if we don't currently have a path
            if (currentTravelPath == null || counter > 4)
            {
                currentTravelPath = new Queue<Point>(GenTravelPath(Map.Impassables, Tasks.First()));
                Logger.Log(Tasks.Count + "");
                counter = 0;
            }
            else
                counter++;
            
            Logger.Log("Update for Actor moving from (" + Pos.X + ", " + Pos.Y + ") to");
            Pos = currentTravelPath.Dequeue();
            Logger.Log("(" + Pos.X + ", " + Pos.Y + ")");
        }
        
        
        
        private IEnumerable<Point> GenTravelPath(bool[,] impassables, Task task)
        {
            if (!CanMove(impassables)) return null;

            var points = new Queue<Point>();
            points.Enqueue(Pos);

            var cameFrom = new Dictionary<Point, Point>(); //used to construct our path at the end
            cameFrom.Add(points.Peek(), points.Peek()); //indicate that our starting node "came" from nowhere

            Point current;
            //generate list of paths
            while (points.Count > 0)
            {
                current = points.Dequeue();
                
                foreach (var point in AdjacentPoints(current))
                {
                    if (!cameFrom.ContainsKey(point) && !Map.Impassables[point.X, point.Y])
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

            return adjacents.Any(s => Map.InBounds(s) && !impassables[s.X, s.Y]);
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

            return rawAdjacents.Where(point => Map.InBounds(point)).ToList();
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