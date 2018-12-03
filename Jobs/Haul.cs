using System.Collections.Generic;
using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Haul : Job
    {
        public IList<Entity> Carried;

        private readonly IList<int> EntitiesToHaul;

        private readonly Point LocationToHaulTo;

        public Haul(Point locationToHaulTo, IList<int> entitiesToHaulById, Actor a)
        {
            Carried = new List<Entity>();
            EntitiesToHaul = entitiesToHaulById;
            LocationToHaulTo = locationToHaulTo;
            foreach (var i in entitiesToHaulById)
            {
                a.Map.GetEntityById(i).Locked = true;
            }
        }

        public override void TakeOwnership(Actor a)
        {
            Owner = a;
            GenerateNextLocation();
        }

        public void GenerateNextLocation()
        {
            if (EntitiesToHaul.Count != 0)
            {
                var next = Owner.Map.GetEntityById(EntitiesToHaul[0]);
                Location = next.Pos;
            }
            else
            {
                Logger.Log("Haul job is now headed to final drop location");
                Location = LocationToHaulTo;
            }
        }

        public override void ReleaseOwnership()
        {
            Owner.Map.AddEntities(Carried, Owner.Pos);
            foreach (var e in Carried)
            {
                e.Locked = false;
            }

            Owner = null;
            Completed = true;
        }

        public override void Work()
        {
            if (EntitiesToHaul.Count == 0)
            {
                Logger.Log("Haul job finishing!");
                Finish();
            }
            else
            {
                Logger.Log($"Haul.Work Picking up entity with id {EntitiesToHaul[0]}");
                Carried.Add(Owner.Map.GetEntityById(EntitiesToHaul[0]));
                Owner.Map.RemoveEntityById(EntitiesToHaul[0]);
                EntitiesToHaul.RemoveAt(0);
                GenerateNextLocation();
            }
        }
    }
}