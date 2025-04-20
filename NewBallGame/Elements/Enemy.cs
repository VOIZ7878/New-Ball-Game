using BallGame.Rendering;

namespace BallGame
{
    public class Enemy : GameElement
    {
        public int X, Y;

        public Enemy(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override void Render(IRenderer renderer, int x, int y)
        {
            renderer.RenderAt(x, y, "E");
        }

        public void Move(GameField field)
        {
            Random rnd = new Random();
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };
            int dir = rnd.Next(4);
            int newX = X + dx[dir];
            int newY = Y + dy[dir];

            if (!field.IsWall(newX, newY) && !field.IsEnemy(newX, newY))
            {
                X = newX;
                Y = newY;
            }
        }

        public static bool IsEnemy(List<Enemy> enemies, int x, int y)
        {
            return enemies.Any(enemy => enemy.X == x && enemy.Y == y);
        }

        public static void UpdateEnemies(List<Enemy> enemies, GameElement?[,] grid, GameField gameField)
        {
            foreach (var enemy in enemies)
            {
                grid[enemy.X, enemy.Y] = null;
            }

            enemies.ForEach(e =>
            {
                e.Move(gameField);
                grid[e.X, e.Y] = e;
            });
        }
    }
}