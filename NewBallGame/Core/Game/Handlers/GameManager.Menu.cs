namespace BallGame
{
    public partial class GameManager
    {
        private async Task HandleMainMenuStateAsync()
        {
            soundManager.PlayBackgroundMusic("menu.mp3");
            await HandleMenuAsync();
        }

        private async Task HandleRunningStateAsync()
        {
            if (gameField != null)
                await RunGameLoop(gameField);
            soundManager.PlayBackgroundMusic("menu.mp3");
            currentState = GameState.MainMenu;
        }

        private async Task HandlePausedStateAsync()
        {
            await Task.Yield();
        }

        private async Task HandleGameOverStateAsync()
        {
            soundManager.PlayBackgroundMusic("menu.mp3");
            currentState = GameState.MainMenu;
            await Task.Yield();
        }
        
        private async Task HandleStartGameAsync()
        {
            if (gameField != null)
            {
                lastScore = gameField.Player.Score;
                lastTimePlayed = ElapsedTimeSeconds;
                lastDate = DateTime.Now;
                resultManager.SaveLastScore(lastScore, lastTimePlayed, lastDate);
            }
            SaveResultsWithCurrentScore();
            levelBuilder = new LevelBuilder();
            gameField = StartNewGame(true);
            soundManager.PlayBackgroundMusic("background.mp3");
            currentState = GameState.Running;
            await Task.Yield();
        }

        private async Task HandleShowResultsAsync()
        {
            resultManager.ShowResults();
            await Task.Yield();
        }

        private async Task HandleSettingsAsync()
        {
            await menuManager.ShowSettingsMenuAsync(GenerationSettings);
            SettingsManager.Save(GenerationSettings);
            await Task.Yield();
        }

        private async Task HandleExitAsync()
        {
            SaveResults();
            soundManager.StopMusic();
            currentState = GameState.Exit;
        #if WINDOWS
            System.Windows.Forms.Application.Exit();
        #else
            Environment.Exit(0);
        #endif
            await Task.Yield();
        }

        private async Task HandleManualLevelAsync()
        {
            levelBuilder = new LevelBuilder();
            gameField = LoadLevel("level1.txt");
            soundManager.PlayBackgroundMusic("background.mp3");
            currentState = GameState.Running;
            await Task.Yield();
        }

        private async Task HandleLoadGameAsync()
        {
            gameField = LoadSavedGame();
            if (gameField != null)
            {
                soundManager.PlayBackgroundMusic("background.mp3");
                currentState = GameState.Running;
            }
            else
            {
                renderer.WriteLine("No saved game found.");
                await Task.Delay(1000);
            }
        }

        private async Task HandleMenuAsync()
        {
            MenuChoice choice = await menuManager.ShowMainMenuAsync(LastScoreDisplay);
            if (menuChoiceHandlers.TryGetValue(choice, out var handler))
            {
                await handler();
            }
        }
    }
}
