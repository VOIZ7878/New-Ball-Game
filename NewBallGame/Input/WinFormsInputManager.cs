using System.Windows.Forms;
using BallGame.Input;

namespace BallGame.Input
{
    public class WinFormsInputManager : IInputManager
    {
        public event KeyPressedHandler? KeyPressed;

        private ConsoleKey? lastKey = null;

        public void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (KeyMap.KeyToConsoleKey.TryGetValue(e.KeyCode, out var consoleKey))
            {
                lastKey = consoleKey;
                KeyPressed?.Invoke(consoleKey);
            }
            else
            {
                lastKey = null;
            }
        }

        public bool KeyAvailable => lastKey.HasValue;

        public ConsoleKey ReadKey(bool intercept = true)
        {
            if (lastKey.HasValue)
            {
                var key = lastKey.Value;
                lastKey = null;
                return key;
            }
            return ConsoleKey.NoName;
        }
    }
}