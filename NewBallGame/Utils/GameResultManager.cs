using System;
using System.IO;
using System.Linq;

namespace BallGame
{
    public class GameResultManager
    {
        private const string ResultsFilePath = "GameResults.txt";
        private const int MaxSavedResults = 20;

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

        public static void ShowSavedResults(string filePath = "GameResults.txt")
        {
            Console.Clear();
            if (File.Exists(filePath))
            {
                Console.WriteLine("Game Results:");
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.TakeLast(20))
                    Console.WriteLine(line);
            }
            else
            {
                Console.WriteLine("No results found.");
            }
            Console.WriteLine("\nPress any key to return to the game...");
            Console.ReadKey(true);
        }
    }
}