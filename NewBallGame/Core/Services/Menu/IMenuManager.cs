using System.Threading.Tasks;

namespace BallGame
{
    public interface IMenuManager
    {
        Task<MenuChoice> ShowMainMenuAsync();
    }
}