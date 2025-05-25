using BallGame.Rendering;

namespace BallGame
{
    public class Hint : GameElement
    {
        private (int x, int y)? hintPosition;
        private char? hintDirection;

        public (int x, int y)? GetHintPosition() => hintPosition;
        public char? GetHintDirection() => hintDirection;
        public override bool IsMoveable() => true;

        public void CalculateHint(GameField gameField)
        {
            if (gameField.Ball == null || gameField.EnergyBallCount == 0)
                return;

            var start = (x: gameField.Ball.X, y: gameField.Ball.Y);
            var dir = (dx: gameField.Ball.Dx, dy: gameField.Ball.Dy);

            var visited = new HashSet<(int x, int y, int dx, int dy)>();
            var queue = new Queue<(int x, int y, int dx, int dy, List<(int sx, int sy, char dir)> shields)>();
            queue.Enqueue((start.x, start.y, dir.dx, dir.dy, new List<(int, int, char)>()));

            while (queue.Count > 0)
            {
                var (x, y, dx, dy, shields) = queue.Dequeue();

                int cx = x, cy = y;
                for (int step = 0; step < 50; step++)
                {
                    cx += dx;
                    cy += dy;

                    if (!gameField.IsInside(cx, cy))
                        break;

                    var cell = gameField[cx, cy];

                    if (cell is EnergyBall)
                    {
                        if (shields.Count > 0)
                        {
                            var (sx, sy, sdir) = shields[0];
                            hintPosition = (sx, sy);
                            hintDirection = sdir;
                            return;
                        }
                        else
                        {
                            hintPosition = null;
                            hintDirection = null;
                            return;
                        }
                    }

                    if (cell is Wall || cell is Shield)
                        break;

                    if (shields.Count >= 5)
                        continue;

                    foreach (char mirror in new[] { '/', '\\' })
                    {
                        int nx = cx, ny = cy;
                        if (!IsValidShieldPosition(gameField, nx, ny))
                            continue;

                        int ndx = dx, ndy = dy;
                        Reflect(mirror, ref ndx, ref ndy);

                        if (!visited.Contains((nx, ny, ndx, ndy)))
                        {
                            var newShields = new List<(int, int, char)>(shields) { (nx, ny, mirror) };
                            queue.Enqueue((nx, ny, ndx, ndy, newShields));
                            visited.Add((nx, ny, ndx, ndy));
                        }
                    }
                }
            }

            hintPosition = null;
            hintDirection = null;
        }

        private void Reflect(char mirror, ref int dx, ref int dy)
        {
            if (mirror == '/')
            {
                int temp = dx;
                dx = -dy;
                dy = -temp;
            }
            else if (mirror == '\\')
            {
                int temp = dx;
                dx = dy;
                dy = temp;
            }
        }

        private bool IsValidShieldPosition(GameField gameField, int x, int y)
        {
            return x >= 0 && x < gameField.Width &&
                   y >= 0 && y < gameField.Height &&
                   gameField[x, y] == null;
        }

        public void ClearHint()
        {
            hintPosition = null;
            hintDirection = null;
        }
    }
}