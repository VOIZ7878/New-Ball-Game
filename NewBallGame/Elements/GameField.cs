using BallGame.Rendering; 

namespace BallGame
{
    public class GameField
    {
        public bool StateRun { get; set; } = true;
        private int width, height;
        private GameElement?[,] grid;
        public Ball? Ball { get; set; }
        public Player Player { get; set; }
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        private int energyBallCount;
        public DateTime StartTime { get; set; }
        public int TotalScore { get; set; }
        private Hint hint = new Hint();
        private GameManager gameManager;

        public int Width => width;
        public int Height => height;

        public GameElement?[,] Grid => grid;

        public int EnergyBallCount
        {
            get => energyBallCount;
            set => energyBallCount = value;
        }

        public Hint Hint => hint;

        public GameField(int w, int h)
        {
            width = w;
            height = h;
            grid = new GameElement?[w, h];
            Player = new Player(1, 1);

            var initializer = new LevelBuilder(this);
            initializer.InitializeField();

            StartTime = DateTime.Now;

            gameManager = new GameManager(this);
        }

        public GameElement? this[int x, int y]
        {
            get => grid[x, y];
            set => grid[x, y] = value;
           }

        public bool IsWall(int x, int y) => x < 0 || x >= width || y < 0 || y >= height || Wall.IsWall(grid[x, y]);
        public bool IsEnemy(int x, int y) => Enemy.IsEnemy(Enemies, x, y);
        public bool IsShield(int x, int y, out char dir) => Shield.IsShield(grid[x, y], out dir);
        public bool IsEnergyBall(int x, int y) => EnergyBall.IsEnergyBall(grid[x, y]);
        public void CollectEnergyBall(int x, int y) => EnergyBall.Collect(grid, x, y, ref energyBallCount, Player);
        public bool PlaceShield(int x, int y, char direction) => Shield.PlaceShield(grid, x, y, direction);

        public bool IsMoveable(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;

            return grid[x, y]?.IsMoveable() ?? true;
        }

        private bool AllEnergyBallsReachable()
        {
            return EnergyBall.EnergyBallReachable(this, Player);
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

            hint.CalculateHint(this);
        }
        public void RenderField() => RenderField(new ConsoleRenderer());

        public void RenderField(IRenderer renderer)
        {
            Console.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.SetCursorPosition(x, y);

                    if (hint.GetHintPosition() is (int hx, int hy) && hx == x && hy == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(hint.GetHintDirection() ?? ' ');
                    }
                    else if (Ball != null && Ball.X == x && Ball.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("•");
                    }
                    else if (Player.X == x && Player.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("I");
                    }
                    else if (grid[x, y] is Wall)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("#");
                    }
                    else if (grid[x, y] is EnergyBall)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("@");
                    }
                    else if (grid[x, y] is Enemy)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("E");
                    }
                    else if (grid[x, y] is Shield && Shield.IsShield(grid[x, y], out char dir))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(dir);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                }
            }
            Console.ResetColor();
        }
    }
}