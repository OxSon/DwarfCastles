using System;
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

        private char[,] VisibleChars;
        private ConsoleColor[,] VisibleCharsColorsForeground;
        private ConsoleColor[,] VisibleCharsColorsBackground;
        private bool[,] VisibleCharOwnershipSet;

        public Gui()
        {
            CameraOffset = new Point();
            CameraSize = new Point(25, 25);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            for (int i = 0; i < CameraSize.Y; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
            }

            Console.CursorVisible = false;
        }

        private void SetUpNewDraw()
        {
            VisibleChars = new char[CameraSize.X, CameraSize.Y];
            VisibleCharsColorsForeground = new ConsoleColor[CameraSize.X, CameraSize.Y];
            VisibleCharsColorsBackground = new ConsoleColor[CameraSize.X, CameraSize.Y];
            VisibleCharOwnershipSet = new bool[CameraSize.X, CameraSize.Y];
        }

        private void FinishDraw()
        {
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
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(GameManager.ActiveMap.InBounds(new Point(i, j)) ? '.' : ' ');
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(' ');
                    }
                }
            }
        }

        private void PrepareDraw(char c, int x, int y, ConsoleColor background, ConsoleColor foreground,
            bool DrawOnTop = false)
        {
            if (VisibleCharOwnershipSet[x, y])
            {
                return;
            }

            VisibleChars[x, y] = c;
            VisibleCharsColorsBackground[x, y] = background;
            VisibleCharsColorsForeground[x, y] = foreground;
            VisibleCharOwnershipSet[x, y] = DrawOnTop;
        }

        public void Draw(Map map, MenuManager menus, InputManager input)
        {
            SetUpNewDraw();

            IList<Entity> snapshot = new List<Entity>();
            // Use a snapshot to ensure the List is not changed during draw by another thread
            foreach (var e in map.Entities)
            {
                snapshot.Add(e);
            }

            foreach (var e in snapshot)
            {
                if (map.InBounds(Point.Add(e.Pos, new Size(CameraOffset))))
                {
                    var RelativePoint = new Point(e.Pos.X + CameraOffset.X, e.Pos.Y + CameraOffset.Y);
                    if (VisibleCharOwnershipSet[RelativePoint.X, RelativePoint.Y])
                    {
                        continue;
                    }

                    ConsoleColor c1;
                    ConsoleColor c2;
                    if (e.BackgroundColor == ConsoleColor.Black)
                    {
                        c1 = ConsoleColor.White;
                    }
                    else if (e.BackgroundColor == ConsoleColor.White)
                    {
                        c1 = ConsoleColor.Black;
                    }
                    else
                    {
                        c1 = e.BackgroundColor;
                    }

                    if (e.ForegroundColor == ConsoleColor.Black)
                    {
                        c2 = ConsoleColor.White;
                    }
                    else if (e.ForegroundColor == ConsoleColor.White)
                    {
                        c2 = ConsoleColor.Black;
                    }
                    else
                    {
                        c2 = e.ForegroundColor;
                    }

                    VisibleCharsColorsBackground[RelativePoint.X, RelativePoint.Y] = c1;
                    VisibleCharsColorsForeground[RelativePoint.X, RelativePoint.Y] = c2;
                    VisibleChars[RelativePoint.X, RelativePoint.Y] = e.Ascii;
                    if (e is Actor)
                    {
                        VisibleCharOwnershipSet[RelativePoint.X, RelativePoint.Y] = true;
                    }
                }
            }

            FinishDraw();

            if (menus.State == 1)
            {
                VisibleCharsColorsBackground[input.CursorPosition.X, input.CursorPosition.Y] = ConsoleColor.Magenta;
                VisibleChars[input.CursorPosition.X, input.CursorPosition.Y] = ' ';
            }
            else if (menus.State == 2)
            {
                var r = MenuManager.FixedRectangle(menus.FirstPoint, input.CursorPosition);
                for (int i = r.X; i <= r.Right; i++)
                {
                    VisibleCharsColorsBackground[i, r.Y] = ConsoleColor.Magenta;
                    VisibleChars[i, r.Y] = ' ';
                    VisibleCharsColorsBackground[i, r.Bottom] = ConsoleColor.Magenta;
                    VisibleChars[i, r.Bottom] = ' ';
                }

                for (int i = r.Y; i <= r.Bottom; i++)
                {
                    VisibleCharsColorsBackground[r.X, i] = ConsoleColor.Magenta;
                    VisibleChars[r.X, i] = ' ';
                    VisibleCharsColorsBackground[r.Right, i] = ConsoleColor.Magenta;
                    VisibleChars[r.Right, i] = ' ';
                }
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            for (int i = 0; i < CameraSize.Y; i++)
            {
                Console.SetCursorPosition(CameraSize.X * 2, i);
                Console.Write("|");
            }

            int FreeSpace = Console.WindowWidth - CameraSize.X * 2 - 3;
            int Start = CameraSize.X * 2 + 2;

            IList<string> correctedLines = new List<string>();

            foreach (var s in menus.GetMenuDisplay().Split('\n'))
            {
                foreach (var splitString in Split(s, FreeSpace))
                {
                    correctedLines.Add(splitString);
                }
            }

            // Clear the menu as to ensure there is no overlapped Lines
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < CameraSize.Y; i++)
            {
                Console.SetCursorPosition(Start, i);
                Console.Write(string.Concat(Enumerable.Repeat(" ", FreeSpace)));
            }

            Console.ForegroundColor = ConsoleColor.Black;
            var line = 0;
            foreach (var s in correctedLines)
            {
                Console.SetCursorPosition(Start, line);
                Console.Write(s);
                line++;
            }

            Console.BackgroundColor = ConsoleColor.Magenta;
        }

        private static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize + 1)
                .Select(i => str.Substring(i * chunkSize, Math.Min(str.Length, chunkSize)));
        }
    }
}