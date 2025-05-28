using BallGame.Rendering;
using BallGame.Utils;
using BallGame.Input;

namespace BallGame
{
    public class StateManager
    {
        private readonly string saveFilePath;
        private readonly IRenderer renderer;
        private readonly ISoundManager soundManager;
        private readonly IInputManager inputManager;

        public StateManager(string saveFilePath, IRenderer renderer, ISoundManager soundManager, IInputManager inputManager)
        {
            this.saveFilePath = saveFilePath;
            this.renderer = renderer;
            this.soundManager = soundManager;
            this.inputManager = inputManager;
        }

        public void SaveGameState(GameField field)
        {
            try
            {
                string json = JSONLevelLoader.Serialize(field);
                File.WriteAllText(saveFilePath, json);
            }
            catch (Exception ex)
            {
                renderer.WriteLine("Failed to save game state: " + ex.Message);
            }
        }

        public GameField? LoadGameState()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    return JSONLevelLoader.Deserialize(json, renderer, soundManager, inputManager);
                }
            }
            catch (Exception ex)
            {
                renderer.WriteLine("Failed to load game state: " + ex.Message);
            }
            return null;
        }
    }
}