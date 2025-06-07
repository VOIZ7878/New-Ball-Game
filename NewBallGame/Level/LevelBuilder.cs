namespace BallGame
{
    public class LevelBuilder
    {
        private readonly Random rnd = new();

        private GameField field = null!;

        public GameField CreateGameField(bool randomSize = true)
        {
            int width = 10;
            int height = 10;

            if (randomSize)
            {
                width += rnd.Next(5);
                height += rnd.Next(5);
            }

            return new GameField(width, height);
        }

        public void InitializeField(GameField gameField)
        {
            const int maxAttempts = 10;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                field = gameField;
                InitializeGrid();
                PlacePerimeterWalls();

                field.Ball = new Ball(field.Width / 2, field.Height / 2);
                PlacePlayer();

                /*PlaceElements(
                    count: 1,
                    factory: (x, y) => new Ball(x, y),
                    canPlace: (x, y) => true,
                    onPlaced: (e, x, y) => field.Ball = (Ball)e
                );

                PlaceElements(
                    count: 1,
                    factory: (x, y) => new Player(x, y),
                    canPlace: (x, y) => field[x, y] == null,
                    onPlaced: (e, x, y) => field.Player = (Player)e
                );*/

                // Walls
                PlaceElements(
                    count: rnd.Next(5, 15),
                    factory: (x, y) => new Wall(),
                    canPlace: (x, y) => Math.Abs(x - field.Ball.X) > 1 || Math.Abs(y - field.Ball.Y) > 1
                        && (field.Player == null || field.Player.X != x || field.Player.Y != y)
                );

                // EnergyBalls
                int energyCount = rnd.Next(2, 4);
                field.EnergyBallList.Clear();
                EnergyBall? first = null;
                PlaceElements(
                    count: energyCount,
                    factory: (x, y) => new EnergyBall(),
                    canPlace: (x, y) => Math.Abs(x - field.Ball.X) > 1 || Math.Abs(y - field.Ball.Y) > 1,
                    onPlaced: (e, x, y) => {
                        var eb = (EnergyBall)e;
                        field.EnergyBallList.Add((eb, x, y));
                        if (first == null) first = eb;
                    }
                );
                field.EnergyBall = first;
                field.EnergyBallCount = energyCount;

                // Enemies
                PlaceElements(
                    count: rnd.Next(1, 4),
                    factory: (x, y) => new Enemy(x, y),
                    canPlace: (x, y) => !field.IsEnemy(x, y)
                        && (x != field.Ball.X || y != field.Ball.Y)
                        && (field.Player == null || x != field.Player.X || y != field.Player.Y),
                    onPlaced: (e, x, y) => field.Enemies.Add((Enemy)e)
                );

                if (IsLevelPassable()) return;
            }

            throw new Exception("Failed to generate a passable level.");
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < field.Width; x++)
                for (int y = 0; y < field.Height; y++)
                    field[x, y] = null;

            field.Enemies.Clear();
            field.EnergyBallList.Clear();
        }

        private void PlacePerimeterWalls()
        {
            for (int x = 0; x < field.Width; x++)
                for (int y = 0; y < field.Height; y++)
                    if (x == 0 || y == 0 || x == field.Width - 1 || y == field.Height - 1)
                        field[x, y] = new Wall();
        }

        private void PlacePlayer()
        {
            int x, y;
            do
            {
                x = rnd.Next(1, field.Width - 1);
                y = rnd.Next(1, field.Height - 1);
            }
            while (field[x, y] != null);

            field.Player = new Player(x, y);
        }
        
        private void PlaceElements(
            int count,
            Func<int, int, GameElement> factory,
            Func<int, int, bool>? canPlace = null,
            Action<GameElement, int, int>? onPlaced = null)
        {
            int placed = 0, attempts = 0;
            int maxAttempts = count * 20;
            while (placed < count && attempts++ < maxAttempts)
            {
                int x = rnd.Next(1, field.Width - 1);
                int y = rnd.Next(1, field.Height - 1);
                if (field[x, y] != null) continue;
                if (canPlace != null && !canPlace(x, y)) continue;
                var element = factory(x, y);
                field[x, y] = element;
                onPlaced?.Invoke(element, x, y);
                placed++;
            }
            if (placed < count)
                throw new Exception($"Failed to place elements ({placed}/{count})");
        }

        private bool IsLevelPassable()
        {
            if (field.Ball == null || field.Player == null || field.EnergyBallList.Count == 0)
                return false;

            if (!Pathfinding.PathExists(field, field.Player.X, field.Player.Y, field.Ball.X, field.Ball.Y))
                return false;

            foreach (var (_, x, y) in field.EnergyBallList)
                if (!Pathfinding.PathExists(field, field.Ball.X, field.Ball.Y, x, y))
                    return false;

            return true;
        }
    }
}