using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Construct : Job
    {
        private string ConstructName;

        private double WorkRequired;

        private Point Location;
        
        public Construct(Point location, string constructName)
        {
            Location = location;
            WorkRequired = ResourceMasterList.GetDefault(constructName).GetTag("buildable.workrequired").Value.GetDouble();
        }

        public override void Work()
        {
            WorkRequired--;
        }

        public override void Finish()
        {
            Owner.Map.AddEntity(ResourceMasterList.GetDefaultClone(ConstructName), Location);
        }
    }
}