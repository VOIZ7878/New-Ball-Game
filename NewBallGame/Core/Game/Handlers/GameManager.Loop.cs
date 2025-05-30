namespace BallGame
{
    public partial class GameManager
    {
        private async Task RunGameLoop(GameField field)
        {
            soundManager.PlaySoundEffect("start.mp3");
            var controls = new ControlsManager(field, gameStateManager, inputManager, renderer);

            renderer.PreRender(field);

            var startTime = DateTime.Now;
            bool running = true;
            while (running)
            {
                renderer.PostRender(field);

                var (nextState, playerMoved) = controls.HandleInput();

                if (gameLoopStateHandlers.TryGetValue(nextState, out var handler))
                {
                    var (shouldContinue, shouldBreak) = await handler(field, controls, playerMoved, startTime);
                    running = shouldContinue;
                    if (shouldBreak)
                        break;
                    if (nextState == GameState.Restart || (nextState == GameState.Running && gameField != field))
                    {
                        field = gameField!;
                        controls = new ControlsManager(field, gameStateManager, inputManager, renderer);
                        renderer.PreRender(field);
                        continue;
                    }
                }
                else
                {
                    await Task.Yield();
                }

                await Task.Delay(40);
            }

            soundManager.PlaySoundEffect("lose.mp3");
        }
    }
}
