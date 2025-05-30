namespace BallGame
{
    public partial class GameManager
    {
        public async Task<bool> CheckGameOverConditionsAsync()
        {
            if (gameField!.Player.Score < 0)
            {
                await HandleGameOverAsync(GameOverMessage);
                return true;
            }

            if (gameField!.IsEnemy(gameField.Player.X, gameField.Player.Y))
            {
                await HandleGameOverAsync(GameOverEnemyMessage);
                return true;
            }

            if (!CanWin())
            {
                await HandleGameOverAsync(GameOverNoBallsMessage);
                return true;
            }

            return false;
        }

        public async Task<bool> CheckLevelCompletionAsync()
        {
            if (gameField!.EnergyBallCount == 0)
            {
                await HandleLevelCompletionAsync();
                return true;
            }

            return false;
        }

        private async Task HandleLevelCompletionAsync()
        {
            SaveResults();
            gameField!.TotalScore += gameField.Player.Score;
            renderer.Clear();
            renderer.WriteLine(string.Format(LevelWinMessage, gameField.Player.Score, gameField.TotalScore, ElapsedTimeSeconds));
            await Task.Delay(2000);

            StartNewGame(false);
        }

        private Task HandleGameOverAsync(string messageTemplate)
        {
            return EndGameAsync(gameField!.TotalScore, ElapsedTimeSeconds, messageTemplate);
        }

        public async Task EndGameAsync(int totalScore, double elapsedTime, string message)
        {
            SaveResults();
            renderer.Clear();
            renderer.WriteLine(string.Format(message, totalScore, elapsedTime));
            await Task.Delay(2000);
        }

        private bool CanWin()
        {
            if (gameField!.EnergyBallCount == 0) return true;

            foreach (var (_, x, y) in gameField.EnergyBallList)
            {
                if (Pathfinding.PathExists(gameField, gameField.Player.X, gameField.Player.Y, x, y))
                    return true;
            }

            return false;
        }
    }
}
