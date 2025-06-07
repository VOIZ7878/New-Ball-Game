namespace BallGame
{
    public class Hint : GameElement
    {
        private (int x, int y)? hintPosition;
        private char? hintDirection;
        private List<(int x, int y)> pathPoints = new();
        public (int x, int y)? HintPosition => hintPosition;
        public char? HintDirection => hintDirection;
        public List<(int x, int y)> RayPathPoints => pathPoints;
        public override bool IsOpenToMove() => true;

        public void CalculateHint(GameField gameField)
        {
            ClearHint();
            if (gameField.Ball == null || gameField.EnergyBallCount == 0)
                return;

            var forward = (dx: gameField.Ball.Dx, dy: gameField.Ball.Dy);
            var backward = (dx: -gameField.Ball.Dx, dy: -gameField.Ball.Dy);
            var forwardPath = TraceRayToEnergy(gameField, 5, 50, forward.dx, forward.dy);
            var backwardPath = TraceRayToEnergy(gameField, 5, 50, backward.dx, backward.dy);
            var bestPath = ChooseBestPath(forwardPath, backwardPath);

            if (bestPath != null)
            {
                pathPoints = FilterPathForDisplay(bestPath.Points, gameField);
                SetHintForShield(bestPath, gameField);
            }
        }

        private RayPath? TraceRayToEnergy(GameField gameField, int maxShields, int maxSteps, int dirDx, int dirDy)
        {
            if (gameField.Ball == null)
                return null;
            var start = (x: gameField.Ball.X, y: gameField.Ball.Y);
            var dir = (dx: dirDx, dy: dirDy);

            if (gameField[start.x, start.y] is EnergyBall)
                return new RayPath(start.x, start.y, dir.dx, dir.dy, new List<(int, int)> { start });

            var visited = new Dictionary<(int x, int y, int dx, int dy), (int shields, int length)>();
            var pq = new PriorityQueue<RayPath, (int shields, int length)>();
            pq.Enqueue(new RayPath(start.x, start.y, dir.dx, dir.dy, new List<(int, int)> { start }), (0, 0));

            RayPath? bestPath = null;
            int minShields = int.MaxValue;
            int minLength = int.MaxValue;

            while (pq.Count > 0)
            {
                var path = pq.Dequeue();
                int shieldCount = path.Shields.Count;
                int x = path.X;
                int y = path.Y;
                int dx = path.Dx;
                int dy = path.Dy;
                var currentPoints = new List<(int, int)>(path.Points);

                for (int step = 0; step < maxSteps; step++)
                {
                    x += dx;
                    y += dy;

                    if (!gameField.IsInside(x, y))
                        break;

                    currentPoints.Add((x, y));

                    var cell = gameField[x, y];
                    if (cell is EnergyBall)
                    {
                        int pathLength = currentPoints.Count;
                        if (shieldCount < minShields || (shieldCount == minShields && pathLength < minLength))
                        {
                            minShields = shieldCount;
                            minLength = pathLength;
                            bestPath = path.WithNewPosition(x, y, currentPoints);
                        }
                        break;
                    }

                    if (cell is Wall)
                        break;

                    if (cell is Shield shield)
                    {
                        int ndx = dx, ndy = dy;
                        GameField.Reflect(shield.Mirror, ref ndx, ref ndy);
                        dx = ndx;
                        dy = ndy;
                        continue;
                    }

                    var state = (x, y, dx, dy);
                    int pathLen = currentPoints.Count;
                    if (visited.TryGetValue(state, out var prev) && (prev.shields < shieldCount || (prev.shields == shieldCount && prev.length <= pathLen)))
                        continue;
                    visited[state] = (shieldCount, pathLen);

                    if (shieldCount < maxShields)
                    {
                        foreach (char mirror in new[] { '/', '\\' })
                        {
                            if (IsValidShieldPosition(gameField, x, y))
                            {
                                int ndx = dx, ndy = dy;
                                GameField.Reflect(mirror, ref ndx, ref ndy);
                                var newState = (x, y, ndx, ndy);
                                int newShieldCount = shieldCount + 1;
                                int newPathLen = pathLen;
                                if (!visited.TryGetValue(newState, out var prev2) || (newShieldCount < prev2.shields || (newShieldCount == prev2.shields && newPathLen < prev2.length)))
                                {
                                    var newPath = path.WithVirtualShield(x, y, mirror, ndx, ndy, currentPoints);
                                    pq.Enqueue(newPath, (newShieldCount, newPathLen));
                                }
                            }
                        }
                    }
                }
            }
            return bestPath;
        }

        private bool IsValidShieldPosition(GameField gameField, int x, int y)
        {
            return x >= 0 && x < gameField.Width &&
                   y >= 0 && y < gameField.Height &&
                   gameField[x, y] == null;
        }

        private RayPath? ChooseBestPath(RayPath? path1, RayPath? path2)
        {
            if (path1 == null) return path2;
            if (path2 == null) return path1;
            int s1 = path1.Shields.Count, s2 = path2.Shields.Count;
            int l1 = path1.Points.Count, l2 = path2.Points.Count;
            if (s1 < s2 || (s1 == s2 && l1 <= l2)) return path1;
            return path2;
        }

        private List<(int x, int y)> FilterPathForDisplay(List<(int x, int y)> points, GameField gameField)
        {
            return points.Where(p => (gameField[p.x, p.y] == null || gameField[p.x, p.y] is Ball) && !(gameField[p.x, p.y] is EnergyBall)).ToList();
        }

        private void SetHintForShield(RayPath path, GameField gameField)
        {
            if (!path.Shields.Any()) return;
            var (sx, sy, sdir) = path.Shields.First();
            if (gameField.Ball != null && sx == gameField.Ball.X && sy == gameField.Ball.Y)
            {
                char chosenMirror = (sdir == '/' ? '/' : '\\');
                hintPosition = (sx, sy);
                hintDirection = chosenMirror;
            }
            else
            {
                hintPosition = (sx, sy);
                hintDirection = sdir;
            }
        }

        public void ClearHint()
        {
            hintPosition = null;
            hintDirection = null;
            pathPoints.Clear();
        }

    }
}