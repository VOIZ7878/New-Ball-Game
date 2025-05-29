using System.Collections.Generic;
using System.Windows.Forms;

namespace BallGame.Input
{
    public static class KeyMap
    {
        public enum InGameAction
        {
            None,
            ExitToMenu,
            Restart,
            ShowResults,
            Hint,
            Save
        }

        public static readonly Dictionary<ConsoleKey, InGameAction> ConsoleKeyToInGameAction = new()
        {
            { ConsoleKey.Escape, InGameAction.ExitToMenu },
            { ConsoleKey.R, InGameAction.Restart },
            { ConsoleKey.V, InGameAction.ShowResults },
            { ConsoleKey.H, InGameAction.Hint },
        };
        
        public static readonly Dictionary<Keys, ConsoleKey> KeyToConsoleKey = new()
        {
            { Keys.W, ConsoleKey.W },
            { Keys.A, ConsoleKey.A },
            { Keys.S, ConsoleKey.S },
            { Keys.D, ConsoleKey.D },
            { Keys.Up, ConsoleKey.UpArrow },
            { Keys.Down, ConsoleKey.DownArrow },
            { Keys.Left, ConsoleKey.LeftArrow },
            { Keys.Right, ConsoleKey.RightArrow },
            { Keys.Space, ConsoleKey.Spacebar },
            { Keys.P, ConsoleKey.Enter },
            { Keys.R, ConsoleKey.R },
            { Keys.Escape, ConsoleKey.Escape },
            { Keys.H, ConsoleKey.H }
        };

        public static readonly Dictionary<ConsoleKey, (int dx, int dy)> MovementKeys = new()
        {
            { ConsoleKey.W, (0, -1) },
            { ConsoleKey.S, (0, 1) },
            { ConsoleKey.A, (-1, 0) },
            { ConsoleKey.D, (1, 0) },
            { ConsoleKey.UpArrow, (0, -1) },
            { ConsoleKey.DownArrow, (0, 1) },
            { ConsoleKey.LeftArrow, (-1, 0) },
            { ConsoleKey.RightArrow, (1, 0) }
        };

        public static readonly Dictionary<ConsoleKey, char> ShieldKeys = new()
        {
            { ConsoleKey.Spacebar, '/' },
            { ConsoleKey.Enter, '\\' }
        };
    }
}