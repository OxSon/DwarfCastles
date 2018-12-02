using System.Drawing;
using System.Security.Policy;

namespace DwarfCastles.Jobs
{
    public class Seek : Task
    {
        private readonly string typeOrName;
        private readonly double amount;
        public Entity Found { get; private set; }

        public Seek(Actor actor, Point location, string type, double amount) : base(actor, location, 0)
            
        {
            typeOrName = type;
            this.amount = amount;
        }

        public override void Work()
        {
            Logger.Log("Seek : Work");
            int index = 0;

            if (index < Actor.Map.Entities.Count)
            {
                do
                {
                    Entity next = null;

                    if (index < Actor.Map.Entities.Count)
                    {
                        next = Actor.Map.Entities[index];
                        index++;

                        if (next?.GetTag("harvestable") != null)
                        {
                            Logger.Log("Seek : Work -> Found resource! :)");
                            string query = next.GetTag("type") == null ? "name" : "type";

                            if (next.GetTag(query).Value.GetString() == typeOrName)
                            {
                                Found = next;
                            }
                            else
                            {
                                Logger.Log("Seek: next was null");
                            }
                        }
                    }
                } while (Found == null) ;

                PickUp(Found);
            }

        }
        
        

        private void PickUp(Entity resource)
        {
            if (Actor.inventory.TryGetValue(resource.Name, out double current))
            {
                Actor.inventory[resource.Name] += current;
            }
            else
            {
                Actor.inventory.Add(typeOrName, amount);
            }
        }
    }
}