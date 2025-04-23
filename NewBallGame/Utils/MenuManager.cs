using System;
using BallGame.Rendering;

namespace BallGame
{
    public enum MenuChoice
    {
        StartGame,
        ShowResults,
        TestLevel,
        Exit
    }
    public class MenuManager
    {
        private readonly IRenderer renderer;

        public MenuManager(IRenderer renderer)
        {
            this.renderer = renderer;
        }

        public MenuChoice ShowMainMenu()
        {
            while (true)
            {
                renderer.Clear();
                renderer.WriteLine("=== BALL GAME ===");
                renderer.WriteLine("1. Start Game");
                renderer.WriteLine("2. Show Results");
                renderer.WriteLine("3. Test Level");
                renderer.WriteLine("4. Exit");
                renderer.WriteLine("\nSelect option [1-4]:");
                renderer.WriteLine("\nIn-game actions: R - Restart; H - Hint; V - View Results; ESC - Exit");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        return MenuChoice.StartGame;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        return MenuChoice.ShowResults;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        return MenuChoice.TestLevel;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.Escape:
                        return MenuChoice.Exit;

                    default:
                        renderer.WriteLine("Invalid choice. Try again...");
                        renderer.Pause(1000);
                        break;
                }
            }
        }
    }
}