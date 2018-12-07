using System;

namespace DwarfCastles.Jobs
{
    public sealed class Eat : Job
    {
        private int foodRequired = 5;
        private int sateRate = 1;
        
        private ConsoleColor origFg;
        private const ConsoleColor tempFg = ConsoleColor.Green;
        private ConsoleColor[] colors;
        private int colorIndex;

        public Eat(Actor owner)
        {
            Owner = owner;
            //TODO need to set a propeer eating location 
            Location = owner.Pos;
//            Location = owner.Map.MessHall;
            origFg = owner.ForegroundColor;
            colors = new[] {origFg, tempFg};
        }
        
        public override void Work()
        {
            Logger.Log($"{owner} is eating; food required = {foodRequired}");
            //flash between two colors
            colorIndex = (colorIndex + 1) % 2;
            owner.ForegroundColor = colors[colorIndex];
            
            foodRequired -= sateRate;
            if (foodRequired <= 0)
                Finish();
        }
        
        protected override void Finish()
        {
            Logger.Log($"{owner} is finishing eating");
            owner.Hunger = 0;
            Completed = true;
        }
    }
}