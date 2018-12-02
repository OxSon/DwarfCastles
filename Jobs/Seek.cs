using System.Drawing;
using System.Security.Policy;

namespace DwarfCastles.Jobs
{
    public class Seek : Task
    {
        private string type;
        private double amount;
        public Entity Found { get; private set; }

        public Seek(Actor actor, Point location, string type, double amount) : base(actor, location,
            ResourceMasterList.GetDefault(type).GetTag("harvestable.workrequired").Value.GetDouble())
        {
            this.type = type;
            this.amount = amount;
        }

        public override void Work()
        {
            Entity resource = null;
            int index = 0;

            do
            {
                Entity next = Actor.Map.Entities[index];
                index++;

                if (next.Name == type)
                    resource = next;
            } while (resource == null);

            Found = resource;
            PickUp(resource);
        }
        
        

        private void PickUp(Entity resource)
        {
            if (Actor.inventory.TryGetValue(resource.Name, out double current))
            {
                Actor.inventory[resource.Name] += current;
            }
            else
            {
                Actor.inventory.Add(type, amount);
            }
        }
    }
}