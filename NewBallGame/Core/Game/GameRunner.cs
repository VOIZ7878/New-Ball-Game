using BallGame.Rendering;
using BallGame.Input;
using BallGame.Audio;

namespace BallGame
{
    public class GameRunner
    {
        private readonly GameManager gameManager;

        public GameRunner(IRenderer renderer, IInputManager inputManager, ISoundManager soundManager, IMenuManager menuManager)
        {
            gameManager = new GameManager(renderer, inputManager, soundManager, menuManager);
        }

        public async Task Run()
        {
            await gameManager.Run();
        }
    }
}