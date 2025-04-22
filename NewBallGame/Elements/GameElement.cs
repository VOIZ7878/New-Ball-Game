using BallGame.Rendering;

namespace BallGame
{
    public abstract class GameElement
    {
        public int X { get; set; }
        public int Y { get; set; }

        public virtual void Move(GameField field, ConsoleKey? key = null)
        {}
        
        public abstract bool IsMoveable();
    }
}