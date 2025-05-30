using System.Runtime.InteropServices;
using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [STAThread]
        static async Task Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "winforms")
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new MainForm(new SoundManager()));
            }
            else
            {
                AllocConsole();
                var runner = new GameRunner(
                    new ConsoleRenderer(), 
                    new ConsoleInputManager(), 
                    new SoundManager(), 
                    new ConsoleMenuManager(new ConsoleRenderer(), new ConsoleInputManager())
                );
                await runner.Run();
            }
        }
    }
}