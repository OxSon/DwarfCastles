using DwarfFortress;

namespace DwarfCastles
{
    public class Program 
    {
        public static void Main()
        {
             ResourceParser.ParseFile("BuildingObjects.info");
            GameManager game = new GameManager(new Map(), new Gui());
        }
    }
}