namespace DwarfCastles
{
    public class Program 
    {
        public static void Main()
        {
            // ResourceParser.ParseFile("Entities/Entity.json");
            //TODO JOSH i will never stop fighting the battle of the var; game on sucker
            var game = new GameManager(new Map(), new Gui());
        }
    }
}