using BallGame.Rendering;

namespace BallGame
{
    public class GameManager
    {
        private GameField gameField;
        private readonly IRenderer renderer;
        private readonly GameResultManager resultSaver;
        private const string GameOverMessage = "Game Over! Final Total Score: {0}, Total Time Played: {1:F2} seconds";
        private const string GameOverNoBallsMessage = "Game Over! No reachable energy balls. Final Total Score: {0}, Total Time Played: {1:F2} seconds";
        private const string LevelWinMessage = "You win this level! Level Score: {0}, Total Score: {1}, Time: {2:F2} seconds";
        private double ElapsedTimeSeconds => (DateTime.Now - gameField.StartTime).TotalSeconds;

        public GameManager(GameField field, IRenderer renderer)
        {
            gameField = field;
            resultSaver = new GameResultManager();
            this.renderer = renderer;
        }

        public void SaveResults()
        {
            resultSaver.Save(gameField.TotalScore, ElapsedTimeSeconds);
        }

        public void ShowGameResults()
        {
            GameResultManager.ShowSavedResults();
        }

        public void UpdateEnemies()
        {
            Enemy.UpdateEnemies(gameField.Enemies, gameField.Grid, gameField);
        }

        public bool CheckGameOverConditions()
        {
            if (gameField.IsEnemy(gameField.Player.X, gameField.Player.Y))
            {
                HandleGameOver(GameOverMessage);
                return true;
            }

            if (!CanWin())
            {
                HandleGameOver(GameOverNoBallsMessage);
                return true;
            }

            return false;
        }

        private void HandleGameOver(string messageTemplate)
        {
            EndGame(gameField.TotalScore, ElapsedTimeSeconds, messageTemplate);
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
            Console.Clear();
            Console.WriteLine(string.Format(LevelWinMessage, gameField.Player.Score, gameField.TotalScore, ElapsedTimeSeconds));
            System.Threading.Thread.Sleep(2000);

            RestartLevel(false);
        }

        private bool CanWin()
        {
            if (gameField.EnergyBallCount == 0) return true;

            foreach (var (_, x, y) in gameField.EnergyBallList)
            {
                if (Pathfinding.PathExists(gameField, gameField.Player.X, gameField.Player.Y, x, y))
                    return true;
            }

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
            SaveResults();
            Console.Clear();
            Console.WriteLine(string.Format(message, totalScore, elapsedTime));
            System.Threading.Thread.Sleep(2000);
            var app = new GameRunner();
            app.Run();
        }
    }
}