namespace DwarfCastles
{
    public class Program 
    {
        public static void Main()
        {
            ResourceMasterList.LoadAllResources();
            //ResourceParser.ParseFile("Entities/Entity.json");
            GameManager game = new GameManager(new Map(), new Gui());
        }
    }
}