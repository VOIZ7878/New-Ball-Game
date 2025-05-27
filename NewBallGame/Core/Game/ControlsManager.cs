using System;
using System.IO;
using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    public class ControlsManager
    {
        private readonly GameField gameField;
        private readonly GameManager gameManager;
        private readonly StateManager gameStateManager;
        private readonly IInputManager inputManager;
        private readonly IRenderer renderer;

        public ControlsManager(GameField field, GameManager manager, StateManager gameStateManager, IInputManager inputManager, IRenderer renderer)
        {
            this.gameField = field;
            this.gameManager = manager;
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
                        renderer.WriteLine($"Exiting game... Final Total Score: {gameField.TotalScore}");
                        renderer.Pause(1000);
                        gameStateManager.SaveGameState(gameField);
                        return (GameState.MainMenu, false);
                    case KeyMap.InGameAction.Restart:
                        gameManager.RestartLevel(true);
                        return (GameState.Running, false);
                    case KeyMap.InGameAction.ShowResults:
                        gameManager.ShowGameResults();
                        return (GameState.Running, false);
                    case KeyMap.InGameAction.Hint:
                        gameField.Hint.CalculateHint(gameField);
                        return (GameState.Running, false);
                    case KeyMap.InGameAction.Save:
                        renderer.WriteLine("Saving game...");
                        gameStateManager.SaveGameState(gameField);
                        renderer.WriteLine("Game saved successfully!");
                        renderer.Pause(1000);
                        return (GameState.Running, false);
                }
            }

            bool moved = gameField.Player.Move(gameField, key);
            return (GameState.Running, moved);
        }
    }
}