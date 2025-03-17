namespace BallGame
{
    class Shield : GameElement
    {
        public char Direction { get; }
        public Shield(char direction)
        {
            Type = CellType.Shield;
            Direction = direction;
        }
    }
}