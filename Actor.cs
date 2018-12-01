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
            
            var updateables = GetTag("updateables");
            Logger.Log("Found updateables to be non-null");
            
            if (updateables != null)
            {
                foreach (var tag in updateables.SubTags)
                {
                    var value = tag.GetTag("value").Value;
                    var rate = tag.GetTag("rate").Value;
                    value.setValue(value.GetDouble() - rate.GetDouble());
                }
                Logger.Log("Actor.Update : exited foreach loop");
            }

            Logger.Log("Task Count: " + Tasks.Count);
            
            if (Tasks.Count == 0 || Tasks.First().Location.Equals(Pos)) return;
            
            Logger.Log("Next step");
            //recheck our pathing every 5 moves, or if we don't currently have a path
            if (currentTravelPath == null || counter > 4)
            {
                currentTravelPath = new Queue<Point>(Tasks.First().GenTravelPath());
                Logger.Log(Tasks.Count + "");
                counter = 0;
            }
            else
                counter++;

            Logger.Log("Update for Actor moving from (" + Pos.X + ", " + Pos.Y + ") to");
            //TODO fixme
//            Pos = currentTravelPath.Dequeue();
            Logger.Log("(" + Pos.X + ", " + Pos.Y + ")");
        }

        public bool CanMove()
        {
            return Map.AdjacentPoints(Pos).Any(s => Map.InBounds(s) && !Map.Impassables[s.X, s.Y]);
        }

    }
}