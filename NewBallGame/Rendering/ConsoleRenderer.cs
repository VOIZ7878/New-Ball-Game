namespace BallGame.Rendering
{
    public class ConsoleRenderer : IRenderer
    {
        public void PreRender(GameField field)
        {
            Console.Clear();
            DrawScore(field);
            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    var cell = field[x, y];
                    if (cell is Wall)
                    {
                        var visual = ElementVisuals.Get(cell);
                        Console.SetCursorPosition(x, y + 1);
                        Console.ForegroundColor = visual.Color;
                        Console.Write(visual.Symbol);
                        Console.ResetColor();
                    }
                }
            }
        }

        public void PostRender(GameField field)
        {
            DrawScore(field);
            var rayPath = field.Hint.RayPathPoints;
            var hintPos = field.Hint.HintPosition;
            var hintDir = field.Hint.HintDirection;
            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    var cell = field[x, y];
                    if (cell is Wall) continue;

                    Console.SetCursorPosition(x, y + 1);
                    char symbol = ' ';
                    ConsoleColor color = Console.ForegroundColor;

                    if (field.Player != null && field.Player.X == x && field.Player.Y == y)
                    {
                        var visual = ElementVisuals.Get(field.Player);
                        symbol = visual.Symbol[0];
                        color = visual.Color;
                    }
                    else if (field.Ball != null && field.Ball.X == x && field.Ball.Y == y)
                    {
                        var visual = ElementVisuals.Get(field.Ball);
                        symbol = visual.Symbol[0];
                        color = visual.Color;
                    }
                    else if (hintPos.HasValue && hintPos.Value.x == x && hintPos.Value.y == y && hintDir.HasValue)
                    {
                        var visual = ElementVisuals.Get(field.Hint);
                        symbol = hintDir.Value.ToString()[0];
                        color = visual.Color;
                    }
                    else if (rayPath.Any(p => p.x == x && p.y == y))
                    {
                        symbol = '.';
                        color = ConsoleColor.Cyan;
                    }
                    else if (field.IsEnergyBall(x, y))
                    {
                        symbol = '@';
                        color = ConsoleColor.Yellow;
                    }
                    else if (cell != null)
                    {
                        var visual = ElementVisuals.Get(cell);
                        symbol = visual.Symbol[0];
                        color = visual.Color;
                    }

                    Console.ForegroundColor = color;
                    Console.Write(symbol);
                    Console.ResetColor();
                }
            }
        }

        private void DrawScore(GameField field)
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(new string(' ', 20));
            Console.SetCursorPosition(0, 0);
            if (field.Player != null)
                Console.WriteLine($"Score: {field.Player.Score}");
            else
                Console.WriteLine("Score: 0");
            Console.ResetColor();
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Pause(int milliseconds = 2000)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }
    }
}
