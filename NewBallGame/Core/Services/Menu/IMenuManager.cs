namespace BallGame
{
    public interface IMenuManager
    {
        Task<MenuChoice> ShowMainMenuAsync(string lastScoreDisplay);
        event Action<MenuChoice>? MenuChoiceSelected;
        Task ShowSettingsMenuAsync(GenerationSettings settings);
    }
}