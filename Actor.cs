using System;
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
        public Map Map { get; set; } //current map Actor is on
        
        private Queue<Job> standardJobs = new Queue<Job>();
        private Queue<Job> overrideJobs = new Queue<Job>();
        public Queue<Job> Jobs => overrideJobs.Count > 0 ? overrideJobs : standardJobs;
        
       
        private int hunger; //values increase overtime
        private const int hungerThreshold = 40; //point at which an actor must eat
        private const int hungerRate = 10; //rate at which hunger increases
        public int Hunger
        {
            get => hunger;
            set
            {
                hunger = value;
                //if we're beyond hunger threshold and do not have an eat job in our queue yet add one
                if (hunger >= hungerThreshold && !(overrideJobs.Any(j => j is Eat)))
                    overrideJobs.Enqueue(new Eat());
            }
        }
        
        private int exhaustion; //values increase overtime
        private const int exhaustionThreshold = 40; //point at which an actor must eat
        private const int exhaustionRate = 5; //rate at which exhaustion increases
        public int Exhaustion
        {
            get => exhaustion;
            set
            {
                exhaustion = value;
                if (exhaustion >= exhaustionThreshold && !(overrideJobs.Any(j => j is Sleep)))
                    overrideJobs.Enqueue(new Sleep());
            }
        }

        public void Update()
        {
            Logger.Log("Update Method for Actor");
            Exhaustion += exhaustionRate;
            Hunger += hungerRate;
            
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
                if (Map.Jobs.TryDequeue(out var newJob))
                {
                    Logger.Log("Actor taking job from GameManager.Jobs");
                    Jobs.Enqueue(newJob);
                    newJob.Owner = this;
                } else
                {
                    Jobs.Enqueue(new Wander(this));
                }
                return;
            }
            Logger.Log("Checking if Actor should work on job");
            if (Jobs.First().NextToLocation(Pos))
            {
                Logger.Log("Actor is working on Job");
                Jobs.First().Work();
                if (Jobs.First().Completed)
                {
                    Logger.Log("Actor is completing a job");
                    Jobs.First().Owner = null;
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

            if (currentTravelPath != null)
            {
                if (currentTravelPath.Count > 0)
                {
                    string logText = $"Update for Actor moving from ({Pos.X}, {Pos.Y}) to";
                    Pos = currentTravelPath.Dequeue();
                    Logger.Log($"{logText} ({Pos.X}, {Pos.Y})");
                }
            }
        }

        public void Inturrupt()
        {
            Job returnToQueue = Jobs.Dequeue();
            Map.AddTask(returnToQueue);
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