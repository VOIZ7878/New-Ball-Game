using System.Windows.Forms;
using BallGame.Input;

namespace BallGame.Input
{
    public class WinFormsInputManager : IInputManager
    {
        private ConsoleKey? lastKey = null;

        public void OnKeyDown(object? sender, KeyEventArgs e)
        {
            lastKey = e.KeyCode switch
            {
                Keys.W => ConsoleKey.W,
                Keys.A => ConsoleKey.A,
                Keys.S => ConsoleKey.S,
                Keys.D => ConsoleKey.D,
                Keys.Up => ConsoleKey.UpArrow,
                Keys.Down => ConsoleKey.DownArrow,
                Keys.Left => ConsoleKey.LeftArrow,
                Keys.Right => ConsoleKey.RightArrow,
                Keys.Space => ConsoleKey.Spacebar,
                Keys.P => ConsoleKey.Enter,
                Keys.R => ConsoleKey.R,
                Keys.Escape => ConsoleKey.Escape,
                Keys.V => ConsoleKey.V,
                Keys.H => ConsoleKey.H,
                _ => null
            };
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