using System;
using System.Drawing;

namespace DwarfCastles.Jobs
{
    public sealed class Wander : Job
    {
        public Wander(Actor a)
        {
            var r = new Random();
            do
            {
                Location = new Point(a.Pos.X + r.Next(7) - 3, a.Pos.Y + r.Next(7) - 3);
            } while (!a.Map.InBounds(Location));

            Owner = a;
        }

        public override void Work()
        {
            Finish();
        }
    }
}