using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Construct : Task
    {
        private string structure;

        public Construct(Actor actor, Point location, string structure) : base(actor, location,
            ResourceMasterList.GetDefault(structure).GetTag("buildable.workrequired").Value.GetDouble())
        {
            Actor = actor;
            var resources = ResourceMasterList.GetDefault(structure).GetTag("buildable.resources").SubTags;
        }

        public override void Work()
        {
            Logger.Log("entered construct work");
            //TODO
        }
    }
}