using BallGame.Utils;

namespace BallGame
{
    public class Shield : GameElement
    {
        public char Direction { get; }
        public char Mirror => Direction;

        public Shield(char direction)
        {
            Direction = direction;
        }

        public override bool IsOpenToMove() => false;

        public static bool PlaceShield(GameElement?[,] grid, int x, int y, char direction, ISoundManager soundManager)
        {
            if (grid[x, y] == null)
            {
                grid[x, y] = new Shield(direction);
                soundManager.PlaySoundEffect("shield.mp3");
                return true;
            }
            return false;
        }

        public static bool IsShield(GameElement? element, out char dir)
        {
            if (element is Shield shield)
            {
                dir = shield.Direction;
                return true;
            }
            dir = ' ';
            return false;
        }
    }
}