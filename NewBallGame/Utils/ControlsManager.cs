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

        public bool HandleInput()
        {
            if (!Console.KeyAvailable)
                return false;

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.Escape:
                    Console.Clear();
                    Console.WriteLine($"Exiting game... Final Total Score: {gameField.TotalScore}");
                    System.Threading.Thread.Sleep(1000);
                    gameField.StateRun = false;
                    return false;

                case ConsoleKey.R:
                    gameManager.RestartLevel(true);
                    return false;

                case ConsoleKey.V:
                    gameManager.ShowGameResults();
                    return false;

                case ConsoleKey.H:
                    gameField.Hint.CalculateHint(gameField);
                    return false;

                case ConsoleKey.Q:
                    Console.WriteLine("Saving game...");
                    var gameRunner = new GameRunner();
                    gameRunner.SaveGameState(gameField);
                    Console.WriteLine("Game saved successfully!");
                    System.Threading.Thread.Sleep(1000);
                    return false;

                default:
                    return gameField.Player.Move(gameField, key);
            }
        }
    }
}