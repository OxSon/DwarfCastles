using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Harvest : Job
    {
        private Entity Resource;

        private double WorkRequired;

        public Harvest(Entity resource) 
        {
            Resource = resource;
            Location = resource.Pos;
            WorkRequired = resource.GetTag("harvestable.workrequired").Value.GetDouble();
        }

        public override void Work()
        {
            Logger.Log("Working on Harvest job");
            WorkRequired--;
            if (WorkRequired <= 0)
            {
                Finish();
            }
        }

        public override void Finish()
        {
            Owner.Map.RemoveEntityById(Resource.Id);
            var yield = Resource.GetTag("harvestable.yield").SubTags;
            foreach (var y in yield)
            {
                for (var i = y.GetTag("amount").Value.GetDouble(); i > 0; i--)
                {
                    Owner.Map.AddEntity(ResourceMasterList.GetDefaultClone(y.GetTag("name").Value.GetString()), Resource.Pos);
                }
            }

            Completed = true;
        }
    }
}