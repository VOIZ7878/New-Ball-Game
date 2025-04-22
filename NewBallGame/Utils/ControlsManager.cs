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
                    gameManager.ShowGameResults();
                    break;

                case ConsoleKey.H:
                    gameField.Hint.CalculateHint(gameField);
                    break;

                case ConsoleKey.J:
                    ILevelLoader loader = new TextFileLevelLoader();
                    loader.Load(gameField, "Levels/level1.txt");
                    break;
                    
                default:
                    gameField.Player.Move(gameField, key);
                    break;
            }
        }
    }
}