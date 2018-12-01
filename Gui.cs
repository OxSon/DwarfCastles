using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles
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

        

        public void DrawMenu(Dictionary<string, string> Options)
        {
            
        }

        public void Draw(Map map)
        {
            char[,] visibleChars = new char[CameraSize.X, CameraSize.Y];
            ConsoleColor[,] visibleCharsColorsForeground = new ConsoleColor[CameraSize.X, CameraSize.Y];
            ConsoleColor[,] visibleCharsColorsBackground = new ConsoleColor[CameraSize.X, CameraSize.Y];
            Logger.Log(visibleChars[0, 0] + " " + visibleCharsColorsBackground[0, 0] + " " +
                       visibleCharsColorsForeground[0, 0]);
            Console.CursorVisible = false;
            foreach (var e in map.Entities)
            {
                if (map.InBounds(Point.Add(e.Pos, new Size(CameraOffset))))
                {
                    Point RelativePoint = new Point(e.Pos.X + CameraOffset.X, e.Pos.Y + CameraOffset.Y);
                    visibleCharsColorsBackground[RelativePoint.X, RelativePoint.Y] = e.BackgroundColor;
                    visibleCharsColorsForeground[RelativePoint.X, RelativePoint.Y] = e.ForegroundColor;
                    visibleChars[RelativePoint.X, RelativePoint.Y] = e.Ascii;
                }
            }

            for (int i = 0; i < visibleChars.GetLength(0); i++)
            {
                for (int j = 0; j < visibleChars.GetLength(1); j++)
                {
                    Console.SetCursorPosition(i * 2, j - CameraOffset.Y);
                    if (visibleChars[i, j] != '\0')
                    {
                        Console.BackgroundColor = visibleCharsColorsBackground[i, j];
                        Console.ForegroundColor = visibleCharsColorsForeground[i, j];
                        Console.Write(visibleChars[i, j]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(map.InBounds(new Point(i, j)) ? '.' : ' ');
                    }
                }
            }
        }

        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }
}