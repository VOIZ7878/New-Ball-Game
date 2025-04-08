using BallGame.Rendering;

namespace BallGame
{
    class Shield : GameElement
    {
        public char Direction { get; }

        public Shield(char direction)
        {
            Direction = direction;
        }

        public override void Render(IRenderer renderer, int x, int y)
        {
            renderer.RenderAt(x, y, Direction.ToString());
        }
    }
}