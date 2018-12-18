using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles.Jobs
{
    public class Craft : Construct
    {
        private bool CraftAtLocation;

        public Craft(string itemName)
        {
            Name = itemName;
            var craftable = ResourceMasterList.GetDefault(itemName).GetTag("craftable");
            ResourceTags = craftable.GetTag("resources").SubTags;
            WorkRequired = craftable.GetTag("workrequired").Value.GetDouble();

            try
            {
                var station = craftable.GetTag("station").Value.GetString();
                foreach (var e in GameManager.ActiveMap.Entities)
                {
                    if (e.Name != station) continue;
                    Site = e.Pos;
                    break;
                }
            }
            catch (Exception)
            {
                CraftAtLocation = true;
            }
        }

        public override void TakeOwnership(Actor a)
        {
            Owner = a;
            if (CraftAtLocation)
            {
                Site = a.Pos;
                CraftAtLocation = false;
            }

            GenerateNextStep();
        }
    }
}