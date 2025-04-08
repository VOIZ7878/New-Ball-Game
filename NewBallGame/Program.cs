using System;
using System.IO;

namespace BallGame
{
    enum CellType { Empty, Wall, Shield, EnergyBall, Ball, Player, Enemy }
}

namespace BallGame
{
    class Program
    {
        static void Main()
        {
            GameField field = new GameField(10, 10);

            while (true)
            {
                field.Render();

                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape)
                    {
                        Console.Clear();
                        Console.WriteLine($"Exiting game... Final Total Score: {field.TotalScore}");
                        System.Threading.Thread.Sleep(1000);
                        Environment.Exit(0);
                    }
                    else if (key == ConsoleKey.R)
                    {
                        field.RestartLevel(true);
                    }
                    else if (key == ConsoleKey.V)
                    {
                        Console.Clear();
                        string filePath = "GameResults.txt";
                        if (File.Exists(filePath))
                        {
                            Console.WriteLine("Game Results:");
                            Console.WriteLine(File.ReadAllText(filePath));
                        }
                        else
                        {
                            Console.WriteLine("No results found.");
                        }
                        Console.WriteLine("\nPress any key to return to the game...");
                        Console.ReadKey(true);
                    }
                    else
                    {
                        field.Player.Move(key, field);
                    }
                }

                field.Update(false);
                System.Threading.Thread.Sleep(40);
            }
        }
    }
}
