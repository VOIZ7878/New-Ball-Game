using System.Text.Json;
using System.IO;

namespace BallGame
{
    public static class SettingsManager
    {
        private const string SettingsFile = "assets/Data/GenerationSettings.json";

        public static void Save(GenerationSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
        }

        public static GenerationSettings Load()
        {
            if (!File.Exists(SettingsFile))
                return new GenerationSettings();
            try
            {
                var json = File.ReadAllText(SettingsFile);
                return JsonSerializer.Deserialize<GenerationSettings>(json) ?? new GenerationSettings();
            }
            catch
            {
                return new GenerationSettings();
            }
        }
    }
}
