using BallGame.Rendering;

namespace BallGame
{
    public class GameRunner
    {
        private readonly IRenderer renderer;
        private readonly MenuManager menuManager;

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
                        RunGameLoop();
                        break;

                    case MenuChoice.ShowResults:
                        GameResultSaver.ShowSavedResults();
                        break;

                    case MenuChoice.Exit:
                        return;
                }
            }
        }

        private void RunGameLoop()
        {
            GameField field = new GameField(10, 6, renderer);
            GameManager manager = new GameManager(field, renderer);
            ControlsManager controls = new ControlsManager(field, manager);

            manager.RestartLevel(true);

            while (field.StateRun)
            {
                renderer.Render(field);
                controls.HandleInput();
                field.Update(false);
                System.Threading.Thread.Sleep(60);
            }

            manager.SaveResults();
        }
    }
}
