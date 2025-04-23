using BallGame.Rendering;

namespace BallGame
{
    public class EnergyBall : GameElement
    {
        public override bool IsMoveable() => true;

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