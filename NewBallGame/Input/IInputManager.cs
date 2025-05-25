namespace BallGame.Input
{
    public interface IInputManager
    {
        ConsoleKey ReadKey(bool intercept = true);
        bool KeyAvailable { get; }
    }
}