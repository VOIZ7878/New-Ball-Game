namespace BallGame.Rendering
{
    public class ConsoleRenderer : IRenderer
    {
        public void PreRender(GameField field)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(new string(' ', 20));
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Score: {field.Player.Score}");
            Console.ResetColor();

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
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(new string(' ', 20));
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Score: {field.Player.Score}");
            Console.ResetColor();

            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    var cell = field[x, y];
                    bool isPlayer = (field.Player.X == x && field.Player.Y == y);
                    bool isBall = (field.Ball is { X: var bx, Y: var by } && bx == x && by == y);

                    var hintPos = field.Hint.GetHintPosition();
                    var hintDir = field.Hint.GetHintDirection();
                    bool isHint = hintPos.HasValue && hintPos.Value.x == x && hintPos.Value.y == y;

                    if (cell is Wall)
                        continue;

                    Console.SetCursorPosition(x, y + 1);

                    if (isPlayer)
                    {
                        var playerVisual = ElementVisuals.Get(field.Player);
                        Console.ForegroundColor = playerVisual.Color;
                        Console.Write(playerVisual.Symbol);
                    }
                    else if (isBall && field.Ball != null)
                    {
                        var ballVisual = ElementVisuals.Get(field.Ball);
                        Console.ForegroundColor = ballVisual.Color;
                        Console.Write(ballVisual.Symbol);
                    }
                    else if (isHint && hintDir.HasValue)
                    {
                        var hintVisual = ElementVisuals.Get(field.Hint);
                        Console.ForegroundColor = hintVisual.Color;
                        Console.Write(hintDir.Value);
                    }
                    else if (cell != null)
                    {
                        var visual = ElementVisuals.Get(cell);
                        Console.ForegroundColor = visual.Color;
                        Console.Write(visual.Symbol);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                    Console.ResetColor();
                }
            }
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
