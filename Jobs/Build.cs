using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Build : Job
    {
        private double WorkRequired;
        
        public Build(Point Position, string name)
        {
            var workRequired = ResourceMasterList.GetDefault(name).GetTag("buildable.workrequired");
            if (workRequired != null)
            {
                WorkRequired = workRequired.Value.GetDouble();
            }
        }
        
        public override void Work()
        {
            
        }
    }
}