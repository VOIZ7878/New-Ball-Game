using BallGame.Rendering;

namespace BallGame
{
    public class GameRunner
    {
        private readonly IRenderer renderer;
        private readonly MenuManager menuManager;
        private GameField? gameField;

        public GameRunner()
        {
            renderer = new ConsoleRenderer();
            menuManager = new MenuManager(renderer);
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
                        return;
                }
            }
        }

        private void StartNewGame()
        {
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
                System.Threading.Thread.Sleep(60);
            }

            manager.SaveResults();
        }

        private void LoadTestLevel(string fileName)
        {
            gameField = CreateEmptyGameField(10, 10);

            ILevelLoader loader = new TextFileLevelLoader();
            loader.Load(gameField, fileName);
            
            RunGameLoop(gameField, restart: false);
        }
    }
}