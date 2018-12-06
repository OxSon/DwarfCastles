using System;

namespace DwarfCastles.Jobs
{
    public sealed class Eat : Job
    {
        private int foodRequired = 40;
        private int sateRate = 10;
        
        private ConsoleColor origFg;
        private const ConsoleColor tempFg = ConsoleColor.Green;
        private ConsoleColor[] colors;
        private int colorIndex;

        public Eat()
        {
            Location = owner.Map.MessHall;
            origFg = owner.ForegroundColor;
            colors = new[] {origFg, tempFg};
        }
        
        public override void Work()
        {
            //flash between two colors
            colorIndex = (colorIndex + 1) % 2;
            owner.ForegroundColor = colors[colorIndex];
            
            foodRequired -= sateRate;
            if (foodRequired <= 0)
                Finish();
        }
        
        protected override void Finish()
        {
            owner.Hunger = 0;
            owner.Jobs.Dequeue();
        }
    }
}