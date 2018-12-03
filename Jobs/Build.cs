using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles.Jobs
{
    public class Build : Job
    {
        public string BuildingName { get; }
        private IList<Tag> ResourcesRequired;

        private IList<Entity> ResourcesCaptured;

        private Point BuildSite;

        private double WorkRequired;

        public Build(Point location, string buildingName)
        {
            BuildingName = buildingName;
            BuildSite = location;
            var buildable = ResourceMasterList.GetDefault(buildingName).GetTag("buildable");
            ResourcesRequired = buildable.GetTag("resources").SubTags;
            WorkRequired = buildable.GetTag("workrequired").Value.GetDouble();
        }

        private Tag NextRequiredResource()
        {
            foreach (var r in ResourcesRequired)
            {
                var capturedAmount = CountMatchingResources(r);
                if (r.GetTag("amount").Value.GetDouble() > capturedAmount)
                {
                    return r;
                }
            }

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
            GenerateNextStep();
        }

        public void GenerateNextStep()
        {
            var next = NextRequiredResource();
            if (next == null)
            {
                Location = BuildSite;
            }
            else
            {
                CollectResource(next);
            }
        }

        public override void Work()
        {
            if (SubJobs.Count != 0)
            {
                WorkRequired--;
            }
            else
            {
                SubJobs.Peek().Work();
                if (SubJobs.Peek().Completed)
                {
                    if (SubJobs.Peek() is Haul)
                    {
                        var j = (Haul)SubJobs.Peek();
                        foreach (var c in j.Carried)
                        {
                            ResourcesCaptured.Add(c);
                        }
                        j.Carried = new List<Entity>();
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
            Owner.Map.AddEntity(ResourceMasterList.GetDefaultClone(BuildingName));
            Completed = true;
        }
    }
}