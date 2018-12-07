using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace DwarfCastles
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// observer and input handler for game
    /// </summary>
    public class GameManager
    {
        public static Map ActiveMap { get; private set; }
        private Gui Gui { get; }
        private bool Running = true;
        private bool Paused = false;

        public static MenuManager Menu;
        public static InputManager Input;

        public GameManager(Map activeMap, Gui gui)
        {
            ActiveMap = activeMap;
            Gui = gui;
            Menu = new MenuManager();
            Input = new InputManager();
            GenerateStartingResources();
            Run();
        }

        
        /// <summary>
        /// Sets up a thread for game updates,
        /// and then loops for user input and gui draw
        /// </summary>
        private void Run()
        {
            Thread childThread = new Thread(GameUpdate);
            childThread.Start();
            
            while (Running)
            {
                HandleInput();
                Gui.Draw(ActiveMap, Menu, Input);
                Thread.Sleep(50);
            }
        }

        public void GenerateStartingResources()
        {
            for (int i = 0; i < 100; i++)
            {
                ActiveMap.AddEntity(ResourceMasterList.GetDefaultClone("wood"), new Point(12, 12));
            }
        }

        private void GameUpdate()
        {
            while (Running)
            {
                Logger.Log("Entering Update Method in GameManager.cs");
                if (!Paused)
                {
                    var snapShot = new List<Entity>();
                    foreach (var e in ActiveMap.Entities)
                    {
                        snapShot.Add(e);
                    }

                    foreach (var e in snapShot)
                    {

                        if (e is Actor)
                        {
                            var a = (Actor) e;
                            a.Update();
                        }
                    }
                }
                else
                {
                    Logger.Log("Game was paused, exiting");
                }
                Thread.Sleep(200);
            }
        }

        private void HandleInput()
        {
            Input.HandleUserInput(Menu);
        }
    }
}