using BallGame.Rendering;
using BallGame.Input;

namespace BallGame
{
    public class ControlsManager
    {
        private readonly GameField gameField;
        private readonly StateManager gameStateManager;
        private readonly IInputManager inputManager;
        private readonly IRenderer renderer;

        public ControlsManager(GameField field, StateManager gameStateManager, IInputManager inputManager, IRenderer renderer)
        {
            gameField = field;
            this.gameStateManager = gameStateManager;
            this.inputManager = inputManager;
            this.renderer = renderer;
        }

        public (GameState, bool) HandleInput()
        {
            if (!inputManager.KeyAvailable)
                return (GameState.Running, false);

            ConsoleKey key = inputManager.ReadKey(true);

            if (KeyMap.ConsoleKeyToInGameAction.TryGetValue(key, out var action))
            {
                switch (action)
                {
                    case KeyMap.InGameAction.ExitToMenu:
                        renderer.Clear();
                        renderer.Pause(50);
                        gameStateManager.SaveGameState(gameField);
                        return (GameState.MainMenu, false);
                    case KeyMap.InGameAction.Restart:
                        return (GameState.Restart, false);
                    case KeyMap.InGameAction.Hint:
                        gameField.Hint.CalculateHint(gameField);
                        return (GameState.Running, false);
                }
            }

            bool moved = gameField.Player.Move(gameField, key);
            return (GameState.Running, moved);
        }
    }
}