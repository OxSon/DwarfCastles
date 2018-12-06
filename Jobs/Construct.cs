using System.Drawing;

namespace DwarfCastles.Jobs
{
    public sealed class Construct : Job
    {
        private string ConstructName;//TODO this is never assigned
        private double WorkRequired;//TODO this is assigned but never accessed

        public Construct(Point location, string constructName)
        {
            Location = location;
            WorkRequired = ResourceMasterList.GetDefault(constructName).GetTag("buildable.workrequired").Value.GetDouble();
        }

        public override void Work()
        {
            WorkRequired--;
        }

        protected override void Finish()
        {
            Owner.Map.AddEntity(ResourceMasterList.GetDefaultClone(ConstructName), Location);
        }
    }
}