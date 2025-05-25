namespace BallGame.Input
{
    public class ConsoleInputManager : IInputManager
    {
        public ConsoleKey ReadKey(bool intercept = true)
        {
            return Console.ReadKey(intercept).Key;
        }

        public bool KeyAvailable => Console.KeyAvailable;
    }
}