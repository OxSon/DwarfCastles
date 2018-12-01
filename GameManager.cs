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
<<<<<<< Updated upstream
        private static ConcurrentQueue<Job> Tasks;
=======
        private static ConcurrentQueue<Task> Tasks;
        private MenuManager Menu;
        private InputManager Input;
        
>>>>>>> Stashed changes

        public GameManager(Map map, Gui gui)
        {
            Map = map;
            Gui = gui;
<<<<<<< Updated upstream
            Tasks = new ConcurrentQueue<Job>();
=======
            Tasks = new ConcurrentQueue<Task>();
            Menu = new MenuManager();
            Input = new InputManager();
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                if (e is Actor a)
                {
                    if (a.Jobs.Count == 0)
                    {
                        var r = new Random();
                        
                        Point nextPos;
                        do
                        {
                            nextPos = new Point(r.Next(0, Map.Size.X), r.Next(0, Map.Size.Y));
                        } while (Map.Impassables[nextPos.X,nextPos.Y]);
                        
                        Job j = new Build(nextPos, "forge");
                        a.Jobs.Enqueue(j);
                        j.Actor = a;
                    }
                    a.Update();
=======
                if (!(e is Actor)) 
                    continue;
                
                Actor a = (Actor) e;
                if (a.Tasks.Count == 0)
                {
                    var r = new Random();
                        
                    var nextPos = new Point();
                    do
                    {
                        nextPos = new Point(r.Next(0, Map.Size.X), r.Next(0, Map.Size.Y));
                    } while (Map.Impassables[nextPos.X,nextPos.Y]);
                        
                    var t = new Task(0, nextPos, a);
                    a.Tasks.Enqueue(t);
>>>>>>> Stashed changes
                }
                a.Update();
            }
            Gui.Draw(Map, Menu);
        }

        private void HandleInput()
        {
            Input.HandleUserInput(Menu);
        }
    }
}