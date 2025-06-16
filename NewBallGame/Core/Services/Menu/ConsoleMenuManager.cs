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

        public event Action<MenuChoice>? MenuChoiceSelected;

        public async Task<MenuChoice> ShowMainMenuAsync(string lastScoreDisplay)
        {
            await Task.Yield();
            var menuItems = new[]
            {
                ("New Game", MenuChoice.StartGame),
                ("Continue", MenuChoice.LoadGame),
                ("Show Results", MenuChoice.ShowResults),
                ("Settings", MenuChoice.Settings),
                ("Exit", MenuChoice.Exit),
                ("Manual Level", MenuChoice.ManualLevel)
            };
            int selected = 0;

            while (true)
            {
                renderer.Clear();
                renderer.WriteLine($"=== BALL GAME ===");
                for (int i = 0; i < menuItems.Length; i++)
                {
                    string prefix = i == selected ? "> " : "  ";
                    renderer.WriteLine($"{prefix}{i + 1}. {menuItems[i].Item1}");
                }
                renderer.WriteLine("In-game actions: R - Restart; H - Hint; ESC - Exit");
                renderer.WriteLine("\nLast Score:");
                renderer.WriteLine(lastScoreDisplay);

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
                    var choice = menuItems[selected].Item2;
                    MenuChoiceSelected?.Invoke(choice);
                    return choice;
                }
            }
        }

        public async Task ShowSettingsMenuAsync(GenerationSettings settings)
        {
            while (true)
            {
                renderer.Clear();
                renderer.WriteLine("=== SETTINGS ===");
                renderer.WriteLine($"1. EnergyBall count range: {settings.EnergyBallRange.Min}-{settings.EnergyBallRange.Max}");
                renderer.WriteLine($"2. Wall count range: {settings.WallRange.Min}-{settings.WallRange.Max}");
                renderer.WriteLine($"3. Enemy count range: {settings.EnemyRange.Min}-{settings.EnemyRange.Max}");
                renderer.WriteLine($"4. Field width: {settings.FieldWidth}");
                renderer.WriteLine($"5. Field height: {settings.FieldHeight}");
                renderer.WriteLine("Enter number to change (1-5), or ESC to exit:");
                var key = inputManager.ReadKey(true);
                if (key == ConsoleKey.Escape) break;
                int field = -1;
                if (key >= ConsoleKey.D1 && key <= ConsoleKey.D5)
                    field = (int)key - (int)ConsoleKey.D1 + 1;
                else if (key >= ConsoleKey.NumPad1 && key <= ConsoleKey.NumPad5)
                    field = (int)key - (int)ConsoleKey.NumPad1 + 1;
                if (field >= 1 && field <= 3)
                {
                    renderer.WriteLine($"Enter min value (0-99):");
                    int min = ReadInt();
                    renderer.WriteLine($"Enter max value (0-99):");
                    int max = ReadInt();
                    if (min < 0 || max < 0 || min > 99 || max > 99)
                    {
                        renderer.WriteLine("Values must be between 0 and 99. Press any key to continue...");
                        inputManager.ReadKey(true);
                        continue;
                    }
                    if (min > max)
                    {
                        renderer.WriteLine("Min cannot be greater than Max. Press any key to continue...");
                        inputManager.ReadKey(true);
                        continue;
                    }
                    switch (field)
                    {
                        case 1: settings.EnergyBallRange = new Range(min, max); break;
                        case 2: settings.WallRange = new Range(min, max); break;
                        case 3: settings.EnemyRange = new Range(min, max); break;
                        case 4: settings.FieldWidth = new Range(min, max); break;
                        case 5: settings.FieldHeight = new Range(min, max); break;
                    }
                }
                renderer.Clear();
                renderer.WriteLine("Settings updated successfully!");
                await Task.Delay(700);
            }
        }

        public async Task<string?> ShowLevelSelectMenuAsync()
        {
            await Task.Yield();
            var levelDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Levels");
            var files = Directory.GetFiles(levelDir, "*.txt").Select(Path.GetFileName).ToArray();
            if (files.Length == 0)
            {
                renderer.WriteLine("No levels. You can add levels to the 'assets/Levels' directory.");
                inputManager.ReadKey(true);
                return null;
            }
            int selected = 0;
            while (true)
            {
                renderer.Clear();
                renderer.WriteLine("=== SELECT LEVEL ===");
                for (int i = 0; i < files.Length; i++)
                {
                    string prefix = i == selected ? "> " : "  ";
                    renderer.WriteLine($"{prefix}{files[i]}");
                }
                renderer.WriteLine("ESC - back");
                var key = inputManager.ReadKey(true);
                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + files.Length) % files.Length;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % files.Length;
                else if (key == ConsoleKey.Enter)
                {
                    var fileName = files.ElementAtOrDefault(selected);
                    if (!string.IsNullOrEmpty(fileName))
                        return fileName;
                    else
                    {
                        renderer.WriteLine("Error: level not found.");
                        await Task.Delay(1000);
                        return null;
                    }
                }
                else if (key == ConsoleKey.Escape)
                    return null;
            }
        }

        private int ReadInt()
        {
            string input = "";
            while (true)
            {
                var key = inputManager.ReadKey(true);
                if (key == ConsoleKey.Enter && int.TryParse(input, out int result))
                {
                    renderer.WriteLine("");
                    return result;
                }
                if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
                {
                    input += (char)('0' + (key - ConsoleKey.D0));
                }
                else if (key >= ConsoleKey.NumPad0 && key <= ConsoleKey.NumPad9)
                {
                    input += (char)('0' + (key - ConsoleKey.NumPad0));
                }
                else if (key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                }
                renderer.WriteLine($"Current: {input}");
            }
        }
    }
}