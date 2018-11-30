namespace DwarfCastles
{
    public class Program 
    {
        public static void Main()
        {
            ResourceParser.ParseFile("Entities/Entity.json");
            //GameManager game = new GameManager(new Map(), new Gui());
        }
    }
}