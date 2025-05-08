using System;
using BallGame;

namespace BallGame.Rendering
{
    public class ConsoleRenderer : IRenderer
    {
        public void Render(GameField field)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Score: {field.Player.Score}");
            Console.ResetColor();

            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    Console.SetCursorPosition(x, y + 1);

                    if (field.Hint.GetHintPosition() is (int hx, int hy) && hx == x && hy == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(field.Hint.GetHintDirection() ?? ' ');
                        Console.ResetColor();
                        continue;
                    }

                    if (field.Ball is { X: var bx, Y: var by } && bx == x && by == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("•");
                        Console.ResetColor();
                        continue;
                    }

                    if (field.Player.X == x && field.Player.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("I");
                        Console.ResetColor();
                        continue;
                    }

                    var cell = field[x, y];
                    switch (cell)
                    {
                        case SmartEnemy:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("S");
                            break;
                        case BossEnemy:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("B");
                            break;
                        case Enemy:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("E");
                            break;
                        case Wall:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("#");
                            break;
                        case EnergyBall:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("@");
                            break;
                        case Shield s when Shield.IsShield(s, out var dir):
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(dir);
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" ");
                            break;
                    }
                    Console.ResetColor();
                }
            }

            Console.ResetColor();
        }

        public void RenderAt(int x, int y, string content)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(content);
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void RenderFrame()
        {
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Pause(int milliseconds = 2000)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void WaitForKeyPress(string prompt = "\nPress any key to continue...")
        {
            Console.WriteLine(prompt);
            Console.ReadKey(true);
        }
    }
}
