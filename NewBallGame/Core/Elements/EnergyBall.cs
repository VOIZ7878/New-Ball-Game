using BallGame.Utils;

namespace BallGame
{
    public class EnergyBall : GameElement
    {
        public override bool IsOpenToMove() => true;

        public static void Collect(GameElement?[,] grid, int x, int y, ref int energyBallCount, Player player, ISoundManager soundManager)
        {
            grid[x, y] = null;
            energyBallCount--;
            player.AddScore(100);
            soundManager.PlaySoundEffect("collect.mp3");
        }

        public static bool IsEnergyBall(GameElement? element)
        {
            return element is EnergyBall;
        }
    }
}