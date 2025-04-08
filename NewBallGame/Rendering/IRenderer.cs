namespace BallGame.Rendering
{
    public interface IRenderer
    {
        void RenderAt(int x, int y, string content);
        void Clear();
        void RenderFrame();
    }
}