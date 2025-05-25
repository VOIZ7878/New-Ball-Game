using System;
using System.Threading.Tasks;
using BallGame.Rendering;
using BallGame.Input;

namespace BallGame
{
    public enum MenuChoice
    {
        StartGame,
        LoadGame,
        ShowResults,
        Settings,
        Exit,
        TestLevel
    }

    public class ConsoleMenuManager : IMenuManager
    {
        private readonly IRenderer renderer;
        private readonly IInputManager inputManager;

        public ConsoleMenuManager(IRenderer renderer, IInputManager inputManager)
        {
            this.renderer = renderer;
            this.inputManager = inputManager;
        }

        public async Task<MenuChoice> ShowMainMenuAsync()
        {
            while (true)
            {
                renderer.Clear();
                renderer.WriteLine("=== BALL GAME ===");
                renderer.WriteLine("1. New Game");
                renderer.WriteLine("2. Continue");
                renderer.WriteLine("3. Show Results");
                renderer.WriteLine("4. Settings");
                renderer.WriteLine("5. Exit");
                renderer.WriteLine("6. Test Level");
                renderer.WriteLine("\nSelect option [1-6]:");
                renderer.WriteLine("\nIn-game actions: R - Restart; H - Hint; V - View Results; F5 - Save Game; ESC - Exit");

                var key = inputManager.ReadKey(true);

                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        return MenuChoice.StartGame;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        return MenuChoice.LoadGame;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        return MenuChoice.ShowResults;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        return MenuChoice.Settings;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                    case ConsoleKey.Escape:
                        return MenuChoice.Exit;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        return MenuChoice.TestLevel;

                    default:
                        renderer.WriteLine("Invalid choice. Try again...");
                        await Task.Delay(1000);
                        break;
                }
            }
        }
    }
}