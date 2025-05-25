using System;
using NAudio.Wave;

namespace BallGame.Utils
{
    public class SoundManager : ISoundManager
    {
        private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Sounds");
        private static IWavePlayer? backgroundPlayer;
        private static AudioFileReader? backgroundReader;

        public void PlayBackgroundMusic(string fileName)
        {
            try
            {
                string fullPath = Path.Combine(BasePath, fileName);
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"Background music file not found: {fullPath}");
                    return;
                }

                StopMusic();
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

        public void PlaySoundEffect(string effectName)
        {
            Task.Run(() =>
            {
                try
                {
                    string fullPath = Path.Combine(BasePath, effectName);
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

        public void StopMusic()
        {
            backgroundPlayer?.Stop();
            backgroundPlayer?.Dispose();
            backgroundReader?.Dispose();
            backgroundPlayer = null;
            backgroundReader = null;
        }
    }
}