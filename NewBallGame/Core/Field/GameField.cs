using BallGame.Rendering;
using BallGame.Utils;
using BallGame.Input;

namespace BallGame
{
    public class GameField
    {
        private readonly int width, height;
        private readonly GameElement?[,] grid;
        private readonly IInputManager inputManager;
        private readonly ISoundManager soundManager;
        public Ball? Ball { get; set; }
        public EnergyBall? EnergyBall { get; set; }
        public Player Player { get; set; } = new Player(1, 1);
        public List<Enemy> Enemies { get; } = new List<Enemy>();
        public DateTime StartTime { get; set; }
        public int TotalScore { get; set; }
        private int energyBallCount;
        public List<(EnergyBall ball, int x, int y)> EnergyBallList { get; } = new();
        private readonly Hint hint = new();

        public int Width => width;
        public int Height => height;
        public Hint Hint => hint;

        public GameField(int w, int h, IRenderer renderer, IInputManager inputManager, ISoundManager soundManager, bool initialize = true)
        {
            width = w;
            height = h;
            grid = new GameElement?[w, h];
            this.inputManager = inputManager;
            this.soundManager = soundManager;

            Player = new Player(1, 1);

            if (initialize)
            {
                var initializer = new LevelBuilder(this);
                initializer.InitializeField();
            }

            StartTime = DateTime.Now;
        }

        public GameElement? this[int x, int y]
        {
            get => grid[x, y];
            set => grid[x, y] = value;
        }
        public GameElement?[,] Grid => grid;

        public int EnergyBallCount
        {
            get => energyBallCount;
            set => energyBallCount = value;
        }

        public bool IsWall(int x, int y) =>
            x < 0 || x >= width || y < 0 || y >= height || Wall.IsWall(grid[x, y]);
        public bool IsEnemy(int x, int y) => Enemy.IsEnemy(Enemies, x, y);

        public bool IsShield(int x, int y, out char dir) => Shield.IsShield(grid[x, y], out dir);

        public bool IsEnergyBall(int x, int y) => EnergyBall.IsEnergyBall(grid[x, y]);

        public void CollectEnergyBall(int x, int y) => EnergyBall.Collect(grid, x, y, ref energyBallCount, Player, soundManager);

        public bool PlaceShield(int x, int y, char direction) => Shield.PlaceShield(grid, x, y, direction, soundManager);

        public bool IsMoveable(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return false;
            return grid[x, y]?.IsMoveable() ?? true;
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public void Update(bool playerMoved)
        {
            if (Ball == null) return;
            Ball.Move(this);
        }
    }
}