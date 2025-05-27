namespace BallGame.Input
{
    public class ConsoleInputManager : IInputManager
    {
        public event KeyPressedHandler? KeyPressed;

        public ConsoleKey ReadKey(bool intercept = true)
        {
            var key = Console.ReadKey(intercept).Key;
            KeyPressed?.Invoke(key);
            return key;
        }

        public bool KeyAvailable => Console.KeyAvailable;
    }
}