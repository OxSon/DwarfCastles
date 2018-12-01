using System.Collections.Generic;

namespace DwarfCastles
{
    public abstract class Job
    {
        public IList<Task> Tasks;

        public abstract void Work();
    }
}