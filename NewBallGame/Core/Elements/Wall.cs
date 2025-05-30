namespace BallGame
{
    public class Wall : GameElement
    {
        public Wall() { }
        public override bool IsMoveable() => false;

        public static bool IsWall(GameElement? element)
        {
            return element is Wall;
        }
    }
}