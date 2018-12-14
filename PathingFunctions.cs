using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace DwarfCastles
{
    public static class PathingFunctions
    {
        public struct Position :  IComparable<Position>
        {
            public Point p { get; set; }
            private double v { get; set; }

            public Position(Point point, double priority)
            {
                p = point;
                v = priority;
            }

            public int CompareTo(Position other)
            {
                return v < other.v ? -1 : v > other.v ? 1 : 0;
            }
        }
        public static IEnumerable<Point> GenerateAStarPath(Point to, Point from, Map map)
        {
            var q = new PriorityQueue<Position>();
            q.Enqueue(new Position(from, 0));
            var cameFrom = new Dictionary<Point, Point?>();
            var costSoFar = new Dictionary<Point, double>();
            cameFrom.Add(from, null);
            costSoFar.Add(from, 0);

            Point? NextPointToAdd;
            
            while (q.Count != 0)
            {
                var current = q.Dequeue();

                if (current.p == to || map.Impassables[current.p.X, current.p.Y] && NextToLocation(current.p, to))
                {
                    NextPointToAdd = current.p;
                    break;
                }

                foreach (var neighbor in map.PassableAdjacents(current.p))
                {
                    var totalCost = costSoFar[current.p] + map.costOfTile(neighbor);
                    if (!costSoFar.ContainsKey(neighbor) || totalCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = totalCost;
                        q.Enqueue(new Position(neighbor, totalCost + Heuristic(neighbor, to)));
                        cameFrom[neighbor] = current.p;
                    }
                }
            }
            
            var Path = new List<Point>();
            while (true)
            {
                Path.Add((Point)NextPointToAdd);
                NextPointToAdd = cameFrom[(Point)NextPointToAdd];
                if (NextPointToAdd == null)
                {
                    break;
                }
            }
            return Path;
        }

        private static double Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
        
        private static bool NextToLocation(Point a, Point b)
        {
            if (a.X == b.X && a.Y == b.Y)
            {
                return true;
            }
            return GameManager.ActiveMap.AdjacentPoints(a).Any(x => x.X == b.X && x.Y == b.Y);
        }
    }
}