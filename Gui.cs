using System;
using System.Drawing;

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

        public void Draw(Map map)
        {
            char[,] VisibleChars = new char[CameraSize.X, CameraSize.Y];
            ConsoleColor[,] VisibleCharsColorsForeground = new ConsoleColor[CameraSize.X, CameraSize.Y];
            ConsoleColor[,] VisibleCharsColorsBackground = new ConsoleColor[CameraSize.X, CameraSize.Y];
            Logger.Log(VisibleChars[0, 0] + " " + VisibleCharsColorsBackground[0, 0] + " " +
                       VisibleCharsColorsForeground[0, 0]);
            Console.CursorVisible = false;
            foreach (var e in map.Entities)
            {
                if (map.InBounds(Point.Add(e.Pos, new Size(CameraOffset))))
                {
                    Point RelativePoint = new Point(e.Pos.X + CameraOffset.X, e.Pos.Y + CameraOffset.Y);
                    VisibleCharsColorsBackground[RelativePoint.X, RelativePoint.Y] = e.BackgroundColor;
                    VisibleCharsColorsForeground[RelativePoint.X, RelativePoint.Y] = e.ForegroundColor;
                    VisibleChars[RelativePoint.X, RelativePoint.Y] = e.Ascii;
                }
            }

            for (int i = 0; i < VisibleChars.GetLength(0); i++)
            {
                for (int j = 0; j < VisibleChars.GetLength(1); j++)
                {
                    Console.SetCursorPosition(i * 2, j - CameraOffset.Y);
                    if (VisibleChars[i, j] != '\0')
                    {
                        Console.BackgroundColor = VisibleCharsColorsBackground[i, j];
                        Console.ForegroundColor = VisibleCharsColorsForeground[i, j];
                        Console.Write(VisibleChars[i, j]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        if (map.InBounds(new Point(i, j)))
                        {
                            Console.Write('.');
                        }
                        else
                        {
                            Console.Write(' ');
                        }
                    }
                }
            }
        }
    }
}