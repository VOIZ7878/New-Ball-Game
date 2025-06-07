namespace BallGame
{
    public class TextFileLevelLoader : ILevelLoader
    {
        public void Load(GameField gameField, string fileName)
        {
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Levels", fileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Level file not found at: {fullPath}");

            var lines = File.ReadAllLines(fullPath);
            if (lines.Length == 0)
                throw new Exception("Level file is empty.");
            if (lines.Any(l => l.Length != lines[0].Length))
                throw new Exception("Level file has inconsistent row lengths.");

            var grid = lines.Select(l => l.ToCharArray()).ToArray();
            gameField.Enemies.Clear();
            gameField.EnergyBallList.Clear();
            Level.LevelElementPlacer.LoadSymbols(gameField, grid);
            gameField.EnergyBall = gameField.EnergyBallList.FirstOrDefault().Item1;
            gameField.EnergyBallCount = gameField.EnergyBallList.Count;
        }
    }
}