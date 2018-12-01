using System;
using System.Drawing;

namespace DwarfCastles
{
    public class InputManager
    {
        public Point CursorPosition;

        public InputManager()
        {
            CursorPosition = new Point();
        }
        
        public void HandleUserInput(MenuManager menus)
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        CursorPosition.Y--;
                        break;
                    case ConsoleKey.DownArrow:
                        CursorPosition.Y++;
                        break;
                    case ConsoleKey.LeftArrow:
                        CursorPosition.X = Math.Max(0, CursorPosition.X - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        CursorPosition.X = Math.Min(Console.WindowWidth, CursorPosition.X + 1); // TODO use map Width instead?
                        break;
                    case ConsoleKey.Enter:
                        if (menus.State > 1)
                        {
                            menus.SetPointAction(new Point(CursorPosition.X, CursorPosition.Y));
                        }
                        break;
                    default:
                        menus.HandleMenuCommand(keyInfo.KeyChar);
                        break;
                }
            }
        }
    }
}