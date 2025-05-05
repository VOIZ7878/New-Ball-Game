using NAudio.Wave;

namespace BallGame
{
    public static class SoundManager
    {
        private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
        private static IWavePlayer? backgroundPlayer;
        private static AudioFileReader? backgroundReader;

        public static void PlaySound(string fileName)
        {
            Task.Run(() =>
            {
                try
                {
                    string fullPath = Path.Combine(BasePath, fileName);
                    if (!File.Exists(fullPath))
                    {
                        Console.WriteLine($"Sound file not found: {fullPath}");
                        return;
                    }

                    using var audioFile = new AudioFileReader(fullPath);
                    using var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to play sound: {ex.Message}");
                }
            });
        }

        public static void PlayBackgroundMusic(string fileName)
        {
            try
            {
                string fullPath = Path.Combine(BasePath, fileName);
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"Background music file not found: {fullPath}");
                    return;
                }

                StopBackgroundMusic();
                backgroundReader = new AudioFileReader(fullPath);
                backgroundPlayer = new WaveOutEvent();
                backgroundPlayer.Init(backgroundReader);
                backgroundPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to play background music: {ex.Message}");
            }
        }

        public static void StopBackgroundMusic()
        {
            backgroundPlayer?.Stop();
            backgroundPlayer?.Dispose();
            backgroundReader?.Dispose();
            backgroundPlayer = null;
            backgroundReader = null;
        }
    }

    public static class SoundEvents
    {
        public static void HandleEnergyBallCollected(GameField field)
        {
            if (field.EnergyBallCount < field.EnergyBallList.Count)
            {
                SoundManager.PlaySound("collect.mp3");
            }
        }

        public static void HandleGameOver()
        {
            SoundManager.StopBackgroundMusic();
            SoundManager.PlaySound("lose.mp3");
        }

        public static void HandleLevelWin()
        {
            SoundManager.PlaySound("menu.mp3");
        }
    }
}