namespace BallGame
{
    public partial class GameManager
    {
        private void UpdateLastSessionInfo(GameField field, DateTime startTime)
        {
            lastScore = field.Player.Score;
            lastTimePlayed = (DateTime.Now - startTime).TotalSeconds;
            lastDate = DateTime.Now;
        }

        private void HandlePausedState(GameField field, DateTime startTime)
        {
            gameStateManager.SaveGameState(field);
            UpdateLastSessionInfo(field, startTime);
        }

        private void HandleExitState(GameField field, DateTime startTime)
        {
            gameStateManager.SaveGameState(field);
            UpdateLastSessionInfo(field, startTime);
        }

        private void HandleMainMenuState(GameField field, DateTime startTime)
        {
            UpdateLastSessionInfo(field, startTime);
        }

        private async Task<bool> HandleRunningState(GameField field, bool playerMoved, DateTime startTime)
        {
            field.Update(playerMoved);

            if (playerMoved)
                UpdateEnemies();

            if (await CheckGameOverConditionsAsync())
            {
                SaveResults();
                return false;
            }

            if (await CheckLevelCompletionAsync())
            {
                int prevScore = field.TotalScore;
                int prevPlayerScore = field.Player.Score;

                gameField = StartNewGame(false);
                gameField.TotalScore = prevScore;
                gameField.Player.Score = prevPlayerScore;
                field = gameField;
                renderer.PreRender(field);
                return true;
            }
            return true;
        }

        private void HandleRestartState(ref GameField field, ref ControlsManager controls)
        {
            SaveResultsWithCurrentScore();
            gameField = StartNewGame(true);
            field = gameField;
            controls = new ControlsManager(field, gameStateManager, inputManager, renderer);
            renderer.PreRender(field);
        }

        private void HandleGameOverState(GameField field, DateTime startTime)
        {
            SaveResults();
            soundManager.PlaySoundEffect("lose.mp3");
            UpdateLastSessionInfo(field, startTime);
        }
        
    }
}
