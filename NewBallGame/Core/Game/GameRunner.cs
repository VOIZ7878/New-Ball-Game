using System;
using System.Threading.Tasks;
using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    public class GameRunner
    {
        private GameField? gameField;
        private GameState currentState = GameState.MainMenu;
        private GameManager? gameManager;
        private readonly IRenderer renderer;
        private readonly IMenuManager menuManager;
        private readonly StateManager gameStateManager;
        private readonly ResultManager resultManager;
        private readonly ISoundManager soundManager;
        private readonly IInputManager inputManager;

        public GameRunner(IRenderer renderer, IInputManager inputManager, ISoundManager soundManager, IMenuManager menuManager)
        {
            this.renderer = renderer;
            this.inputManager = inputManager;
            this.soundManager = soundManager;
            this.menuManager = menuManager;

            gameStateManager = new StateManager("GameState.json", renderer, soundManager, inputManager);
            resultManager = new ResultManager(renderer, inputManager);
        }

        public async Task Run()
        {
            currentState = GameState.MainMenu;

            while (currentState != GameState.Exit)
            {
                switch (currentState)
                {
                    case GameState.MainMenu:
                        MenuChoice choice = await menuManager.ShowMainMenuAsync();

                        switch (choice)
                        {
                            case MenuChoice.StartGame:
                                if (gameField != null && gameManager != null)
                                {
                                    gameManager.SaveResults();
                                }
                                var levelBuilder = new LevelBuilder();
                                gameManager = new GameManager(resultManager, renderer, levelBuilder);
                                gameField = gameManager.StartNewGame(true);
                                currentState = GameState.Running;
                                break;

                            case MenuChoice.ShowResults:
                                resultManager.ShowResults();
                                break;

                            case MenuChoice.Settings:
                                break;

                            case MenuChoice.Exit:
                                if (gameField != null && gameManager != null)
                                {
                                    gameManager.SaveResults();
                                }
                                break;

                            case MenuChoice.TestLevel:
                                gameField = new GameField(10, 10);
                                var testLevelBuilder = new LevelBuilder();
                                gameManager = new GameManager(
                                    resultManager, renderer, testLevelBuilder);
                                gameField = gameManager.LoadLevel("level1.txt");
                                currentState = GameState.Running;
                                break;

                            case MenuChoice.LoadGame:
                                gameField = gameStateManager.LoadGameState();
                                if (gameField != null)
                                {
                                    var loadedLevelBuilder = new LevelBuilder();
                                    gameManager = new GameManager(resultManager, renderer, loadedLevelBuilder);
                                    gameManager.SetGameField(gameField);
                                    currentState = GameState.Running;
                                }
                                else
                                {
                                    renderer.WriteLine("No saved game found.");
                                    await Task.Delay(1000);
                                }
                                break;
                        }
                        break;

                    case GameState.Running:
                        if (gameField != null)
                            await RunGameLoop(gameField);

                        currentState = GameState.MainMenu;
                        break;

                    case GameState.Paused:
                        break;

                    case GameState.GameOver:
                        currentState = GameState.MainMenu;
                        break;
                }
            }
        }

        private async Task RunGameLoop(GameField field)
        {
            soundManager.PlaySoundEffect("start.mp3");
            var controls = new ControlsManager(field, gameManager!, gameStateManager, inputManager, renderer);

            renderer.PreRender(field);

            bool running = true;
            while (running)
            {
                renderer.PostRender(field);

                var (nextState, playerMoved) = controls.HandleInput();

                if (nextState == GameState.Paused)
                {
                    gameStateManager.SaveGameState(field);
                    break;
                }
                if (nextState == GameState.Exit)
                {
                    gameStateManager.SaveGameState(field);
                    break;
                }
                if (nextState == GameState.MainMenu)
                {
                    break;
                }
                if (nextState == GameState.Running)
                {
                    field.Update(playerMoved);

                    if (playerMoved)
                        gameManager!.UpdateEnemies();

                    if (await gameManager!.CheckGameOverConditionsAsync())
                    {
                        gameManager.SaveResults();
                        break;
                    }

                    if (await gameManager.CheckLevelCompletionAsync())
                    {
                        int prevScore = field.TotalScore;
                        int prevPlayerScore = field.Player.Score;

                        gameField = gameManager.StartNewGame(false);
                        gameField.TotalScore = prevScore;
                        gameField.Player.Score = prevPlayerScore;

                        field = gameField;
                        controls = new ControlsManager(field, gameManager!, gameStateManager, inputManager, renderer);
                        renderer.PreRender(field);
                        continue;
                    }
                }
                if (nextState == GameState.Restart)
                {
                    gameManager!.SaveResultsWithCurrentScore();
                    gameField = gameManager!.StartNewGame(true);
                    field = gameField;
                    controls = new ControlsManager(field, gameManager!, gameStateManager, inputManager, renderer);
                    renderer.PreRender(field);
                    continue;
                }
                if (nextState == GameState.GameOver)
                {
                    gameManager!.SaveResults();
                    soundManager.PlaySoundEffect("lose.mp3");
                    break;
                }

                await Task.Delay(40);
            }

            soundManager.PlaySoundEffect("lose.mp3");
        }
    }
}