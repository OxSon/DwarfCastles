using System.Collections.Generic;
using System.Drawing;

namespace DwarfCastles.Jobs
{
    public sealed class Haul : Job
    {
        private readonly IList<int> EntitiesToHaul;
        private readonly Point LocationToHaulTo;
        
        public IList<Entity> Carried;

        public override Actor Owner
        {
            get => owner;
            set
            {
                owner = value;
                
                if (value != null)
                {
                    GenerateNextLocation();
                }
                else //setting null Owner value represents release of ownership
                {
                    Owner.Map.AddEntities(Carried, Owner.Pos);
                    foreach (var e in Carried)
                    {
                        e.Locked = false;
                    }

                    Finish();
                }
            }
        }

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