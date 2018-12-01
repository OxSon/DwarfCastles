using System;
using System.Collections.Concurrent;
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
        public Map Map { get; }
        public Gui Gui { get; }
        private bool Running = true;
        private static ConcurrentQueue<Task> Tasks;

        public GameManager(Map map, Gui gui)
        {
            Map = map;
            Gui = gui;
            Tasks = new ConcurrentQueue<Task>();
            Run();
        }

        public void AddTask(Task task)
        {
            Tasks.Enqueue(task);
        }

        public Task GetTask()
        {
            return Tasks.TryDequeue(out var result) ? result : null;
        }

        
        public void Run()
        {
            while (Running)
            {
                Update();
                Thread.Sleep(1000);
            }
        }

        public void Update()
        {
            Logger.Log("Entering Update Method in GameManager.cs");
            foreach (Entity e in Map.Entities)
            {
                if (e is Actor)
                {
                    Actor a = (Actor) e;
                    if (a.Tasks.Count == 0)
                    {
                        Random r = new Random();
                        
                        Point nextPos = new Point();
                        do
                        {
                            nextPos = new Point(r.Next(0, Map.Size.X), r.Next(0, Map.Size.Y));
                        } while (Map.Impassables[nextPos.X,nextPos.Y]);
                        
                        Task t = new Task(0, nextPos, a);
                        a.Tasks.Enqueue(t);
                    }
                    a.Update();
                }
            }
            Gui.Draw(Map);
        }

        public void HandleInput()
        {
        }
    }
}