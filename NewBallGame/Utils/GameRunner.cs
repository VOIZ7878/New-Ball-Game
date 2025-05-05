using BallGame.Rendering;
using System.Text.Json;

namespace BallGame
{
    public class GameRunner
    {
        private readonly IRenderer renderer;
        private readonly MenuManager menuManager;
        private GameField? gameField;
        private const string SaveFilePath = "GameState.json";

        public GameRunner()
        {
            renderer = new ConsoleRenderer();
            menuManager = new MenuManager(renderer);

            SoundManager.PlayBackgroundMusic("background.mp3");
        }

        public void Run()
        {
            while (true)
            {
                MenuChoice choice = menuManager.ShowMainMenu();

                switch (choice)
                {
                    case MenuChoice.StartGame:
                        StartNewGame();
                        break;

                    case MenuChoice.ShowResults:
                        GameResultSaver.ShowSavedResults();
                        break;

                    case MenuChoice.TestLevel:
                        LoadTestLevel("level1.txt");
                        break;

                    case MenuChoice.Exit:
                        if (gameField != null)
                        {
                            SaveGameState(gameField);
                        }
                        return;

                    case MenuChoice.LoadGame:
                        gameField = LoadGameState();
                        if (gameField != null)
                        {
                            RunGameLoop(gameField, restart: false);
                        }
                        else
                        {
                            Console.WriteLine("No saved game found.");
                            System.Threading.Thread.Sleep(1000);
                        }
                        break;
                }
            }
        }

        private void StartNewGame()
        {
            SoundManager.PlaySound("start.mp3");
            gameField = CreateEmptyGameField();
            RunGameLoop(gameField);
        }

        private GameField CreateEmptyGameField(int width, int height)
        {
            return new GameField(width, height, renderer, initialize: false);
        }

        private GameField CreateEmptyGameField()
        {
            return CreateEmptyGameField(10, 10);
        }

        private void RunGameLoop(GameField field, bool restart = true)
        {
            var manager = new GameManager(field, renderer);
            var controls = new ControlsManager(field, manager);

            if (restart)
                manager.RestartLevel(true);

            while (field.StateRun)
            {
                renderer.Render(field);
                bool playerMoved = controls.HandleInput();
                field.Update(playerMoved);

                SoundEvents.HandleEnergyBallCollected(field);

                System.Threading.Thread.Sleep(50);
            }

            manager.SaveResults();
            SoundEvents.HandleGameOver();
        }

        private void LoadTestLevel(string fileName)
        {
            gameField = CreateEmptyGameField(10, 10);

            ILevelLoader loader = new TextFileLevelLoader();
            loader.Load(gameField, fileName);
            
            RunGameLoop(gameField, restart: false);
        }

        public void SaveGameState(GameField field)
        {
            try
            {
                string json = field.Serialize();
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save game state: " + ex.Message);
            }
        }

        public GameField? LoadGameState()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    return GameField.Deserialize(json, renderer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load game state: " + ex.Message);
            }
            return null;
        }
    }
}