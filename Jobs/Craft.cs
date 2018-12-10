using System;
using System.Collections.Generic;
using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Craft : Job
    {
        public string ItemName { get; }
        private IList<Tag> ResourcesRequired;

        private IList<Entity> ResourcesCaptured;

        private Point CraftSite;
        private bool CraftAtLocation;
        private double WorkRequired;

        public Craft(string buildingName)
        {
            ResourcesCaptured = new List<Entity>();
            ItemName = buildingName;
            var craftable = ResourceMasterList.GetDefault(buildingName).GetTag("craftable");
            ResourcesRequired = craftable.GetTag("resources").SubTags;
            WorkRequired = craftable.GetTag("workrequired").Value.GetDouble();
            var station = craftable.GetTag("station").Value.GetString();
            try
            {
                station = craftable.GetTag("station").Value.GetString();
            }
            catch (Exception e)
            {
                station = "none";
            }

            if (station == "none")
            {
                CraftAtLocation = true;
            }
            else
            {
                foreach (var e in GameManager.ActiveMap.Entities)
                {
                    if (e.Name == station)
                    {
                        CraftSite = e.Pos;
                        break;
                    }
                }
            }
        }
        
        

        private Tag NextRequiredResource()
        {
            foreach (var r in ResourcesRequired)
            {
                Logger.Log($"Craft.NextRequiredResource for resource {r}");
                var capturedAmount = CountMatchingResources(r);
                Logger.Log($"Craft.NextRequiredResource Found {capturedAmount} of the resource");
                if (r.GetTag("amount").Value.GetDouble() > capturedAmount)
                {
                    Logger.Log("Craft.NextRequiredResource Returning resource as the next needed");
                    return r;
                }
            }
            Logger.Log($"Craft.NextRequiredResource Returning null as no more resources are required");
            return null;
        }

        public int CountMatchingResources(Tag t)
        {
            int count = 0;

            foreach (var r in ResourcesCaptured)
            {
                if (Matches(t, r))
                {
                    count++;
                }
            }

            return count;
        }

        public override void TakeOwnership(Actor a)
        {
            Owner = a;
            if (CraftAtLocation)
            {
                CraftSite = a.Pos;
                CraftAtLocation = false;
            }
            GenerateNextStep();
        }

        public void GenerateNextStep()
        {
            var next = NextRequiredResource();
            if (next == null)
            {
                Location = CraftSite;
            }
            else
            {
                CollectResource(next);
                if (SubJobs.Count == 0)
                {
                    Owner.Inturrupt();
                }
            }
        }

        public override Point GetLocation()
        {
            if (SubJobs.Count == 0)
            {
                Logger.Log("Build job returning it's own location");
                return Location;
            }
            Logger.Log("Build job returning Subjob Location");
            return SubJobs.Peek().Location;
        }

        public override void Work()
        {
            if (SubJobs.Count == 0)
            {
                Logger.Log("Build doing self work");
                WorkRequired--;
                if (WorkRequired <= 0)
                {
                    Logger.Log("Build Finishing");
                    Finish();
                }
            }
            else
            {
                Logger.Log("Build doing subwork");
                SubJobs.Peek().Work();
                Location = SubJobs.Peek().Location;
                if (SubJobs.Peek().Completed)
                {
                    if (SubJobs.Peek() is Haul)
                    {
                        var j = (Haul) SubJobs.Peek();
                        foreach (var c in j.Carried)
                        {
                            ResourcesCaptured.Add(c);
                        }

                        j.Carried = new List<Entity>();
                        j.ReleaseOwnership();
                    }

                    SubJobs.Dequeue();
                    GenerateNextStep();
                }
            }
        }

        private void CollectResource(Tag resourceTag)
        {
            var amount = resourceTag.GetTag("amount").Value.GetDouble();

            IList<int> entityIds = new List<int>();
            var found = 0;
            foreach (var e in Owner.Map.Entities)
            {
                if (e.Locked)
                {
                    continue;
                }

                var carriable = e.GetTag("attributes.carriable");
                if (carriable != null)
                {
                    if (!carriable.Value.GetBool())
                    {
                        continue;
                    }
                }

                if (Matches(resourceTag, e))
                {
                    entityIds.Add(e.Id);
                    found++;
                }

                if (found == amount)
                {
                    Logger.Log("Found the correct amount of resources needed to build");
                    break;
                }
            }

            if (found == amount)
            {
                SubJobs.Enqueue(new Haul(CraftSite, entityIds, Owner));
                SubJobs.Peek().TakeOwnership(Owner);
                Location = SubJobs.Peek().Location;
            }
        }

        private bool Matches(Tag ResourceTag, Entity e)
        {
            if (ResourceTag.GetTag("type") != null && e.HasType(ResourceTag.GetTag("type").Value.GetString()))
            {
                return true;
            }

            if (ResourceTag.GetTag("name") != null && ResourceTag.GetTag("name").Value.GetString() == e.Name)
            {
                return true;
            }

            return false;
        }

        public override void Finish()
        {
            Owner.Map.AddEntity(ResourceMasterList.GetDefaultClone(ItemName), CraftSite);
            Completed = true;
        }
    }
}