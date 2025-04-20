using System;
using System.IO;

namespace BallGame
{
    public class ControlsManager
    {
        private GameField gameField;
        private GameManager gameManager;

        public ControlsManager(GameField field, GameManager manager)
        {
            gameField = field;
            gameManager = manager;
        }

        public void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.Escape:
                    Console.Clear();
                    Console.WriteLine($"Exiting game... Final Total Score: {gameField.TotalScore}");
                    System.Threading.Thread.Sleep(1000);
                    gameField.StateRun = false;
                    break;

                case ConsoleKey.R:
                    gameManager.RestartLevel(true);
                    break;

                case ConsoleKey.V:
                    ShowGameResults();
                    break;

                case ConsoleKey.H:
                    gameField.Hint.CalculateHint(gameField);
                    break;

                default:
                    gameField.Player.Move(key, gameField);
                    break;
            }
        }

        private void ShowGameResults()
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
    }
}