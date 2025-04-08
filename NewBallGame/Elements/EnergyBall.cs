using BallGame.Rendering;

namespace BallGame
{
    class EnergyBall : GameElement
    {
        public EnergyBall() { }

        public override void Render(IRenderer renderer, int x, int y)
        {
            renderer.RenderAt(x, y, "@");
        }
    }
}