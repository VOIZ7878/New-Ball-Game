using BallGame.Rendering;

namespace BallGame
{
    public class GameField
    {
        private readonly int width, height;
        private readonly GameElement?[,] grid;

        public bool StateRun { get; set; } = true;

        public Ball? Ball { get; set; }
        public EnergyBall? EnergyBall { get; set; }
        public Player Player { get; set; } = new Player(1, 1);
        public List<Enemy> Enemies { get; } = new List<Enemy>();
        public DateTime StartTime { get; set; }
        public int TotalScore { get; set; }
        private int energyBallCount;
        public List<(EnergyBall ball, int x, int y)> EnergyBallList { get; } = new();
        private readonly Hint hint = new();
        private readonly GameManager gameManager;

        public int Width => width;
        public int Height => height;
        public Hint Hint => hint;

        public GameField(int w, int h, IRenderer renderer)
        {
            width = w;
            height = h;
            grid = new GameElement?[w, h];

            Player = new Player(1, 1);
            var initializer = new LevelBuilder(this);
            initializer.InitializeField();

            StartTime = DateTime.Now;
            gameManager = new GameManager(this, renderer);
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

        public GameElement? GetElement(int x, int y) => grid[x, y];

        public bool IsWall(int x, int y) =>
            x < 0 || x >= width || y < 0 || y >= height || Wall.IsWall(grid[x, y]);
        public bool IsEnemy(int x, int y) => Enemy.IsEnemy(Enemies, x, y);

        public bool IsShield(int x, int y, out char dir) => Shield.IsShield(grid[x, y], out dir);

        public bool IsEnergyBall(int x, int y) => EnergyBall.IsEnergyBall(grid[x, y]);

        public void CollectEnergyBall(int x, int y) => EnergyBall.Collect(grid, x, y, ref energyBallCount, Player);

        public bool PlaceShield(int x, int y, char direction) => Shield.PlaceShield(grid, x, y, direction);

        public bool IsMoveable(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return false;
            return grid[x, y]?.IsMoveable() ?? true;
        }

        public void Update(bool playerMoved)
        {
            if (Ball == null) return;

            Ball.Move(this);

            if (playerMoved)
            {
                gameManager.UpdateEnemies();
                hint.ClearHint();
            }

            if (gameManager.CheckGameOverConditions()) return;
            if (gameManager.CheckLevelCompletion()) return;
        }
    }
}