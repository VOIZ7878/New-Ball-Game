using BallGame.Rendering;

namespace BallGame
{
    class Wall : GameElement
    {
        public Wall() { }

        public override void Render(IRenderer renderer, int x, int y)
        {
            renderer.RenderAt(x, y, "#");
        }
    }
}