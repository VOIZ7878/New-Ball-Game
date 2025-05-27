namespace BallGame.Input
{
    public delegate void KeyPressedHandler(ConsoleKey key);

    public interface IInputManager
    {
        event KeyPressedHandler? KeyPressed;
        ConsoleKey ReadKey(bool intercept = true);
        bool KeyAvailable { get; }
    }
}