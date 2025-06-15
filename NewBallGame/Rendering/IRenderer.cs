namespace BallGame.Rendering
{
    public interface IRenderer
    {
        void PreRender(GameField field);
        void PostRender(GameField field);
        void Clear();
        void WriteLine(string message);
        void Pause(int milliseconds = 2000);
    }
}