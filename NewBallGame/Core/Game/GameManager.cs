using System.Reflection.Metadata;
using BallGame.Rendering;

namespace BallGame
{
    public partial class GameManager
    {
        private GameField? gameField;
        private readonly LevelBuilder levelBuilder;
        private readonly ResultManager resultSaver;
        private readonly IRenderer renderer;
        private double ElapsedTimeSeconds => (DateTime.Now - gameField!.StartTime).TotalSeconds;

        public GameManager(ResultManager resultSaver, IRenderer renderer, LevelBuilder levelBuilder)
        {
            this.resultSaver = resultSaver;
            this.levelBuilder = levelBuilder;
            this.renderer = renderer;
        }

        public GameField LoadLevel(string fileName)
        {
            var lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "assets", "Levels", fileName));
            int height = lines.Length;
            int width = lines[0].Length;

            gameField = levelBuilder.CreateGameField(false);
            ILevelLoader loader = new TextFileLevelLoader();
            loader.Load(gameField, fileName);
            return gameField;
        }

        public GameField StartNewGame(bool resetScore)
        {
            gameField = levelBuilder.CreateGameField(true);
            levelBuilder.InitializeField(gameField);

            if (resetScore)
            {
                gameField.TotalScore = 0;
                gameField.Player.ResetScore();
            }

            gameField.StartTime = DateTime.Now;
            renderer.PreRender(gameField);
            return gameField;
        }

        public void SetGameField(GameField field)
        {
            this.gameField = field;
        }
    }
}