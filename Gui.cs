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
        
        public Point CameraSize { get; }

        public Gui()
        {
            CameraOffset = new Point();
            CameraSize = new Point(25, 25);
        }

        public void Draw(Map map)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < CameraSize.X; i++)
            {
                for (int j = 0; j < CameraSize.Y; j++)
                {
                    if (map.InBounds(Point.Add(new Point(i, j), new Size(CameraOffset))))
                    {
                        Console.SetCursorPosition((i - CameraOffset.X) * 2, j - CameraOffset.Y);
                        Console.Write('.');
                    }
                }
            }
            Console.CursorVisible = false;
            foreach (var e in map.Entities)
            {
                if (map.InBounds(Point.Add(e.Pos, new Size(CameraOffset))))
                {
                    Console.BackgroundColor = e.BackgroundColor;
                    Console.ForegroundColor = e.ForegroundColor;
                    Console.SetCursorPosition((e.Pos.X - CameraOffset.X) * 2, e.Pos.Y - CameraOffset.Y);
                    Console.Write(e.Ascii);
                }
            }
        }
    }
}