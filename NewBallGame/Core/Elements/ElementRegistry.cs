namespace BallGame.Rendering
{
    public class ElementInfo
    {
        public char Symbol;
        public Func<int, int, GameElement> Factory;
        public string VisualSymbol;
        public ConsoleColor ConsoleColor;
        public Brush WinFormsBrush;

        public ElementInfo(char symbol, Func<int, int, GameElement> factory, string visualSymbol, ConsoleColor consoleColor, Brush winFormsBrush)
        {
            Symbol = symbol;
            Factory = factory;
            VisualSymbol = visualSymbol;
            ConsoleColor = consoleColor;
            WinFormsBrush = winFormsBrush;
        }
    }

    public static class ElementRegistry
    {
        public static readonly Dictionary<Type, ElementInfo> TypeToInfo = new()
        {
            { typeof(Wall), new ElementInfo('#', (x, y) => new Wall(), "#", ConsoleColor.Gray, Brushes.DarkGray) },
            { typeof(Player), new ElementInfo('I', (x, y) => new Player(x, y), "I", ConsoleColor.Green, Brushes.Green) },
            { typeof(Ball), new ElementInfo('O', (x, y) => new Ball(x, y), "O", ConsoleColor.Red, Brushes.White) },
            { typeof(EnergyBall), new ElementInfo('@', (x, y) => new EnergyBall(), "@", ConsoleColor.Cyan, Brushes.Yellow) },
            { typeof(Enemy), new ElementInfo('E', (x, y) => new Enemy(x, y), "E", ConsoleColor.Magenta, Brushes.Red) },
            { typeof(SmartEnemy), new ElementInfo('S', (x, y) => new SmartEnemy(x, y), "S", ConsoleColor.Red, Brushes.White) },
            { typeof(BossEnemy), new ElementInfo('B', (x, y) => new BossEnemy(x, y), "B", ConsoleColor.Red, Brushes.White) },
            { typeof(Hint), new ElementInfo('*', (x, y) => new Hint(), "*", ConsoleColor.Yellow, Brushes.Yellow) },
        };

        public static readonly Dictionary<char, ElementInfo> SymbolToInfo = new();

        static ElementRegistry()
        {
            foreach (var info in TypeToInfo.Values)
                SymbolToInfo[info.Symbol] = info;
        }
    }

    public struct ElementVisualData
    {
        public string Symbol { get; }
        public ConsoleColor Color { get; }
        public Brush Brush { get; }
        public ElementVisualData(string symbol, ConsoleColor color, Brush brush)
        {
            Symbol = symbol;
            Color = color;
            Brush = brush;
        }
    }

    public static class ElementVisuals
    {
        public static ElementVisualData Get(GameElement element)
        {
            if (element is Shield shield)
            {
                return new ElementVisualData(
                    shield.Direction.ToString(),
                    ConsoleColor.Blue,
                    Brushes.Cyan
                );
            }
            if (ElementRegistry.TypeToInfo.TryGetValue(element.GetType(), out var info))
                return new ElementVisualData(info.VisualSymbol, info.ConsoleColor, info.WinFormsBrush);
            return new ElementVisualData(" ", ConsoleColor.White, Brushes.White);
        }
    }
}