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
        public Queue<Job> Jobs { get; } = new Queue<Job>();
        public Map Map { get; } //current map Actor is on
        public IDictionary<string, double> inventory { get; }

        public Actor()
        {
        }

        public Actor(string name, Point pos, char ascii, Map map,
            ConsoleColor backgroundColor = ConsoleColor.Black,
            ConsoleColor foregroundColor = ConsoleColor.White) :
            base(name, pos, ascii, backgroundColor, foregroundColor) {
                Map = map;
                inventory = new Dictionary<string, double>();
        }

        public void Update()
        {
            Logger.Log("Update Method for Actor");

            var updateables = GetTag("updateables");
            Logger.Log("Found updateables to be non-null");

            if (updateables != null)
            {
                foreach (var tag in updateables.SubTags)
                {
                    var value = tag.GetTag("value").Value;
                    var rate = tag.GetTag("rate").Value;
                    value.SetValue(value.GetDouble() - rate.GetDouble());
                }

//                Logger.Log("Actor.Update : exited foreach loop");
            }

            Logger.Log("Task Count: " + Jobs.Count);

            if (Jobs.Count == 0) return;

            if (Jobs.First().Location.Equals(Pos))
            {
                Jobs.First().Work();
                if (Jobs.First().WorkRequired <= 0)
                    Jobs.First().Finish();
                return;
            }

            Logger.Log("Next step");
            //recheck our pathing every 5 moves, or if we don't currently have a path
            if (currentTravelPath == null || counter > 4)
            {
                var attemptedPath = Jobs.First().GenTravelPath();
                if (attemptedPath != null)
                    currentTravelPath = new Queue<Point>(attemptedPath);
                else
                    Logger.Log($"Destination unreachable; Dq'ing: {Jobs.Dequeue()}");

                Logger.Log(Jobs.Count + "");
                counter = 0;
            }
            else
                counter++;

            Logger.Log("Update for Actor moving from (" + Pos.X + ", " + Pos.Y + ") to");
            if (currentTravelPath != null)
                Pos = currentTravelPath.Dequeue();
            Logger.Log("(" + Pos.X + ", " + Pos.Y + ")");
        }

        public void Travel(Point location)
        {
        }

        public bool CanMove()
        {
            return Map.AdjacentPoints(Pos).Any(s => Map.InBounds(s) && !Map.Impassables[s.X, s.Y]);
        }
    }
}