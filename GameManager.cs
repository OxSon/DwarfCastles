using System;

namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// observer and input handler for game
    /// </summary>
    public class GameManager
    {
        public GameManager(Map map, Gui gui)
        {
            Map = map;
            Gui = gui;
        }

        public Map Map { get; }
        public Gui Gui { get; }

        public void Update()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void HandleInput()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}