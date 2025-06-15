namespace BallGame.Audio
{
    public interface ISoundManager
    {
        void PlayBackgroundMusic(string fileName);
        void PlaySoundEffect(string effectName);
        void StopMusic();
    }
}