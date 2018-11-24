using System;

namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// handles drawing in console
    /// </summary>
    public class Gui
    {
        public Point CameraOffset { get; }

        public Gui()
        {
            CameraOffset = new Point();
        }

        public void Draw(Map map)
        {
            Console.CursorVisible = false;
            foreach (Entity e in map.Entities)
            {
                if (map.InBounds(e.Pos.Add(CameraOffset)))
                {
                    Console.SetCursorPosition(e.Pos.X - CameraOffset.X, e.Pos.Y - CameraOffset.Y);
                    Console.Write(e.Ascii);
                }
            }
        }
    }
}