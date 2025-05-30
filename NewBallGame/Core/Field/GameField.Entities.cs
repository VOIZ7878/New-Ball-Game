using BallGame.Utils;

namespace BallGame
{
    public partial class GameField
    {
        public Player Player { get; set; }
        public Ball? Ball { get; set; }
        public EnergyBall? EnergyBall { get; set; }
        public List<Enemy> Enemies { get; } = new();
        public List<(EnergyBall ball, int x, int y)> EnergyBallList { get; } = new();
        public Hint Hint => hint;
        private readonly Hint hint = new();
        public int TotalScore { get; set; }
        public DateTime StartTime { get; set; }
        private int energyBallCount;
        public int EnergyBallCount
        {
            get => energyBallCount;
            set => energyBallCount = value;
        }

        public bool IsEnemy(int x, int y) => Enemy.IsEnemy(Enemies, x, y);
        public bool IsShield(int x, int y, out char dir) => Shield.IsShield(grid[x, y], out dir);
        public bool IsEnergyBall(int x, int y) => EnergyBall.IsEnergyBall(grid[x, y]);
        public bool IsWall(int x, int y) => x < 0 || x >= Width || y < 0 || y >= Height || Wall.IsWall(grid[x, y]);

        public void CollectEnergyBall(int x, int y) => EnergyBall.Collect(grid, x, y, ref energyBallCount, Player, new SoundManager());
        public bool PlaceShield(int x, int y, char direction) => Shield.PlaceShield(grid, x, y, direction, new SoundManager());

    }
}