using BallGame.Rendering;
using BallGame.Input;

namespace BallGame
{
    public class ResultManager
    {
        private const string ResultsFilePath = "assets/Data/GameResults.txt";
        private const int MaxSavedResults = 20;
        private readonly IRenderer renderer;
        private readonly IInputManager inputManager;

        public ResultManager(IRenderer renderer, IInputManager inputManager)
        {
            this.renderer = renderer;
            this.inputManager = inputManager;
        }

        public void Save(int score, double timePlayedSeconds)
        {
            try
            {
                string newEntry = $"Score: {score}, Time Played: {timePlayedSeconds:F2} seconds, Date: {DateTime.Now}";
                string[] existingEntries = File.Exists(ResultsFilePath)
                    ? File.ReadAllLines(ResultsFilePath)
                    : new string[0];

                var updatedEntries = existingEntries
                    .Append(newEntry)
                    .Select(entry =>
                    {
                        var parts = entry.Split(new[] { ", " }, StringSplitOptions.None);
                        int entryScore = int.Parse(parts[0].Split(':')[1].Trim());
                        return new { Entry = entry, Score = entryScore };
                    })
                    .OrderByDescending(e => e.Score)
                    .Take(MaxSavedResults)
                    .Select(e => e.Entry)
                    .ToArray();

                File.WriteAllLines(ResultsFilePath, updatedEntries);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save game results: " + ex.Message);
            }
        }

        public void ShowResults()
        {
            renderer.Clear();
            if (File.Exists(ResultsFilePath))
            {
                renderer.WriteLine("Game Results:");
                var lines = File.ReadAllLines(ResultsFilePath);
                foreach (var line in lines.TakeLast(20))
                    renderer.WriteLine(line);
            }
            else
            {
                renderer.WriteLine("No results found.");
            }
            inputManager.ReadKey(true);
        }

        public (int, double, DateTime) LoadLastScore()
        {
            try
            {
                const string LastScoreFile = "assets/Data/LastScore.txt";
                if (File.Exists(LastScoreFile))
                {
                    var line = File.ReadAllText(LastScoreFile);
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        int score = int.Parse(parts[0]);
                        double time = double.Parse(parts[1]);
                        DateTime date = DateTime.Parse(parts[2]);
                        return (score, time, date);
                    }
                }
            }
            catch { }
            return (0, 0, DateTime.MinValue);
        }

        public void SaveLastScore(int score, double timePlayed, DateTime date)
        {
            try
            {
                const string LastScoreFile = "assets/Data/LastScore.txt";
                File.WriteAllText(LastScoreFile, $"{score}|{timePlayed}|{date:O}");
            }
            catch { }
        }
    }
}