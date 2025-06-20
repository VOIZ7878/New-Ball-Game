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
                    : Array.Empty<string>();

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
                {
                    var parts = line.Split(", ");
                    if (parts.Length == 3)
                    {
                        var scorePart = parts[0];
                        var timePart = parts[1];
                        var datePart = parts[2];
                        var timeValue = timePart.Replace("Time Played:", "").Replace("seconds", "").Trim();
                        if (double.TryParse(timeValue, out double seconds))
                        {
                            double minutes = seconds / 60.0;
                            timePart = $"Time Played: {minutes:F2} min";
                        }
                        var dateValue = datePart.Replace("Date:", "").Trim();
                        if (DateTime.TryParse(dateValue, out DateTime dt))
                        {
                            datePart = $"Date: {dt:MM/dd/yyyy HH:mm}";
                        }
                        renderer.WriteLine($"{scorePart}, {timePart}, {datePart}");
                    }
                    else
                    {
                        renderer.WriteLine(line);
                    }
                }
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