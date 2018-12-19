using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles.Jobs
{
    public class Build : Construct
    {

        public Build(Point location, string buildingName)
        {
            Name= buildingName;
            Site = location;
            var buildable = ResourceMasterList.GetDefault(buildingName).GetTag("buildable");
            ResourceTags = buildable.GetTag("resources").SubTags;
            WorkRequired = buildable.GetTag("workrequired").Value.GetDouble();
        }


        public override void TakeOwnership(Actor a)
        {
            Owner = a;
            Owner.Map.Impassables[Site.X, Site.Y] = true; // When someone starts working on it, make the location impassable
            GenerateNextStep();
            if (Location == Site || SubJob != null) return;
            Logger.Log("Interrupting Owner in Build Job", Logger.DEBUG);
            Owner.Interrupt();
        }
    }
}