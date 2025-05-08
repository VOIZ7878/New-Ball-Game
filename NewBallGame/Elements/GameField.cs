using BallGame.Rendering;
using System.Text.Json;

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


        public GameField(int w, int h, IRenderer renderer, bool initialize = true)
        {
            width = w;
            height = h;
            grid = new GameElement?[w, h];

            Player = new Player(1, 1);

            if (initialize)
            {
                var initializer = new LevelBuilder(this);
                initializer.InitializeField();
            }

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

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
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

        public string Serialize()
        {
            var gridData = new List<string>();
            for (int y = 0; y < Height; y++)
            {
                var row = new char[Width];
                for (int x = 0; x < Width; x++)
                {
                    row[x] = Grid[x, y] switch
                    {
                        Wall => '#',
                        Player _ => 'P',
                        Ball _ => 'B',
                        EnergyBall _ => 'E',
                        Enemy => 'X',
                        Shield s when Shield.IsShield(s, out var dir) => dir,
                        _ => '.'
                    };
                }
                gridData.Add(new string(row));
            }

            return JsonSerializer.Serialize(new
            {
                Width,
                Height,
                Grid = gridData,
                Player = new { Player.X, Player.Y, Player.Score },
                Ball = Ball != null ? new { Ball.X, Ball.Y, Ball.Dx, Ball.Dy } : null,
                EnergyBallCount,
                TotalScore
            });
        }

        public static GameField Deserialize(string json, IRenderer renderer)
        {
            var data = JsonSerializer.Deserialize<SerializedGameField>(json);
            if (data == null) throw new Exception("Failed to deserialize game field.");

            var gameField = new GameField(data.Width, data.Height, renderer, initialize: false)
            {
                TotalScore = data.TotalScore,
                EnergyBallCount = data.EnergyBallCount
            };

            for (int y = 0; y < data.Grid.Count; y++)
            {
                for (int x = 0; x < data.Grid[y].Length; x++)
                {
                    char symbol = data.Grid[y][x];
                    switch (symbol)
                    {
                        case '#':
                            gameField[x, y] = new Wall();
                            break;
                        case 'P':
                            gameField.Player = new Player(x, y);
                            break;
                        case 'B':
                            gameField.Ball = new Ball(x, y);
                            break;
                        case 'E':
                            var energyBall = new EnergyBall();
                            gameField[x, y] = energyBall;
                            gameField.EnergyBallList.Add((energyBall, x, y));
                            break;
                        case 'X':
                            var enemy = new Enemy(x, y);
                            gameField[x, y] = enemy;
                            gameField.Enemies.Add(enemy);
                            break;
                        case '/':
                        case '\\':
                            gameField[x, y] = new Shield(symbol);
                            break;
                    }
                }
            }

            if (data.Player != null)
            {
                gameField.Player = new Player(data.Player.X, data.Player.Y)
                {
                    Score = data.Player.Score 
                };
            }

            if (data.Ball != null)
            {
                gameField.Ball = new Ball(data.Ball.X, data.Ball.Y)
                {
                    Dx = data.Ball.Dx,
                    Dy = data.Ball.Dy
                };
            }

            return gameField;
        }

        private class SerializedGameField
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public List<string> Grid { get; set; } = new();
            public int TotalScore { get; set; }
            public int EnergyBallCount { get; set; }
            public PlayerData? Player { get; set; }
            public BallData? Ball { get; set; }

            public class PlayerData
            {
                public int X { get; set; }
                public int Y { get; set; }
                public int Score { get; set; }
            }

            public class BallData
            {
                public int X { get; set; }
                public int Y { get; set; }
                public int Dx { get; set; }
                public int Dy { get; set; }
            }
        }
    }
}