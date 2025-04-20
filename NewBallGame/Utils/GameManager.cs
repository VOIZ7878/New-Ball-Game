using BallGame.Rendering;

namespace BallGame
{
    public class GameManager
    {
        private GameField gameField;

        public GameManager(GameField field)
        {
            gameField = field;
        }

        public void SaveResults(int totalScore, double totalTimePlayed)
        {
            string filePath = "GameResults.txt";
            using StreamWriter writer = new StreamWriter(filePath, append: true);
            writer.WriteLine($"Total Score: {totalScore}, Total Time Played: {totalTimePlayed:F2} seconds");
        }

        public void UpdateEnemies()
        {
            Enemy.UpdateEnemies(gameField.Enemies, gameField.Grid, gameField);
        }

        public bool CheckGameOverConditions()
        {
            if (gameField.IsEnemy(gameField.Player.X, gameField.Player.Y))
            {
                HandleGameOver("Game Over! Final Total Score: {0}, Total Time Played: {1:F2} seconds");
                return true;
            }

            if (!CanWin())
            {
                HandleGameOver("Game Over! No reachable energy balls. Final Total Score: {0}, Total Time Played: {1:F2} seconds");
                return true;
            }

            return false;
        }

        private void HandleGameOver(string message)
        {
            var elapsedTime = (DateTime.Now - gameField.StartTime).TotalSeconds;
            EndGame(gameField.TotalScore, elapsedTime, message);
        }

        public bool CheckLevelCompletion()
        {
            if (gameField.EnergyBallCount == 0)
            {
                HandleLevelCompletion();
                return true;
            }

            return false;
        }

        private void HandleLevelCompletion()
        {
            gameField.TotalScore += gameField.Player.Score;
            var elapsedTime = (DateTime.Now - gameField.StartTime).TotalSeconds;

            Console.Clear();
            Console.WriteLine($"You win this level! Level Score: {gameField.Player.Score}, Total Score: {gameField.TotalScore}, Time: {elapsedTime:F2} seconds");
            System.Threading.Thread.Sleep(2000);

            RestartLevel(false);
        }

        private bool CanWin()
        {
            if (gameField.EnergyBallCount == 0) return true;

            for (int x = 0; x < gameField.Width; x++)
                for (int y = 0; y < gameField.Height; y++)
                    if (gameField[x, y] is EnergyBall && Pathfinding.PathExists(gameField, gameField.Player.X, gameField.Player.Y, x, y))
                        return true;

            return false;
        }

        public void RestartLevel(bool resetScore)
        {
            var levelBuilder = new LevelBuilder(gameField);
            levelBuilder.InitializeField();

            if (resetScore)
            {
                gameField.TotalScore = 0;
                gameField.Player.ResetScore();
            }

            gameField.StartTime = DateTime.Now;
        }

        public void EndGame(int totalScore, double elapsedTime, string message)
        {
            Console.Clear();
            Console.WriteLine(string.Format(message, totalScore, elapsedTime));
            System.Threading.Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}