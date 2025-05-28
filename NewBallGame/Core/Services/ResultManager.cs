using System;
using System.IO;
using System.Linq;
using BallGame.Rendering;
using BallGame.Utils;
using BallGame.Input;

namespace BallGame
{
    public class ResultManager
    {
        private const string ResultsFilePath = "GameResults.txt";
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
    }
}