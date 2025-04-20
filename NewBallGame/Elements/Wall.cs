using BallGame.Rendering;

namespace BallGame
{
    public class Wall : GameElement
    {
        public Wall() { }

        public override void Render(IRenderer renderer, int x, int y)
        {
            renderer.RenderAt(x, y, "#");
        }

        public static bool IsWall(GameElement? element)
        {
            return element is Wall;
        }
    }
}