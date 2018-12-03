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
                    case ConsoleKey.Backspace:
                        menus.ResetMenu();
                        break;
                    case ConsoleKey.UpArrow:
                        CursorPosition.Y = Math.Max(0, CursorPosition.Y - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        CursorPosition.Y = Math.Min(Console.WindowHeight, CursorPosition.Y + 1); // TODO use map Height
                        break;
                    case ConsoleKey.LeftArrow:
                        CursorPosition.X = Math.Max(0, CursorPosition.X - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        CursorPosition.X = Math.Min(Console.WindowWidth, CursorPosition.X + 1); // TODO use map Width 
                        break;
                    case ConsoleKey.Enter:
                        if (menus.State > 0)
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