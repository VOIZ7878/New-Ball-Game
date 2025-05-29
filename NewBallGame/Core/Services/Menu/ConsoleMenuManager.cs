using BallGame.Rendering;
using BallGame.Input;

namespace BallGame
{
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
            await Task.Yield();
            var menuItems = new[]
            {
                ("New Game", MenuChoice.StartGame),
                ("Continue", MenuChoice.LoadGame),
                ("Show Results", MenuChoice.ShowResults),
                ("Settings", MenuChoice.Settings),
                ("Exit", MenuChoice.Exit),
                ("Test Level", MenuChoice.TestLevel)
            };
            int selected = 0;

            while (true)
            {
                renderer.Clear();
                renderer.WriteLine("=== BALL GAME ===");
                for (int i = 0; i < menuItems.Length; i++)
                {
                    string prefix = i == selected ? "> " : "  ";
                    renderer.WriteLine($"{prefix}{i + 1}. {menuItems[i].Item1}");
                }
                renderer.WriteLine("In-game actions: R - Restart; H - Hint; ESC - Exit");

                var key = inputManager.ReadKey(true);
                if (key == ConsoleKey.UpArrow)
                {
                    selected = (selected - 1 + menuItems.Length) % menuItems.Length;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selected = (selected + 1) % menuItems.Length;
                }
                else if (key == ConsoleKey.Enter)
                {
                    return menuItems[selected].Item2;
                }
            }
        }
    }
}