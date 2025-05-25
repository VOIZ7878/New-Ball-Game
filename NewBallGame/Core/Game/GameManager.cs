using System;
using System.Threading.Tasks;
using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    public class GameManager
    {
        private GameField gameField;
        private readonly IRenderer renderer;
        private readonly IInputManager inputManager;
        private readonly ISoundManager soundManager;
        private readonly ResultManager resultSaver;
        private readonly IMenuManager menuManager;

        private const string GameOverMessage = "Game Over! Final Total Score: {0}, Total Time Played: {1:F2} seconds";
        private const string GameOverNoBallsMessage = "Game Over! No reachable energy balls. Final Total Score: {0}, Total Time Played: {1:F2} seconds";
        private const string LevelWinMessage = "You win this level! Level Score: {0}, Total Score: {1}, Time: {2:F2} seconds";

        private double ElapsedTimeSeconds => (DateTime.Now - gameField.StartTime).TotalSeconds;

        public GameManager(GameField field, IRenderer renderer, ResultManager resultSaver, ISoundManager soundManager, IInputManager inputManager, IMenuManager menuManager)
        {
            gameField = field;
            this.resultSaver = resultSaver;
            this.renderer = renderer;
            this.soundManager = soundManager;
            this.inputManager = inputManager;
            this.menuManager = menuManager;
        }

        public void SaveResults()
        {
            resultSaver.Save(gameField.TotalScore, ElapsedTimeSeconds);
        }

        public void ShowGameResults()
        {
            resultSaver.ShowSavedResults();
        }

        public void UpdateEnemies()
        {
            Enemy.UpdateEnemies(gameField.Enemies, gameField.Grid, gameField);
        }

        public async Task<bool> CheckGameOverConditionsAsync()
        {
            if (gameField.IsEnemy(gameField.Player.X, gameField.Player.Y))
            {
                await HandleGameOverAsync(GameOverMessage);
                return true;
            }

            if (!CanWin())
            {
                await HandleGameOverAsync(GameOverNoBallsMessage);
                return true;
            }

            return false;
        }

        private Task HandleGameOverAsync(string messageTemplate)
        {
            return EndGameAsync(gameField.TotalScore, ElapsedTimeSeconds, messageTemplate);
        }

        public async Task<bool> CheckLevelCompletionAsync()
        {
            if (gameField.EnergyBallCount == 0)
            {
                await HandleLevelCompletionAsync();
                return true;
            }

            return false;
        }

        private async Task HandleLevelCompletionAsync()
        {
            gameField.TotalScore += gameField.Player.Score;
            renderer.Clear();
            renderer.WriteLine(string.Format(LevelWinMessage, gameField.Player.Score, gameField.TotalScore, ElapsedTimeSeconds));
            await Task.Delay(2000);

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
            renderer.PreRender(gameField);
        }

        public async Task EndGameAsync(int totalScore, double elapsedTime, string message)
        {
            SaveResults();
            renderer.Clear();
            renderer.WriteLine(string.Format(message, totalScore, elapsedTime));
            await Task.Delay(2000);
        }

        public GameField StartNewGame(int width = 10, int height = 10)
        {
            gameField = CreateEmptyGameField(width, height);
            var levelBuilder = new LevelBuilder(gameField);
            levelBuilder.InitializeField();
            return gameField;
        }

        public GameField LoadLevel(string fileName, int width = 10, int height = 10)
        {
            gameField = CreateEmptyGameField(width, height);
            ILevelLoader loader = new TextFileLevelLoader();
            loader.Load(gameField, fileName);
            return gameField;
        }

        private GameField CreateEmptyGameField(int width, int height)
        {
            return new GameField(width, height, renderer, inputManager, soundManager, initialize: false);
        }
    }
}