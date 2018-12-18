using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
            return ResourcesCaptured.Count(r => Matches(t, r));
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
                if (SubJob == null)
                {
                    Owner.Interrupt();
                }
            }
        }

        public override Point GetLocation()
        {
            if (SubJob == null)
            {
                Logger.Log("Build job returning it's own location");
                return Location;
            }
            Logger.Log("Build job returning Subjob Location");
            return SubJob.GetLocation();
        }

        public override void Work()
        {
            if (SubJob == null)
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
                SubJob.Work();
                Location = SubJob.GetLocation();
                if (SubJob.Completed)
                {
                    if (SubJob is Haul)
                    {
                        var j = (Haul) SubJob;
                        foreach (var c in j.Carried)
                        {
                            ResourcesCaptured.Add(c);
                        }

                        j.Carried = new List<Entity>();
                        j.ReleaseOwnership();
                    }

                    SubJob = null;
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
                SubJob = new Haul(CraftSite, entityIds, Owner);
                SubJob.TakeOwnership(Owner);
            }
        }

        private static bool Matches(Tag ResourceTag, Entity e)
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

        protected override void Finish()
        {
            Owner.Map.AddEntity(ResourceMasterList.GetDefaultClone(ItemName), CraftSite);
            Completed = true;
        }
    }
}
