namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// TODO unknown, josh what is this?
    /// </summary>
    public class Task
    {
        public int ActionId { get; }
        public Point Location { get; }

        public Task(int action, Point location)
        {
            Location = location;
            ActionId = action;
        }
    }
}