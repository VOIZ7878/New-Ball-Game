namespace BallGame
{
    public interface IMenuManager
    {
        Task<MenuChoice> ShowMainMenuAsync();
    }
}