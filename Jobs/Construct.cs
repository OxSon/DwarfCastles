using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles.Jobs
{
    public abstract class Construct : Job
    {
        protected IList<Tag> ResourceTags;
        private IList<Entity> CapturedResources;

        protected string Name;
        protected Point Site;
        protected double WorkRequired;

        protected Construct()
        {
            CapturedResources = new List<Entity>();
        }

        public override void Work()
        {
            if (SubJob == null)
            {
                Logger.Log("Construct.Work doing self work", Logger.DEBUG);
                WorkRequired--;
                if (WorkRequired <= 0)
                {
                    Logger.Log("Construct.Work Finishing", Logger.DEBUG);
                    Finish();
                }
            }
            else
            {
                Logger.Log("Construct.Work doing SubJob.Work", Logger.DEBUG);
                SubJob.Work();
                Location = SubJob.GetLocation();
                if (SubJob.Completed)
                {
                    Logger.Log("Construct.Work SubJob Completed", Logger.DEBUG);
                    var j = (Haul) SubJob;
                    foreach (var c in j.Carried)
                    {
                        CapturedResources.Add(c);
                    }

                    j.Carried = new List<Entity>();
                    j.ReleaseOwnership();

                    SubJob = null;
                    GenerateNextStep();
                }
            }
        }
        
        protected override void Finish()
        {
            Logger.Log("Construct.Finish");
            Owner.Map.AddEntity(ResourceMasterList.GetDefaultClone(Name), Site);
            Completed = true;
        }
        
        public override Point GetLocation()
        {
            if (SubJob == null)
            {
                Logger.Log("Construct job returning it's own location", Logger.DEBUG);
                return Location;
            }
            Logger.Log("Construct job returning Subjob Location", Logger.DEBUG);
            return SubJob.GetLocation();
        }

        protected void CollectResource(Tag resourceTag)
        {
            Logger.Log("Construct.CollectResource", Logger.DEBUG);
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
                    Logger.Log("Found the correct amount of resources needed to build", Logger.DEBUG);
                    break;
                }
            }
            Logger.Log($"Construct.CollectResource found {found} need {amount}");

            if (found == amount)
            {
                Logger.Log("Construct.CollectResource Creating new Haul Subjob", Logger.DEBUG);
                SubJob = new Haul(Site, entityIds, Owner);
                SubJob.TakeOwnership(Owner);
            }
        }

        protected static bool Matches(Tag ResourceTag, Entity e)
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

        protected Tag NextRequiredResource()
        {
            foreach (var r in ResourceTags)
            {
                Logger.Log($"Construct.NextRequiredResource for resource {r}", Logger.VERBOSE);
                var capturedAmount = CountMatchingResources(r);
                Logger.Log($"Construct.NextRequiredResource Found {capturedAmount} of the resource", Logger.VERBOSE);
                if (r.GetTag("amount").Value.GetDouble() > capturedAmount)
                {
                    Logger.Log("Construct.NextRequiredResource Returning resource as the next needed", Logger.VERBOSE);
                    return r;
                }
            }

            Logger.Log("Construct.NextRequiredResource Returning null as no more resources are needed",Logger.VERBOSE);
            return null;
        }

        protected int CountMatchingResources(Tag t)
        {
            return CapturedResources.Count(r => Matches(t, r));
        }

        protected void GenerateNextStep()
        {
            Logger.Log("Construct.GenerateNextStep", Logger.DEBUG);
            var next = NextRequiredResource();
            if (next == null)
            {
                Logger.Log("Construct.GenerateNextStep No more resources required. Setting Location to Site", Logger.DEBUG);
                Location = Site;
            }
            else
            {
                
                CollectResource(next);
                
                if (SubJob == null)
                {
                    Logger.Log("Interrupting Owner in Construct.GenerateNextStep, because there is no way to collect the resource needed", Logger.DEBUG);
                    Owner.Interrupt();
                }
            }
        }
    }
}