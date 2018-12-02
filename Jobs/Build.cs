using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Build : Job
    {
        public string Name { get; }

        public Build(Point location, string name, Actor actor) : base(location, actor)
        {
            Name = name;

            var resources = ResourceMasterList.GetDefault(name).GetTag("buildable.resources").SubTags;

            foreach (var r in resources)
            {
//                Logger.Log($"Resource: {r}");
                //TODO check inventory for resource
                string query = r.GetTag("type") == null ? "name" : "type";
                
                var seek = new Seek(Actor, location, r.GetTag(query).Value.GetString(),
                    r.GetTag("amount").Value.GetDouble());
                SubJobs.Enqueue(seek);
                seek.Work();
                //TODO maybe add Found later?
                SubJobs.Enqueue(new Harvest(Actor, Location, seek.Found));
            }

            SubJobs.Enqueue(new Construct(Actor, location, name));
        }

        public override void Finish()
        {
            
        }
    }
}