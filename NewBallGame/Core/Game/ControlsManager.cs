using System;
using System.IO;
using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    public class ControlsManager
    {
        private GameField gameField;
        private GameManager gameManager;
        private readonly StateManager gameStateManager;
        private readonly IInputManager inputManager;
        private readonly IRenderer renderer;

        public ControlsManager(GameField field, GameManager manager, StateManager gameStateManager, IInputManager inputManager, IRenderer renderer)
        {
            gameField = field;
            gameManager = manager;
            this.gameStateManager = gameStateManager;
            this.inputManager = inputManager;
            this.renderer = renderer;
        }

        public (GameState, bool) HandleInput()
        {
            if (!inputManager.KeyAvailable)
                return (GameState.Running, false);

            ConsoleKey key = inputManager.ReadKey(true);

            switch (key)
            {
                case ConsoleKey.Escape:
                    renderer.Clear();
                    renderer.WriteLine($"Exiting game... Final Total Score: {gameField.TotalScore}");
                    renderer.Pause(1000);
                    gameStateManager.SaveGameState(gameField);
                    return (GameState.MainMenu, false);

                case ConsoleKey.R:
                    gameManager.RestartLevel(true);
                    return (GameState.Running, false);

                case ConsoleKey.V:
                    gameManager.ShowGameResults();
                    return (GameState.Running, false);

                case ConsoleKey.H:
                    gameField.Hint.CalculateHint(gameField);
                    return (GameState.Running, false);

                case ConsoleKey.Q:
                    renderer.WriteLine("Saving game...");
                    gameStateManager.SaveGameState(gameField);
                    renderer.WriteLine("Game saved successfully!");
                    renderer.Pause(1000);
                    return (GameState.Running, false);

                default:
                    bool moved = gameField.Player.Move(gameField, key);
                    return (GameState.Running, moved);
            }
        }
    }
}