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
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Backspace:
                        menus.ResetMenu();
                        break;
                    case ConsoleKey.UpArrow:
                        CursorPosition.Y = Math.Max(0, CursorPosition.Y - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        CursorPosition.Y = Math.Min(GameManager.ActiveMap.Size.Y ,CursorPosition.Y + 1); // TODO use map Height
                        break;
                    case ConsoleKey.LeftArrow:
                        CursorPosition.X = Math.Max(0, CursorPosition.X - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        CursorPosition.X = Math.Min(GameManager.ActiveMap.Size.X, CursorPosition.X + 1); // TODO use map Width 
                        break;
                    case ConsoleKey.Enter:
                        if (menus.State > 0)
                            menus.SetPointAction(new Point(CursorPosition.X, CursorPosition.Y));
                        break;
                    default:
                        menus.HandleMenuCommand(keyInfo.KeyChar);
                        break;
                }
            }
        }
    }
}