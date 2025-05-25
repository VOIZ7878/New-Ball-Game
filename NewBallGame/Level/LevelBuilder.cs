using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    public class LevelBuilder
    {
        private readonly GameField gameField;
        private readonly Random rnd = new Random();
        private readonly IRenderer renderer;
        private readonly IInputManager inputManager;
        private readonly ISoundManager soundManager;

        public LevelBuilder(GameField gameField)
        {
            this.gameField = gameField;
            renderer = new ConsoleRenderer();
            inputManager = new ConsoleInputManager();
            soundManager = new SoundManager();
        }

        public void InitializeField()
        {
            const int maxAttempts = 10;
            int attempt = 0;

            while (attempt++ < maxAttempts)
            {
                InitializeGrid();
                PlaceWalls();
                PlaceBall();
                PlacePlayer();
                PlaceEnergyBalls();
                PlaceEnemies();

                if (IsLevelPassable())
                    return;
            }

            throw new Exception("Failed to generate a passable level.");
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < gameField.Width; x++)
                for (int y = 0; y < gameField.Height; y++)
                    gameField[x, y] = null;

            gameField.Enemies.Clear();
            gameField.EnergyBallList.Clear();
        }

        private void PlaceWalls()
        {
            for (int x = 0; x < gameField.Width; x++)
            {
                for (int y = 0; y < gameField.Height; y++)
                {
                    if (x == 0 || y == 0 || x == gameField.Width - 1 || y == gameField.Height - 1)
                        gameField[x, y] = new Wall();
                }
            }

            int wallCount = rnd.Next(5, 15);
            for (int i = 0; i < wallCount; i++)
            {
                int x, y;
                do
                {
                    x = rnd.Next(1, gameField.Width - 1);
                    y = rnd.Next(1, gameField.Height - 1);
                }
                while (gameField[x, y] != null ||
                       (gameField.Ball != null && Math.Abs(x - gameField.Ball.X) <= 1 && Math.Abs(y - gameField.Ball.Y) <= 1) ||
                       (gameField.Player != null && gameField.Player.X == x && gameField.Player.Y == y));

                gameField[x, y] = new Wall();
            }
        }

        private void PlaceBall()
        {
            gameField.Ball = new Ball(gameField.Width / 2, gameField.Height / 2);
        }

        private void PlacePlayer()
        {
            int x, y;
            do
            {
                x = rnd.Next(1, gameField.Width - 1);
                y = rnd.Next(1, gameField.Height - 1);
            }
            while (gameField[x, y] != null);
            gameField.Player = new Player(x, y);
        }

        private void PlaceEnergyBalls()
        {
            if (gameField.Ball == null) return;

            int energyBallCount = rnd.Next(2, 4);
            gameField.EnergyBallList.Clear();

            for (int i = 0; i < energyBallCount; i++)
            {
                int x, y;
                do
                {
                    x = rnd.Next(1, gameField.Width - 1);
                    y = rnd.Next(1, gameField.Height - 1);
                }
                while (gameField[x, y] != null || Math.Abs(x - gameField.Ball.X) <= 1 && Math.Abs(y - gameField.Ball.Y) <= 1);

                var energyBall = new EnergyBall();
                gameField[x, y] = energyBall;
                gameField.EnergyBallList.Add((energyBall, x, y));

                if (i == 0)
                    gameField.EnergyBall = energyBall;
            }

            gameField.EnergyBallCount = energyBallCount;
        }

        private void PlaceEnemies()
        {
            int enemiesCount = rnd.Next(1, 4);

            for (int i = 0; i < enemiesCount; i++)
            {
                int x, y;
                do
                {
                    x = rnd.Next(1, gameField.Width - 1);
                    y = rnd.Next(1, gameField.Height - 1);
                }
                while (gameField[x, y] != null ||
                       gameField.IsEnemy(x, y) ||
                       (gameField.Ball != null && x == gameField.Ball.X && y == gameField.Ball.Y) ||
                       (gameField.Player != null && x == gameField.Player.X && y == gameField.Player.Y));

                var enemy = new Enemy(x, y);
                gameField.Enemies.Add(enemy);
                gameField[x, y] = enemy;
            }
        }

        private bool IsLevelPassable()
        {
            if (gameField.Ball == null || gameField.Player == null || gameField.EnergyBallList == null || gameField.EnergyBallList.Count == 0)
                return false;

            foreach (var (_, x, y) in gameField.EnergyBallList)
            {
                if (!Pathfinding.PathExists(gameField, gameField.Ball.X, gameField.Ball.Y, x, y))
                    return false;
            }

            if (!Pathfinding.PathExists(gameField, gameField.Player.X, gameField.Player.Y, gameField.Ball.X, gameField.Ball.Y))
                return false;

            foreach (var (_, x, y) in gameField.EnergyBallList)
            {
                if (!Pathfinding.PathExists(gameField, gameField.Player.X, gameField.Player.Y, x, y))
                    return false;
            }

            return true;
        }
    }
}
