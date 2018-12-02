using System;
using System.Collections.Concurrent;
using System.Drawing;
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
        private Map Map { get; }
        private Gui Gui { get; }
        private bool Running = true;
        public static ConcurrentQueue<Job> Tasks { get; } = new ConcurrentQueue<Job>();

        private MenuManager Menu;
        private InputManager Input;

        public GameManager(Map map, Gui gui)
        {
            Map = map;
            Gui = gui;
            Menu = new MenuManager();
            Input = new InputManager();
            Run();
        }

        public void AddTask(Job task)
        {
            Tasks.Enqueue(task);
        }

        public Job GetTask()
        {
            return Tasks.TryDequeue(out var result) ? result : null;
        }


        private void Run()
        {
            while (Running)
            {
                Update();
                Thread.Sleep(1000);
            }
        }

        private void Update()
        {
            HandleInput();
            Logger.Log("Entering Update Method in GameManager.cs");
            
            foreach (var e in Map.Entities)
            {
                if (e is Actor a)
                {
                    if (Tasks.Count < 5)
                    {
                        Tasks.Enqueue(new Seek(a, "ironvein", 10));
                    }
                }

//            {
//                if (e is Actor a)
//                {
//                    if (a.Jobs.Count == 0)
//                    {
//                        var r = new Random();
//
//                        Point nextPos;
//                        do
//                        {
//                            nextPos = new Point(r.Next(0, Map.Size.X), r.Next(0, Map.Size.Y));
//                        } while (Map.Impassables[nextPos.X, nextPos.Y]);
//
//                        Job j = new Build(nextPos, "forge", a);
//
//                        a.Jobs.Enqueue(j);
//                    }
//
//                    a.Update();
//                }
            }

            Gui.Draw(Map, Menu);
        }

        private void HandleInput()
        {
            Input.HandleUserInput(Menu);
        }
    }
}