namespace BallGame.Utils
{
    public interface ISoundManager
    {
        void PlayBackgroundMusic(string fileName);
        void PlaySoundEffect(string effectName);
        void StopMusic();
    }
}