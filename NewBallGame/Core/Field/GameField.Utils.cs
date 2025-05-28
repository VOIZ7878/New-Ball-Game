using BallGame.Rendering;
using BallGame.Utils;
using BallGame.Input;

namespace BallGame
{
    public partial class GameField
    {
        public bool IsMoveable(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
            return grid[x, y]?.IsMoveable() ?? true;
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
    
    public static class Pathfinding
    {
        public static bool PathExists(GameField gameField, int startX, int startY, int targetX, int targetY)
        {
            bool[,] visited = new bool[gameField.Width, gameField.Height];
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((startX, startY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                if (x == targetX && y == targetY) return true;
                if (visited[x, y]) continue;
                visited[x, y] = true;

                foreach (var (dx, dy) in new[] { (0, -1), (0, 1), (-1, 0), (1, 0) })
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && nx < gameField.Width && ny >= 0 && ny < gameField.Height && !visited[nx, ny])
                    {
                        var cell = gameField[nx, ny];
                        if (cell == null || cell is EnergyBall || !(cell is Wall || cell is Shield))
                        {
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }

            return false;
        }

        public static (int, int)? FindNextStep(GameField field, int startX, int startY, int targetX, int targetY)
        {
            if (startX < targetX) return (startX + 1, startY);
            if (startX > targetX) return (startX - 1, startY);
            if (startY < targetY) return (startX, startY + 1);
            if (startY > targetY) return (startX, startY - 1);
            return null;
        }
    }
}
