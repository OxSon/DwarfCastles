using System.Drawing;

namespace DwarfCastles.Jobs
{
    public class Build : Job
    {
        public string Name { get; }

        public Build(Point location, string name, Actor actor = null) : base(location, actor)
        {
            Name = name;
            Location = location;

            var resources = ResourceMasterList.GetDefault(name).GetTag("buildable.resources").SubTags;

            foreach (var r in resources)
            {
                //TODO check inventory for resource
                var seek = new Seek(Actor, location, r.Name, r.Value.GetDouble());
                SubJobs.Enqueue(seek);
                SubJobs.Enqueue(new Harvest(Actor, Location, seek.Found));
            }

            SubJobs.Enqueue(new Construct(Actor, location, name));
        }

        public override void Finish()
        {
            
        }
    }
}