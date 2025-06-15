using BallGame.Rendering;
using BallGame.Input;
using BallGame.Audio;

namespace BallGame
{
    public partial class GameManager
    {
        private GameField? gameField;
        private GameState currentState = GameState.MainMenu;
        private readonly IRenderer renderer;
        private readonly IMenuManager menuManager;
        private readonly StateManager gameStateManager;
        private readonly ResultManager resultManager;
        private readonly ISoundManager soundManager;
        private readonly IInputManager inputManager;
        private LevelBuilder levelBuilder;

        private const string GameOverMessage = "Game Over! Final Total Score: {0}, Time: {1:F2} seconds";
        private const string GameOverEnemyMessage = "Game Over! You have been caught by the enemy. Total Score: {0}, Total Time Played: {1:F2} seconds";
        private const string GameOverNoBallsMessage = "Game Over! No reachable energy balls. Total Score: {0}, Total Time Played: {1:F2} seconds";
        private const string LevelWinMessage = "You win! Level Score: {0}, Total Score: {1}, Time: {2:F2} seconds";

        private int lastScore = 0;
        private double lastTimePlayed = 0;
        private DateTime lastDate = DateTime.MinValue;

        public double ElapsedTimeSeconds => (DateTime.Now - gameField!.StartTime).TotalSeconds;

        private readonly Dictionary<GameState, Func<Task>> stateHandlers;
        private readonly Dictionary<MenuChoice, Func<Task>> menuChoiceHandlers;
        private readonly Dictionary<GameState, Func<GameField, ControlsManager, bool, DateTime, Task<(bool running, bool breakLoop)>>> gameLoopStateHandlers;

        public GameManager(IRenderer renderer, IInputManager inputManager, ISoundManager soundManager, IMenuManager menuManager)
        {
            this.renderer = renderer;
            this.inputManager = inputManager;
            this.soundManager = soundManager;
            this.menuManager = menuManager;
            gameStateManager = new StateManager("GameState.json", renderer);
            resultManager = new ResultManager(renderer, inputManager);
            levelBuilder = new LevelBuilder();
            (lastScore, lastTimePlayed, lastDate) = resultManager.LoadLastScore();

            stateHandlers = new Dictionary<GameState, Func<Task>>
            {
                { GameState.MainMenu, HandleMainMenuStateAsync },
                { GameState.Running, HandleRunningStateAsync },
                { GameState.Paused, HandlePausedStateAsync },
                { GameState.GameOver, HandleGameOverStateAsync }
            };

            menuChoiceHandlers = new Dictionary<MenuChoice, Func<Task>>
            {
                { MenuChoice.StartGame, HandleStartGameAsync },
                { MenuChoice.ShowResults, HandleShowResultsAsync },
                { MenuChoice.Settings, HandleSettingsAsync },
                { MenuChoice.Exit, HandleExitAsync },
                { MenuChoice.ManualLevel, HandleManualLevelAsync },
                { MenuChoice.LoadGame, HandleLoadGameAsync }
            };

            gameLoopStateHandlers = new Dictionary<GameState, Func<GameField, ControlsManager, bool, DateTime, Task<(bool, bool)>>> {
                { GameState.Paused, async (field, controls, playerMoved, startTime) => { HandlePausedState(field, startTime); await Task.CompletedTask; return (false, true); } },
                { GameState.Exit, async (field, controls, playerMoved, startTime) => { HandleExitState(field, startTime); await Task.CompletedTask; return (false, true); } },
                { GameState.MainMenu, async (field, controls, playerMoved, startTime) => { HandleMainMenuState(field, startTime); await Task.CompletedTask; return (false, true); } },
                { GameState.Running, async (field, controls, playerMoved, startTime) => { var running = await HandleRunningState(field, playerMoved, startTime); return (running, !running); } },
                { GameState.Restart, async (field, controls, playerMoved, startTime) => { HandleRestartState(ref field, ref controls); renderer.PreRender(field); await Task.CompletedTask; return (true, false); } },
                { GameState.GameOver, async (field, controls, playerMoved, startTime) => { HandleGameOverState(field, startTime); await Task.CompletedTask; return (false, true); } },
            };
            GenerationSettings = SettingsManager.Load();
        }

        public string LastScoreDisplay => $"Score: {lastScore}, Time Played: {lastTimePlayed:F2} seconds, Date: {lastDate:dd/MM/yyyy HH:mm:ss}";
        public void SaveResults()
        {
            if (gameField != null)
                resultManager.Save(gameField.TotalScore, ElapsedTimeSeconds);
        }

        public void SaveResultsWithCurrentScore()
        {
            if (gameField != null)
            {
                gameField.TotalScore += gameField.Player.Score;
                resultManager.Save(gameField.TotalScore, ElapsedTimeSeconds);
            }
        }

        public void UpdateEnemies()
        {
            Enemy.UpdateEnemies(gameField!.Enemies, gameField.Grid, gameField);
        }

        public void SetGameField(GameField field)
        {
            gameField = field;
        }

        public GameField LoadLevel(string fileName)
        {
            var lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "assets", "Levels", fileName));
            int height = lines.Length;
            int width = lines[0].Length;
            gameField = levelBuilder.CreateGameField(false);
            ILevelLoader loader = new TextFileLevelLoader();
            loader.Load(gameField, fileName);
            return gameField;
        }

        public GameField StartNewGame(bool resetScore)
        {
            gameField = levelBuilder.CreateGameField(true);
            levelBuilder.SetGenerationSettings(GenerationSettings);
            levelBuilder.InitializeField(gameField);
            if (resetScore)
            {
                gameField.TotalScore = 0;
                gameField.Player.ResetScore();
            }
            gameField.StartTime = DateTime.Now;
            renderer.PreRender(gameField);
            return gameField;
        }
        
        public GameField? LoadSavedGame()
        {
            var loadedField = gameStateManager.LoadGameState();
            if (loadedField != null)
            {
                SetGameField(loadedField);
                return loadedField;
            }
            return null;
        }

        public async Task Run()
        {
            currentState = GameState.MainMenu;
            soundManager.PlayBackgroundMusic("menu.mp3");
            while (currentState != GameState.Exit)
            {
                if (stateHandlers.TryGetValue(currentState, out var handler))
                {
                    await handler();
                }
                else
                {
                    await Task.Yield();
                }
            }
        }

        public GenerationSettings GenerationSettings { get; set; } = SettingsManager.Load();
    }
}