namespace BallGame
{
    class Enemy : GameElement
    {
        public int X, Y;
        private Random rnd = new Random();

        public Enemy(int x, int y)
        {
            Type = CellType.Enemy;
            X = x;
            Y = y;
        }

        public void Move(GameField field)
        {
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };
            int dir = rnd.Next(4);
            int newX = X + dx[dir];
            int newY = Y + dy[dir];

            if (!field.IsWall(newX, newY) && !field.IsEnemy(newX, newY))
            {
                X = newX;
                Y = newY;
            }
        }
    }
}