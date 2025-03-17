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
            InitializeField();
            startTime = DateTime.Now;
        }

        private void InitializeField()
        {
            Random rnd = new Random();

            // Cleaning
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    grid[x, y] = null;

            // Walls
            for (int x = 0; x < width; x++)
            {
                grid[x, 0] = new Wall();
                grid[x, height - 1] = new Wall();
            }
            for (int y = 0; y < height; y++)
            {
                grid[0, y] = new Wall();
                grid[width - 1, y] = new Wall();
            }

            // Ball/Player
            Ball = new Ball(width / 2, height / 2);
            Player = new Player(1, 1);

            // Energy balls
            energyBallCount = 1;
            for (int i = 0; i < energyBallCount; i++)
            {
                int x, y;
                do
                {
                    x = rnd.Next(1, width - 1);
                    y = rnd.Next(1, height - 1);
                } while (grid[x, y] != null);

                grid[x, y] = new EnergyBall();
            }

            // Enemies
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

                Enemies.Add(new Enemy(x, y));
            }
        }

        public bool IsWall(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) 
                return true;
            
            return grid[x, y] is Wall;
        }

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

        public void Update(bool playerMoved)
        {
            if (Ball == null) return;
            
            Ball.Move(this);

            if (playerMoved)
            {
                foreach (var enemy in Enemies)
                    enemy.Move(this);
            }

            if (Enemies.Any(o => o.X == Player.X && o.Y == Player.Y))
            {
                Console.Clear();
                Console.WriteLine("Game Over! You were caught by an enemy.");
                Console.WriteLine($"Final Total Score: {TotalScore}");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(0);
            }

            if (energyBallCount == 0)
            {
                TotalScore += Player.Score;
                Console.Clear();
                TimeSpan elapsedTime = DateTime.Now - startTime;
                Console.WriteLine($"You win! Level Score: {Player.Score}, Total Score: {TotalScore}, Time: {elapsedTime.TotalSeconds:F2} seconds");
                System.Threading.Thread.Sleep(2000);
                RestartLevel(false);
                return;
            }
        }

        public void Render()
        {
            Console.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (Ball != null && Ball.X == x && Ball.Y == y)
                        Console.Write("•");
                    else if (Player.X == x && Player.Y == y)
                        Console.Write("I");
                    else if (grid[x, y] is Wall)
                        Console.Write("#");
                    else if (grid[x, y] is Shield s)
                        Console.Write(s.Direction);
                    else if (grid[x, y] is EnergyBall)
                        Console.Write("@");
                    else if (Enemies.Any(e => e.X == x && e.Y == y))
                        Console.Write("E");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
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
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] is EnergyBall)
                    {
                        if (PathExists(Player.X, Player.Y, x, y))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool PathExists(int startX, int startY, int targetX, int targetY)
        {
            bool[,] visited = new bool[width, height];
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((startX, startY));

            while (queue.Count > 0)
            {
                (int x, int y) = queue.Dequeue();
                if (x == targetX && y == targetY) return true;

                if (visited[x, y]) continue;
                visited[x, y] = true;

                foreach ((int dx, int dy) in new (int, int)[] { (0, -1), (0, 1), (-1, 0), (1, 0) })
                {
                    int newX = x + dx, newY = y + dy;
                    if (newX >= 0 && newX < width && newY >= 0 && newY < height && !visited[newX, newY])
                    {
                        if (grid[newX, newY] == null || grid[newX, newY] is EnergyBall)
                        {
                            queue.Enqueue((newX, newY));
                        }
                    }
                }
            }
            return false;
        }
    }
}


