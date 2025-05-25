using BallGame.Rendering;

namespace BallGame
{
    public abstract class GameElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public abstract bool IsMoveable();
    }
}