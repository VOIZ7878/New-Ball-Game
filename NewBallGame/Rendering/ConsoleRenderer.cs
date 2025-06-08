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
                        DrawCell(x, y, visual.Symbol[0], visual.Color);
                    }
                }
            }
        }

        private (char symbol, ConsoleColor color) GetVisual(GameElement element, char? overrideSymbol = null)
        {
            if (element == null) return (' ', Console.ForegroundColor);
            var visual = ElementVisuals.Get(element);
            char symbol = overrideSymbol ?? visual.Symbol[0];
            return (symbol, visual.Color);
        }

        private void DrawCell(int x, int y, char symbol, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y + 1);
            Console.ForegroundColor = color;
            Console.Write(symbol);
            Console.ResetColor();
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

                    char symbol = ' ';
                    ConsoleColor color = Console.ForegroundColor;

                    if (field.Player != null && field.Player.X == x && field.Player.Y == y)
                    {
                        (symbol, color) = GetVisual(field.Player);
                    }
                    else if (field.Ball != null && field.Ball.X == x && field.Ball.Y == y)
                    {
                        (symbol, color) = GetVisual(field.Ball);
                    }
                    else if (cell is Enemy)
                    {
                        (symbol, color) = GetVisual((GameElement)cell);
                    }
                    else if (field.IsEnergyBall(x, y))
                    {
                        symbol = '@';
                        color = ConsoleColor.Yellow;
                    }
                    else if ((hintPos.HasValue && hintPos.Value.x == x && hintPos.Value.y == y && hintDir.HasValue))
                    {
                        (symbol, color) = GetVisual(field.Hint, hintDir.Value.ToString()[0]);
                    }
                    else if (rayPath.Any(p => p.x == x && p.y == y))
                    {
                        symbol = '.';
                        color = ConsoleColor.Cyan;
                    }
                    else if (cell is GameElement ge)
                    {
                        (symbol, color) = GetVisual(ge);
                    }

                    DrawCell(x, y, symbol, color);
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
