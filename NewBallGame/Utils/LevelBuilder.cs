namespace BallGame
{
    public class LevelBuilder
    {
        private readonly GameField gameField;
        private readonly Random rnd = new Random();

        public LevelBuilder(GameField gameField)
        {
            this.gameField = gameField;
        }

        public void InitializeField()
        {
            InitializeGrid();
            PlaceWalls();
            PlaceBallAndPlayer();
            PlaceEnergyBalls();
            PlaceEnemies();
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < gameField.Width; x++)
            {
                for (int y = 0; y < gameField.Height; y++)
                {
                    gameField[x, y] = null;
                }
            }
        }

        private void PlaceWalls()
        {
            for (int x = 0; x < gameField.Width; x++)
            {
                for (int y = 0; y < gameField.Height; y++)
                {
                    if (x == 0 || x == gameField.Width - 1 || y == 0 || y == gameField.Height - 1)
                        gameField[x, y] = new Wall();
                }
            }
        }

        private void PlaceBallAndPlayer()
        {
            gameField.Ball = new Ball(gameField.Width / 2, gameField.Height / 2);
            gameField.Player = new Player(1, 1);
        }

        private void PlaceEnergyBalls()
        {
            if (gameField.Ball == null) return;

            int energyBallCount = rnd.Next(1, 3);
            gameField.PlaceRandomElements<EnergyBall>(energyBallCount, avoidNear: (gameField.Ball.X, gameField.Ball.Y));
            gameField.SetEnergyBallCount(energyBallCount);
        }

        private void PlaceEnemies()
        {
            int enemiesCount = rnd.Next(1, 4);
            gameField.Enemies.Clear();
            for (int i = 0; i < enemiesCount; i++)
            {
                int x, y;
                do
                {
                    x = rnd.Next(1, gameField.Width - 1);
                    y = rnd.Next(1, gameField.Height - 1);
                } while (gameField[x, y] != null || gameField.IsEnemy(x, y) || (x == gameField.Player.X && y == gameField.Player.Y) || (gameField.Ball != null && x == gameField.Ball.X && y == gameField.Ball.Y));

                var enemy = new Enemy(x, y);
                gameField.Enemies.Add(enemy);
                gameField[x, y] = enemy;
            }
        }
    }
}