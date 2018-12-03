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
        public Map Map { get; set; } //current map Actor is on

        public void Update()
        {
            Logger.Log("Update Method for Actor");
            var updateables = GetTag("updateables");
            if (updateables != null)
            {
                Logger.Log("Found updateables to be non-null");
                foreach (var tag in updateables.SubTags)
                {
                    var value = tag.GetTag("value").Value;
                    var rate = tag.GetTag("rate").Value;
                    value.SetValue(value.GetDouble() - rate.GetDouble());
                }
            }

            Logger.Log("Task Count: " + Jobs.Count);
            if (Jobs.Count == 0)
            {
                if (GameManager.Jobs.TryDequeue(out var newJob))
                {
                    Logger.Log("Actor taking job from GameManager.Jobs");
                    newJob.TakeOwnership(this);
                    Jobs.Enqueue(newJob);
                }

                //TODO Generate a wander Task
                return;
            }

            if (Jobs.First().NextToLocation(Pos))
            {
                Jobs.First().Work();
                if (Jobs.First().Completed)
                {
                    Jobs.First().ReleaseOwnership();
                    Jobs.Dequeue();
                    return;
                }
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
                if (currentTravelPath.Count > 0)
                    Pos = currentTravelPath.Dequeue();
            Logger.Log("(" + Pos.X + ", " + Pos.Y + ")");
        }

        public override Entity Clone()
        {
            Entity a = new Actor
            {
                Name = Name, Ascii = Ascii, BackgroundColor = BackgroundColor, ForegroundColor = ForegroundColor, Display = Display
            };
            foreach (var tag in Tags)
            {
                a.AddTag(tag.Clone());
            }

            return a;
        }

        public bool CanMove()
        {
            return Map.AdjacentPoints(Pos).Any(s => Map.InBounds(s) && !Map.Impassables[s.X, s.Y]);
        }
    }
}