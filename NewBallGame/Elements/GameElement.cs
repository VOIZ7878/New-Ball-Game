using BallGame.Rendering;

namespace BallGame
{
    public abstract class GameElement
    {
        public abstract void Render(IRenderer renderer, int x, int y);
    }
}