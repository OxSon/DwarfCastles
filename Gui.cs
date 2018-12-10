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

        private char[,] PastVisibleChars;
        private ConsoleColor[,] PastVisibleCharsColorsForeground;
        private ConsoleColor[,] PastVisibleCharsColorsBackground;
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

            VisibleChars = new char[Console.WindowWidth, Console.WindowHeight];
            VisibleCharsColorsForeground = new ConsoleColor[Console.WindowWidth, Console.WindowHeight];
            VisibleCharsColorsBackground = new ConsoleColor[Console.WindowWidth, Console.WindowHeight];
            VisibleCharOwnershipSet = new bool[Console.WindowWidth, Console.WindowHeight];
            Console.CursorVisible = false;
        }

        private void SetUpNewDraw()
        {
            PastVisibleChars = VisibleChars;
            PastVisibleCharsColorsBackground = VisibleCharsColorsBackground;
            PastVisibleCharsColorsForeground = VisibleCharsColorsForeground;
            VisibleChars = new char[Console.WindowWidth, Console.WindowHeight];
            VisibleCharsColorsForeground = new ConsoleColor[Console.WindowWidth, Console.WindowHeight];
            VisibleCharsColorsBackground = new ConsoleColor[Console.WindowWidth, Console.WindowHeight];
            VisibleCharOwnershipSet = new bool[Console.WindowWidth, Console.WindowHeight];
            for (int i = 0; i < VisibleCharsColorsBackground.GetUpperBound(0); i++)
            {
                for (int j = 0; j < VisibleCharsColorsBackground.GetUpperBound(1); j++)
                {
                    VisibleChars[i, j] = ' ';
                    VisibleCharsColorsBackground[i, j] = ConsoleColor.Black;
                    VisibleCharsColorsForeground[i, j] = ConsoleColor.Black;
                }
            }
        }

        private void FinishDraw()
        {
            for (int i = 0; i < VisibleChars.GetLength(0); i++)
            {
                for (int j = 0; j < VisibleChars.GetLength(1); j++)
                {
                    // Continue if the character is the same as last draw
                    if (PastVisibleChars[i, j] == VisibleChars[i, j] &&
                        PastVisibleCharsColorsBackground[i, j] == VisibleCharsColorsBackground[i, j] &&
                        PastVisibleCharsColorsForeground[i, j] == VisibleCharsColorsForeground[i, j])
                    {
                        continue;
                    }

                    Console.SetCursorPosition(i, j);
                    Console.BackgroundColor = VisibleCharsColorsBackground[i, j];
                    Console.ForegroundColor = VisibleCharsColorsForeground[i, j];
                    Console.Write(VisibleChars[i, j]);
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

        private void DrawFrame()
        {
            // Outter Frame
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                PrepareDraw('_', i, 0, ConsoleColor.Black, ConsoleColor.White, true);
                PrepareDraw('\u00AF', i, Console.WindowHeight - 1, ConsoleColor.Black, ConsoleColor.White);
            }

            for (int i = 0; i < Console.WindowHeight; i++)
            {
                PrepareDraw('|', 0, i, ConsoleColor.Black, ConsoleColor.White, true);
                PrepareDraw('|', Console.WindowWidth - 1, i, ConsoleColor.Black, ConsoleColor.White, true);
            }
            // Right and Bottom of the Map Window
            for (int i = 0; i < CameraSize.Y; i++)
            {
                PrepareDraw('|', CameraSize.X * 2 + 1, i + 1, ConsoleColor.Black, ConsoleColor.White, true);
            }
            for (int i = 0; i < CameraSize.X * 2; i++)
            {
                PrepareDraw('\u00AF', i + 1, CameraSize.Y + 1, ConsoleColor.Black, ConsoleColor.White, true);
            }
        }

        private void DrawInput()
        {
            if (GameManager.Menu.State == 1)
            {
                VisibleCharsColorsBackground[GameManager.Input.CursorPosition.X * 2 + 1, GameManager.Input.CursorPosition.Y + 1] = ConsoleColor.Magenta;
            }
            else if (GameManager.Menu.State == 2)
            {
                var r = MenuManager.FixedRectangle(GameManager.Menu.FirstPoint, GameManager.Input.CursorPosition);
                for (int i = r.X * 2 + 1; i <= r.Right * 2 + 1; i++)
                {
                    VisibleCharsColorsBackground[i, r.Y + 1] = ConsoleColor.Magenta;
                    VisibleCharsColorsBackground[i, r.Bottom + 1] = ConsoleColor.Magenta;
                }

                for (int i = r.Y; i <= r.Bottom; i++)
                {
                    VisibleCharsColorsBackground[r.X * 2 + 1, i + 1] = ConsoleColor.Magenta;
                    VisibleCharsColorsBackground[r.Right * 2 + 1, i + 1] = ConsoleColor.Magenta;
                }
            }
        }

        private void DrawMenu()
        {
            int FreeSpace = Console.WindowWidth - CameraSize.X * 2 - 3;
            int Start = CameraSize.X * 2 + 2;

            IList<string> correctedLines = new List<string>();

            foreach (var s in GameManager.Menu.GetMenuDisplay().Split('\n'))
            {
                foreach (var splitString in Split(s, FreeSpace))
                {
                    correctedLines.Add(splitString);
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            var line = 1;
            foreach (var s in correctedLines)
            {
                Console.SetCursorPosition(Start, line);
                Console.Write(s);
                line++;
            }
        }

        public void Draw()
        {
            SetUpNewDraw();
            DrawFrame();
            for (int i = 0; i < CameraSize.X; i++)
            {
                for (int j = 0; j < CameraSize.Y; j++)
                {
                    PrepareDraw('.', i * 2 + 1, j + 1, ConsoleColor.Black, ConsoleColor.White);
                }
            }

            IList<Entity> snapshot = new List<Entity>();
            // Use a snapshot to ensure the List is not changed during draw by another thread
            foreach (var e in GameManager.ActiveMap.Entities)
            {
                snapshot.Add(e);
            }

            foreach (var e in snapshot)
            {
                if (Map.Within(e.Pos, new Rectangle(CameraOffset.X, CameraOffset.Y, CameraSize.X, CameraSize.Y)))
                {
                    Point DrawPosition = new Point((e.Pos.X - CameraOffset.X) * 2 + 1, e.Pos.Y - CameraOffset.Y + 1);
                    PrepareDraw(e.Ascii, DrawPosition.X, DrawPosition.Y, e.BackgroundColor, e.ForegroundColor,
                        e is Actor);
                }
            }
            DrawInput();
            DrawMenu();
            FinishDraw();

        }

        private static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize + 1)
                .Select(i => str.Substring(i * chunkSize, Math.Min(str.Length, chunkSize)));
        }
    }
}