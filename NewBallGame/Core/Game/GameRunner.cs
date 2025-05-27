using System;
using System.Threading.Tasks;
using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    public enum GameState
    {
        MainMenu,
        Running,
        Paused,
        GameOver,
        Exit
    }

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
                                gameManager = new GameManager(
                                    new GameField(10, 10, inputManager, soundManager, initialize: false),
                                    renderer, resultManager, soundManager, inputManager, menuManager
                                );
                                gameField = gameManager.StartNewGame(10, 10);
                                currentState = GameState.Running;
                                break;

                            case MenuChoice.ShowResults:
                                resultManager.ShowSavedResults();
                                break;

                            case MenuChoice.Settings:
                                break;

                            case MenuChoice.Exit:
                                currentState = GameState.Exit;
                                break;

                            case MenuChoice.TestLevel:
                                gameManager = new GameManager(
                                    new GameField(10, 10, inputManager, soundManager, initialize: false),
                                    renderer, resultManager, soundManager, inputManager, menuManager
                                );
                                gameField = gameManager.LoadLevel("level1.txt", 10, 10);
                                currentState = GameState.Running;
                                break;

                            case MenuChoice.LoadGame:
                                gameField = gameStateManager.LoadGameState();
                                if (gameField != null)
                                {
                                    gameManager = new GameManager(
                                        gameField, renderer, resultManager, soundManager, inputManager, menuManager
                                    );
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
                    gameManager!.SaveResults();
                    gameStateManager.SaveGameState(field);
                    break;
                }
                if (nextState == GameState.Exit)
                {
                    gameManager!.SaveResults();
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
                        gameManager.RestartLevel(false);
                        renderer.PreRender(field);

                        continue;
                    }
                }

                await Task.Delay(40);
            }

            soundManager.PlaySoundEffect("lose.mp3");
        }
    }
}