using BallGame.Rendering; 

namespace BallGame
{
    class GameField
    {
        private int width, height;
        private GameElement?[,] grid;
        public Ball? Ball { get; private set; }
        public Player Player { get; private set; }
        private List<Enemy> Enemies = new List<Enemy>();
        private int energyBallCount;
        private int enemiesCount;
        private DateTime startTime;
        public int TotalScore { get; private set; } = 0;

        public GameField(int w, int h)
        {
            width = w;
            height = h;
            grid = new GameElement?[w, h];
            Player = new Player(1, 1);
            InitializeField();
            startTime = DateTime.Now;
        }

        public GameElement? this[int x, int y]
        {
            get => grid[x, y];
            set => grid[x, y] = value;
        }

        private void InitializeField()
        {
            Random rnd = new Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = null;
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        grid[x, y] = new Wall();
                }
            }

            PlaceRandomElements<Wall>(rnd.Next(2, 4));


            Ball = new Ball(width / 2, height / 2);
            Player = new Player(1, 1);


            energyBallCount = rnd.Next(1, 3);
            PlaceRandomElements<EnergyBall>(energyBallCount);


            enemiesCount = rnd.Next(1, 4);
            Enemies.Clear();
            for (int i = 0; i < enemiesCount; i++)
            {
                int x, y;
                do
                {
                    x = rnd.Next(1, width - 1);
                    y = rnd.Next(1, height - 1);
                } while (grid[x, y] != null || IsEnemy(x, y));

                var enemy = new Enemy(x, y);
                Enemies.Add(enemy);
                grid[x, y] = enemy;
            }
        }

        private void PlaceRandomElements<T>(int count) where T : GameElement, new()
        {
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                int x, y;
                do
                {
                    x = rnd.Next(1, width - 1);
                    y = rnd.Next(1, height - 1);
                } while (grid[x, y] != null);

                grid[x, y] = new T();
            }
        }

        public bool IsWall(int x, int y) => x < 0 || x >= width || y < 0 || y >= height || grid[x, y] is Wall;
        public bool IsEnemy(int x, int y) => Enemies.Any(o => o.X == x && o.Y == y);

        public bool IsShield(int x, int y, out char dir)
        {
            if (grid[x, y] is Shield shield)
            {
                dir = shield.Direction;
                return true;
            }
            dir = ' ';
            return false;
        }

        public bool IsEnergyBall(int x, int y) => grid[x, y] is EnergyBall;

        public void CollectEnergyBall(int x, int y)
        {
            grid[x, y] = null;
            energyBallCount--;
            Player.AddScore(100);
        }

        public bool PlaceShield(int x, int y, char direction)
        {
            if (grid[x, y] == null)
            {
                grid[x, y] = new Shield(direction);
                return true;
            }
            return false;
        }

        private void SaveGameResults(int totalScore, double totalTimePlayed)
        {
            string filePath = "GameResults.txt";
            using StreamWriter writer = new StreamWriter(filePath, append: true);
            writer.WriteLine($"Total Score: {totalScore}, Total Time Played: {totalTimePlayed:F2} seconds");
        }

        public void Update(bool playerMoved)
        {
            if (Ball == null) return;

            Ball.Move(this);

            if (playerMoved)
            {
                foreach (var enemy in Enemies)
                {
                    grid[enemy.X, enemy.Y] = null;
                }

                Enemies.ForEach(e =>
                {
                    e.Move(this);
                    grid[e.X, e.Y] = e;
                });
            }

            if (IsEnemy(Player.X, Player.Y))
            {
                var elapsedTime = (DateTime.Now - startTime).TotalSeconds;
                SaveGameResults(TotalScore, elapsedTime);

                Console.Clear();
                Console.WriteLine($"Game Over! Final Total Score: {TotalScore}, Total Time Played: {elapsedTime:F2} seconds");
                Console.WriteLine("Press 'R' to restart or 'Esc' to exit.");

                while (true)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.R)
                    {
                        RestartLevel(true);
                        return;
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
            }

            if (energyBallCount == 0)
            {
                TotalScore += Player.Score;
                var elapsedTime = (DateTime.Now - startTime).TotalSeconds;
                Console.Clear();
                Console.WriteLine($"You win this level! Level Score: {Player.Score}, Total Score: {TotalScore}, Time: {elapsedTime:F2} seconds");
                System.Threading.Thread.Sleep(2000);

                RestartLevel(false);
            }
            else if (!CanWin())
            {
                var elapsedTime = (DateTime.Now - startTime).TotalSeconds;
                SaveGameResults(TotalScore, elapsedTime);

                Console.Clear();
                Console.WriteLine($"Game Over! No reachable energy balls. Final Total Score: {TotalScore}, Total Time Played: {elapsedTime:F2} seconds");
                Console.WriteLine("Press 'R' to restart or 'Esc' to exit.");

                while (true)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.R)
                    {
                        RestartLevel(true);
                        return;
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        public void Render() => Render(new ConsoleRenderer());

        public void Render(IRenderer renderer)
        {
            renderer.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (Ball != null && Ball.X == x && Ball.Y == y)
                        Ball.Render(renderer, x, y);
                    else if (Player.X == x && Player.Y == y)
                        Player.Render(renderer, x, y);
                    else
                        grid[x, y]?.Render(renderer, x, y);
                }
            }
            renderer.RenderFrame();
        }

        public void RestartLevel(bool resetScore = false)
        {
            Console.Clear();
            Console.WriteLine("Restarting level...");
            System.Threading.Thread.Sleep(1000);

            if (resetScore)
            {
                TotalScore = 0;
                Player = new Player(1, 1);
            }
            else
            {
                TotalScore += Player.Score;
            }

            InitializeField();
            startTime = DateTime.Now;
        }

        private bool CanWin()
        {
            if (energyBallCount == 0) return true;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (grid[x, y] is EnergyBall && PathExists(Player.X, Player.Y, x, y))
                        return true;

            return false;
        }

        private bool PathExists(int startX, int startY, int targetX, int targetY)
        {
            bool[,] visited = new bool[width, height];
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
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny] &&
                        (grid[nx, ny] == null || grid[nx, ny] is EnergyBall))
                        queue.Enqueue((nx, ny));
                }
            }

            return false;
        }
    }
}