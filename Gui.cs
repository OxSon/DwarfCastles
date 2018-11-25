using System;
using System.Drawing;

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
            CameraOffset = new Point(0,0);

        }

        public void Draw(Map map)
        {
            Console.CursorVisible = false;
            foreach (var e in map.Entities)
            {
                if (map.InBounds(Point.Add(e.Pos, new Size(CameraOffset))))
                {
                    Console.SetCursorPosition(e.Pos.X - CameraOffset.X, e.Pos.Y - CameraOffset.Y);
                    Console.Write(e.Ascii);
                }
            }
        }
    }
}