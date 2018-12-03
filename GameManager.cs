using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using DwarfCastles.Jobs;

namespace DwarfCastles
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// observer and input handler for game
    /// </summary>
    public class GameManager
    {
        public Map Map { get; }
        private Gui Gui { get; }
        private bool Running = true;
        public static ConcurrentQueue<Job> Jobs { get; } = new ConcurrentQueue<Job>();

        private MenuManager Menu;
        private InputManager Input;

        public GameManager(Map map, Gui gui)
        {
            Map = map;
            Gui = gui;
            Menu = new MenuManager();
            Input = new InputManager();
            Menu.SetManager(this);
            Run();
        }

        public void AddTask(Job task)
        {
            Jobs.Enqueue(task);
        }

        private void Run()
        {
            test();
            while (Running)
            {
                Update();
                Thread.Sleep(200);
            }
        }

        private void HarvestEverything() //TODO Remove this method it was for testing once it is not longer needed ty
        {
            foreach (var e in Map.Entities)
            {
                var harvestable = e.GetTag("harvestable");
                if (harvestable != null)
                {
                    Jobs.Enqueue(new Harvest(e));
                }
            }
        }


        public void test()
        {
            for (int i = 0; i < 100; i++)
            {
                Map.AddEntity(ResourceMasterList.GetDefaultClone("wood"), new Point(12, 12));
            }
        }

        private void Update()
        {
            HandleInput();
            Logger.Log("Entering Update Method in GameManager.cs");
            var snapShot = new List<Entity>();
            foreach (var e in Map.Entities)
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

            Gui.Draw(Map, Menu, Input);
        }

        private void HandleInput()
        {
            Input.HandleUserInput(Menu);
        }
    }
}