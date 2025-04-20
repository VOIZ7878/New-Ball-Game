namespace BallGame
{
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
                    if (nx >= 0 && nx < gameField.Width && ny >= 0 && ny < gameField.Height && !visited[nx, ny] &&
                        (gameField[nx, ny] == null || gameField[nx, ny] is EnergyBall))
                        queue.Enqueue((nx, ny));
                }
            }

            return false;
        }
    }
}