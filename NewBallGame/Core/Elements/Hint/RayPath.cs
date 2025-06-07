namespace BallGame
{
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
