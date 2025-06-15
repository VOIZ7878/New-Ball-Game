namespace BallGame
{
    public class Hint : GameElement
    {
        private (int x, int y)? hintPosition;
        public (int x, int y)? HintPosition => hintPosition;
        private char? hintDirection;
        public char? HintDirection => hintDirection;
        private List<(int x, int y)> pathPoints = new();
        private List<(int x, int y, char mirror)> hintShields = new();
        public List<(int x, int y)> RayPathPoints => pathPoints;
        public override bool IsOpenToMove() => true;

        public void CalculateHint(GameField gameField)
        {
            ClearHint();
            if (gameField.Ball == null || gameField.EnergyBallCount == 0)
                return;

            var forwardDir = (dx: gameField.Ball.Dx, dy: gameField.Ball.Dy);
            var backwardDir = (dx: -gameField.Ball.Dx, dy: -gameField.Ball.Dy);
            var forwardRay = FindBestRayPath(gameField, 5, 50, forwardDir.dx, forwardDir.dy);
            var backwardRay = FindBestRayPath(gameField, 5, 50, backwardDir.dx, backwardDir.dy);
            var optimalRay = SelectOptimalRayPath(forwardRay, backwardRay);

            if (optimalRay != null)
            {
                pathPoints = FilterPathForDisplay(optimalRay.Points, gameField);
                SetHintForVirtualShields(optimalRay, 2);
            }
        }

        private RayPath? FindBestRayPath(GameField gameField, int maxVirtualShields, int maxRaySteps, int dirDx, int dirDy)
        {
            if (gameField.Ball == null)
                return null;
            var start = (x: gameField.Ball.X, y: gameField.Ball.Y);
            var dir = (dx: dirDx, dy: dirDy);

            if (gameField[start.x, start.y] is EnergyBall)
                return new RayPath(start.x, start.y, dir.dx, dir.dy, new List<(int, int)> { start });

            var visitedStates = new Dictionary<(int x, int y, int dx, int dy), (int shields, int length)>();
            var rayQueue = new PriorityQueue<RayPath, (int shields, int length)>();
            rayQueue.Enqueue(new RayPath(start.x, start.y, dir.dx, dir.dy, new List<(int, int)> { start }), (0, 0));

            RayPath? bestRay = null;
            int minShields = int.MaxValue;
            int minLength = int.MaxValue;

            while (rayQueue.Count > 0)
            {
                var ray = rayQueue.Dequeue();
                int shieldCount = ray.Shields.Count;
                int x = ray.X;
                int y = ray.Y;
                int dx = ray.Dx;
                int dy = ray.Dy;
                var rayPoints = new List<(int, int)>(ray.Points);

                for (int step = 0; step < maxRaySteps; step++)
                {
                    x += dx;
                    y += dy;

                    if (!gameField.IsInside(x, y))
                        break;

                    rayPoints.Add((x, y));

                    var cell = gameField[x, y];
                    if (cell is EnergyBall)
                    {
                        UpdateBestRayIfNeeded(ref bestRay, ref minShields, ref minLength, ray, x, y, rayPoints, shieldCount);
                        break;
                    }

                    if (cell is Wall)
                        break;

                    if (cell is Shield shield)
                    {
                        (dx, dy) = ReflectRayFromShield(shield, dx, dy);
                        continue;
                    }

                    if (!ShouldVisitState(visitedStates, x, y, dx, dy, shieldCount, rayPoints.Count))
                        continue;

                    if (shieldCount < maxVirtualShields)
                    {
                        TryAddVirtualShields(gameField, ray, x, y, dx, dy, shieldCount, rayPoints, rayQueue, visitedStates);
                    }
                }
            }
            return bestRay;
        }

        private static (int dx, int dy) ReflectRayFromShield(Shield shield, int dx, int dy)
        {
            int ndx = dx, ndy = dy;
            GameField.Reflect(shield.Mirror, ref ndx, ref ndy);
            return (ndx, ndy);
        }

        private static void UpdateBestRayIfNeeded(ref RayPath? bestRay, ref int minShields, ref int minLength, RayPath ray, int x, int y, List<(int, int)> rayPoints, int shieldCount)
        {
            int pathLength = rayPoints.Count;
            if (shieldCount < minShields || (shieldCount == minShields && pathLength < minLength))
            {
                minShields = shieldCount;
                minLength = pathLength;
                bestRay = ray.WithNewPosition(x, y, rayPoints);
            }
        }

        private static bool ShouldVisitState(Dictionary<(int x, int y, int dx, int dy), (int shields, int length)> visitedStates, int x, int y, int dx, int dy, int shieldCount, int pathLen)
        {
            var state = (x, y, dx, dy);
            if (visitedStates.TryGetValue(state, out var prev) && (prev.shields < shieldCount || (prev.shields == shieldCount && prev.length <= pathLen)))
                return false;
            visitedStates[state] = (shieldCount, pathLen);
            return true;
        }

        private void TryAddVirtualShields(GameField gameField, RayPath ray, int x, int y, int dx, int dy, int shieldCount, List<(int, int)> rayPoints, PriorityQueue<RayPath, (int shields, int length)> rayQueue, Dictionary<(int x, int y, int dx, int dy), (int shields, int length)> visitedStates)
        {
            char[] mirrors = new char[] { '/', '\\' };
            foreach (char mirror in mirrors)
            {
                if (IsValidShieldPosition(gameField, x, y))
                {
                    int ndx = dx, ndy = dy;
                    GameField.Reflect(mirror, ref ndx, ref ndy);
                    var newState = (x, y, ndx, ndy);
                    int newShieldCount = shieldCount + 1;
                    int newPathLen = rayPoints.Count;
                    if (!visitedStates.TryGetValue(newState, out var prev2) || (newShieldCount < prev2.shields || (newShieldCount == prev2.shields && newPathLen < prev2.length)))
                    {
                        var newRay = ray.WithVirtualShield(x, y, mirror, ndx, ndy, rayPoints);
                        rayQueue.Enqueue(newRay, (newShieldCount, newPathLen));
                    }
                }
            }
        }

        private bool IsValidShieldPosition(GameField gameField, int x, int y)
        {
            return x >= 0 && x < gameField.Width &&
                   y >= 0 && y < gameField.Height &&
                   gameField[x, y] == null;
        }

        private RayPath? SelectOptimalRayPath(RayPath? ray1, RayPath? ray2)
        {
            if (ray1 == null) return ray2;
            if (ray2 == null) return ray1;
            var score1 = (ray1.Shields.Count, ray1.Points.Count);
            var score2 = (ray2.Shields.Count, ray2.Points.Count);
            return score1.CompareTo(score2) <= 0 ? ray1 : ray2;
        }

        private List<(int x, int y)> FilterPathForDisplay(List<(int x, int y)> points, GameField gameField)
        {
            return points.Where(p => gameField[p.x, p.y] switch
            {
                null => true,
                Ball => true,
                Enemy => true,
                EnergyBall => false,
                _ => false
            }).ToList();
        }

        public void SetHintForVirtualShields(RayPath ray, int count)
        {
            hintShields.Clear();
            if (ray?.Shields == null || count <= 0) return;
            foreach (var shield in ray.Shields.Take(count))
                hintShields.Add(shield);
            if (hintShields.Count > 0)
            {
                var (sx, sy, sdir) = hintShields[0];
                hintPosition = (sx, sy);
                hintDirection = sdir;
            }
            else
            {
                hintPosition = null;
                hintDirection = null;
            }
        }

        public void ClearHint()
        {
            hintPosition = null;
            hintDirection = null;
            pathPoints.Clear();
            hintShields.Clear();
        }

    }
    
    public class RayPath
    {
        public int X { get; }
        public int Y { get; }
        public int Dx { get; }
        public int Dy { get; }
        public List<(int x, int y, char mirror)> Shields { get; }
        public List<(int x, int y)> Points { get; }

        public RayPath(int x, int y, int dx, int dy, List<(int, int)> points, List<(int, int, char)>? shields = null)
        {
            X = x;
            Y = y;
            Dx = dx;
            Dy = dy;
            Points = new List<(int, int)>(points);
            Shields = shields != null ? new List<(int, int, char)>(shields) : new();
        }

        public RayPath WithVirtualShield(int x, int y, char mirror, int newDx, int newDy, List<(int, int)> points)
        {
            var newShields = new List<(int, int, char)>(Shields) { (x, y, mirror) };
            return new RayPath(x, y, newDx, newDy, new List<(int, int)>(points), newShields);
        }

        public RayPath WithNewPosition(int x, int y, List<(int, int)> points)
        {
            return new RayPath(x, y, Dx, Dy, new List<(int, int)>(points), Shields);
        }
    }
}