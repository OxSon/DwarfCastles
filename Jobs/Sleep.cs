using System;

namespace DwarfCastles.Jobs
{
    public sealed class Sleep : Job
    {
        private int sleepRequired = 40;
        private const int sleepRate = 10;

        private ConsoleColor origFg;
        private const ConsoleColor tempFg = ConsoleColor.Blue;
        private ConsoleColor[] colors;
        private int colorIndex;

        public Sleep()
        {
            Location = owner.Pos; //we don't move for this job
            origFg = owner.ForegroundColor;
            colors = new[] {origFg, tempFg};
        }

        public override void Work()
        {
            //flash between two colors
            colorIndex = (colorIndex + 1) % 2;
            owner.ForegroundColor = colors[colorIndex];
           
            sleepRequired -= sleepRate;
            if (sleepRequired <= 0)
                Finish();
        }

        protected override void Finish()
        {
            owner.ForegroundColor = origFg;
            owner.Exhaustion = 0;
            owner.Jobs.Dequeue();
        }
    }
}