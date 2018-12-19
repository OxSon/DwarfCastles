using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DwarfCastles.Jobs;

namespace DwarfCastles
{
    public class Actor : Entity
    {
        private Queue<Point> currentTravelPath;
        private static int counter;
        private Job Job { get; set; }
        public Map Map { get; set; } //current map Actor is on

        public void Update()
        {
            Logger.Log($"Actor.Update for Actor ID={Id}");
            var updateables = GetTag("updateables");
            if (updateables != null)
            {
                Logger.Log("Actor.Update Found updateables to be non-null");
                foreach (var tag in updateables.SubTags)
                {
                    var value = tag.GetTag("value").Value;
                    var rate = tag.GetTag("rate").Value;
                    value.SetValue(value.GetDouble() - rate.GetDouble());
                }
            }

            var count = 0;
            if (Job == null && count < Map.Jobs.Count)
            {
                if (Map.Jobs.TryDequeue(out var newJob))
                {
                    Logger.Log("Actor.Update taking job from Map.Jobs");
                    Job = newJob;
                    newJob.TakeOwnership(this);
                }
                count++;
            }

            if (Job == null)
            {
                Job = new Wander(this);
            }
            
            if (Job.NextToLocation(Pos))
            {
                Logger.Log("Actor.Update is working on Job");
                Job.Work();
                if (Job == null || !Job.Completed) return;
                Logger.Log("Actor.Update is completing a job");
                Job.ReleaseOwnership();
                Job = null;
            }
            else
            {
                Logger.Log("Actor.Update Entering Travel portion");
                if (currentTravelPath == null || counter > 4 || (currentTravelPath.Count > 0 &&
                    Map.Impassables[currentTravelPath.Peek().X, currentTravelPath.Peek().Y]))
                {
                    var attemptedPath = Job.GenTravelPath();
                    if (attemptedPath != null)
                    {
                        Logger.Log($"Actor.Update Path created successfully with Length {attemptedPath.Count()}");
                        currentTravelPath = new Queue<Point>(attemptedPath);
                    }
                    else
                    {
                        Logger.Log("Actor.Update Interrupting Actor because path was not successful", Logger.DEBUG);
                        Interrupt();
                    }

                    counter = 0;
                }
                else
                    counter++;
                
                if (currentTravelPath == null) return;
                if (currentTravelPath.Count == 0) return;
                Logger.Log("Updating Position");
                Pos = currentTravelPath.Dequeue();
            }
            // END ======================================================================
        }

        public void Interrupt()
        {
            var returnToQueue = Job;
            Job = null;
            Map.AddTask(returnToQueue);
        }

        public override Entity Clone()
        {
            Entity a = new Actor
            {
                Name = Name, Ascii = Ascii, BackgroundColor = BackgroundColor, ForegroundColor = ForegroundColor,
                Display = Display
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