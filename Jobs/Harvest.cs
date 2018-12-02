using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Harvest : Task
    {
        private Entity resource;

        public Harvest(Actor actor, Point location, Entity resource) : base(actor, location,
            ResourceMasterList.GetDefault(resource.Name).GetTag("harvestable.workrequired").Value.GetDouble())
        {
            this.resource = resource;
        }

        public override void Work()
        {
            WorkRequired--;
        }

        public override void Finish()
        {
            int id = Actor.Map.Entities.Find(s => s == resource).Id;
            Actor.Map.RemoveEntityById(id);
            
            Actor.Map.AddEntity(ResourceMasterList.GetDefaultClone("harvestable.yield"), Location);
        }
    }
}