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

        public static bool PlaceShield(GameElement?[,] grid, int x, int y, char direction)
        {
            if (grid[x, y] == null)
            {
                grid[x, y] = new Shield(direction);
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