namespace DwarfCastles
{
    public static class Program 
    {
        public static void Main()
        {
            ResourceMasterList.LoadAllResources();
            //ResourceParser.ParseFile("Entities/Entity.json");
            var game = new GameManager(new Map(), new Gui());
        }
    }
}