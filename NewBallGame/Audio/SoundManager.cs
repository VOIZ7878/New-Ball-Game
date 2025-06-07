using NAudio.Wave;

namespace BallGame.Utils
{
    public class SoundManager : ISoundManager
    {
        private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Sounds");
        private static IWavePlayer? backgroundPlayer;
        private static AudioFileReader? backgroundReader;
        private string? currentBackgroundFile;

        private void PlayAudio(string fileName, bool loop, bool async, EventHandler<StoppedEventArgs>? onStopped = null)
        {
            string? fullPath = GetSoundFilePath(fileName);
            if (fullPath == null) return;

            Action playAction = () =>
            {
                try
                {
                    var audioFile = new AudioFileReader(fullPath);
                    var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    if (loop && onStopped != null)
                        outputDevice.PlaybackStopped += onStopped;
                    outputDevice.Play();

                    if (!loop)
                    {
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(10);
                        }
                        outputDevice.Dispose();
                        audioFile.Dispose();
                    }
                    else
                    {
                        backgroundPlayer = outputDevice;
                        backgroundReader = audioFile;
                        currentBackgroundFile = fileName;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to play sound: {ex.Message}");
                }
            };

            if (async && !loop)
                Task.Run(playAction);
            else
                playAction();
        }

        public void PlayBackgroundMusic(string fileName)
        {
            StopMusic();
            PlayAudio(fileName, loop: true, async: false, onStopped: BackgroundPlayer_PlaybackStopped);
        }

        public void PlaySoundEffect(string effectName)
        {
            PlayAudio(effectName, loop: false, async: true);
        }

        public void StopMusic()
        {
            if (backgroundPlayer != null)
            {
                backgroundPlayer.PlaybackStopped -= BackgroundPlayer_PlaybackStopped;
                backgroundPlayer.Stop();
                backgroundPlayer.Dispose();
                backgroundReader?.Dispose();
            }

            backgroundPlayer = null;
            backgroundReader = null;
            currentBackgroundFile = null;
        }

        private static string? GetSoundFilePath(string fileName)
        {
            string fullPath = Path.Combine(BasePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Sound file not found: {fullPath}");
                return null;
            }
            return fullPath;
        }

        private void BackgroundPlayer_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (backgroundReader != null && backgroundPlayer != null && currentBackgroundFile != null)
            {
                backgroundReader.Position = 0;
                backgroundPlayer.Play();
            }
        }
    }
}