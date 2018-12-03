using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Seek : Job
    {
        private readonly string typeOrName;
        private readonly double amount;
        public Entity Found { get; private set; }

        public Seek(Actor owner, Point location, string type, double amount) 
            
        {
            typeOrName = type;
            this.amount = amount;
        }

        public override void Work()
        {
            Logger.Log("Seek : Work");
            int index = 0;

            if (index < Owner.Map.Entities.Count)
            {
                do
                {
                    Entity next = null;

                    if (index < Owner.Map.Entities.Count)
                    {
                        next = Owner.Map.Entities[index];
                        index++;

                        if (next.GetTag("harvestable") != null)
                        {
                            Logger.Log("Seek : Work -> Found resource! :)");
                            
                            var harvestable = next.GetTag("harvestable");
                            
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
        }
    }
}