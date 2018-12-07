using System;

namespace DwarfCastles.Jobs
{
    public sealed class Sleep : Job
    {
        private int sleepRequired = 20;
        private const int sleepRate = 1;

        private ConsoleColor origFg;
        private const ConsoleColor tempFg = ConsoleColor.Blue;
        private ConsoleColor[] colors;
        private int colorIndex;

        public Sleep(Actor owner)
        {
            Owner = owner;
            Location = owner.Pos; //we don't move for this job
            origFg = owner.ForegroundColor;
            colors = new[] {origFg, tempFg};
        }

        public override void Work()
        {
            Logger.Log($"{owner} is sleeping; sleep required = {sleepRequired}");
            //flash between two colors
            colorIndex = (colorIndex + 1) % 2;
            owner.ForegroundColor = colors[colorIndex];
           
            sleepRequired -= sleepRate;
            if (sleepRequired <= 0)
                Finish();
        }

        protected override void Finish()
        {
            Logger.Log($"{owner} is finishing sleeping");
            owner.ForegroundColor = origFg;
            owner.Exhaustion = 0;
            Completed = true;
        }
    }
}