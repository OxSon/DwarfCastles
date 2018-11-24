using System;
using System.CodeDom;
using System.Drawing;
using System.Threading;

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
            Run();
        }

        public Map Map { get; }
        public Gui Gui { get; }

        private bool Running = true;
        
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
                        
                        Task t = new Task(0, nextPos);
                        a.Tasks.Add(t);
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