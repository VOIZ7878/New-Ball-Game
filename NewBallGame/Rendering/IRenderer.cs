namespace BallGame.Rendering
{
    public interface IRenderer
    {
        void Render(GameField field);
        void RenderAt(int x, int y, string content);
        void Clear();
        void RenderFrame();

        void WriteLine(string message);
        void Pause(int milliseconds = 2000);
        void WaitForKeyPress(string prompt = "\nPress any key to continue...");
    }
}