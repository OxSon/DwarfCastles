using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles
{
    /// <inheritdoc/>
    public class Actor : Entity
    {
        private Queue<Point> currentTravelPath;
        private static int counter;
        public PriorityQueue<Task> Tasks { get; }
        public Map Map { get; } //current map Actor is on

        public Actor()
        {
        }

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
                currentTravelPath = new Queue<Point>(GenTravelPath(Tasks.First()));
                Logger.Log(Tasks.Count + "");
                counter = 0;
            }
            else
                counter++;

            Logger.Log("Update for Actor moving from (" + Pos.X + ", " + Pos.Y + ") to");
            Pos = currentTravelPath.Dequeue();
            Logger.Log("(" + Pos.X + ", " + Pos.Y + ")");
        }


        private IEnumerable<Point> GenTravelPath(Task task)
        {
            var head = new Path(Pos, Move.Null, task, Map);
            var q = new PriorityQueue<Path>();

            do
            {
                var children = AdjacentPoints(head.pos, Map);

                foreach (var p in children)
                {
                    q.Enqueue(new Path(p, p.Parse(task.Location), task, Map, head));
                }

                head = q.Dequeue();
            } while (head.pos != task.Location);

            return head.Convert();
        }

        public static IEnumerable<Point> AdjacentPoints(Point origin, Map map)
        {
            //all adjacent squares, even if they're outside the bounds of our map
            var options = new List<Point>
            {
                    new Point(origin.X - 1, origin.Y), new Point(origin.X + 1, origin.Y),
                    new Point(origin.X, origin.Y - 1), new Point(origin.X, origin.Y + 1)
            };
            //only return in-bounds points
            return options.Where(p => p.X >= 0 && p.Y >= 0 && p.X <= map.Size.X && p.Y <= map.Size.Y);
        }

        private bool CanMove()
        {
            //returns true if any of the adjacent points are passable otherrwise false
            return AdjacentPoints(Pos).Any(p => !Map.Impassables[p.X, p.Y]);
        }
    }

    public class Path : IComparable
    {
        public Point pos { get; }
        private int priority;
        private Map map;
        private List<Move> prevMoves;
        private Task task;
        private Path prev;

        public Path(Point pos, Move move, Task task, Map map, Path prev = null)
        {
            this.pos = pos;
            this.map = map;
            this.prev = prev;
            this.task = task;
            priority = Map.DistanceHeuristic(pos, task.Location);

            if (prev?.prevMoves != null)
                prevMoves = new List<Move>(prev.prevMoves);
            else
                prevMoves = new List<Move>();

            prevMoves.Add(move);
        }

        public List<Path> choices()
        {
            var result = from p in Actor.AdjacentPoints(pos, map)
                    where !map.Impassables[p.X, p.Y]
                    select new Path(p, pos.Parse(p), task, map, this);

            return result.ToList();
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Path path)) throw new ArgumentException();

            if (path.priority > priority)
                return -1;

            return path.priority == priority ? 0 : 1;
        }

        public IEnumerable<Point> Convert()
        {
            var converted = new List<Point>();

            var head = this;

            do
            {
                converted.Add(head.pos);
                head = prev;
            } while (head.prev != null);

            return converted;
        }
    }

    static class Extensions
    {
        public static Move Parse(this Point origin, Point next)
        {
            //if we're moving upwards
            if (origin.X < next.X)
                return Move.Left;
            if (origin.X > next.X)
                return Move.Right;
            if (origin.Y < next.Y)
                return Move.Down;
            if (origin.Y > next.Y)
                return Move.Up;
        }
    }
}