using System.Drawing;

namespace DwarfCastles
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    /// special case of a Job that is atomic and cannot break down further
    /// </summary>
    public class Task : Job
    {
        //TODO 
//        public int ActionId { get; }

        public Task(Actor actor,Point location,  double workRequired ) : base(location, actor)
        {
//            ActionId = action;
            Actor = actor;
            WorkRequired = workRequired;
            SubJobs.Enqueue(this);
        }
    }
}