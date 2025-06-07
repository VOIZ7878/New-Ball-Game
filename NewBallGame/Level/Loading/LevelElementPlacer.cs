namespace BallGame.Level
{
    public static class LevelElementPlacer
    {
        private static readonly Dictionary<Type, Action<GameField, GameElement, int, int>> actions = new()
        {
            { typeof(Player), (field, e, x, y) => field.Player = (Player)e },
            { typeof(Ball), (field, e, x, y) => field.Ball = (Ball)e },
            { typeof(EnergyBall), (field, e, x, y) => { field[x, y] = e; field.EnergyBallList.Add(((EnergyBall)e, x, y)); } },
            { typeof(Enemy), (field, e, x, y) => { field[x, y] = e; field.Enemies.Add((Enemy)e); } },
            { typeof(SmartEnemy), (field, e, x, y) => { field[x, y] = e; field.Enemies.Add((Enemy)e); } },
            { typeof(BossEnemy), (field, e, x, y) => { field[x, y] = e; field.Enemies.Add((Enemy)e); } }
        };

        public static void PlaceSymbol(GameField gameField, char symbol, int x, int y)
        {
            if (symbol == '/' || symbol == '\\')
            {
                gameField[x, y] = new Shield(symbol);
                return;
            }
            if (symbol == '.')
                return;

            if (BallGame.Rendering.ElementRegistry.SymbolToInfo.TryGetValue(symbol, out var info))
            {
                var element = info.Factory(x, y);
                if (actions.TryGetValue(element.GetType(), out var act))
                {
                    act(gameField, element, x, y);
                }
                else
                {
                    gameField[x, y] = element;
                }
                return;
            }
            throw new Exception($"Unknown symbol in level file at ({x},{y}): '{symbol}'");
        }
        
        public static void LoadSymbols(GameField gameField, char[][] grid)
        {
            int height = grid.Length;
            if (height == 0) return;
            int width = grid[0].Length;
            for (int y = 0; y < height; y++)
            {
                if (grid[y].Length != width)
                    throw new Exception($"Grid row {y} has inconsistent length.");
                for (int x = 0; x < width; x++)
                {
                    gameField[x, y] = null;
                    LevelElementPlacer.PlaceSymbol(gameField, grid[y][x], x, y);
                }
            }
        }
    }
}
