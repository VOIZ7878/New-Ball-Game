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

        public static bool EnergyBallReachable(GameField field, Player player)
        {
            for (int x = 0; x < field.Width; x++)
            {
                for (int y = 0; y < field.Height; y++)
                {
                    if (field[x, y] is EnergyBall && !Pathfinding.PathExists(field, player.X, player.Y, x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void Collect(GameElement?[,] grid, int x, int y, ref int energyBallCount, Player player)
        {
            grid[x, y] = null;
            energyBallCount--;
            player.AddScore(100);
        }

        public static bool IsEnergyBall(GameElement? element)
        {
            return element is EnergyBall;
        }
    }
}