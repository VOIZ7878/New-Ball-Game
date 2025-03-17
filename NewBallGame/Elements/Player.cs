namespace BallGame
{
    class Player
    {
        public int X, Y;
        public int Score { get; private set; } = 0;
        public Player(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Move(ConsoleKey key, GameField field)
        {
            int newX = X, newY = Y;
            switch (key)
            {
                case ConsoleKey.W: newY--; break;
                case ConsoleKey.S: newY++; break;
                case ConsoleKey.A: newX--; break;
                case ConsoleKey.D: newX++; break;
                case ConsoleKey.Spacebar:
                    if (field.PlaceShield(X, Y, '/')) Score -= 1;
                    return;
                case ConsoleKey.Enter:
                    if (field.PlaceShield(X, Y, '\\')) Score -= 1;
                    return;
            }
            if (!field.IsWall(newX, newY))
            {
                X = newX;
                Y = newY;
                field.Update(true);
            }
            else
            {
                field.Update(false);
            }
        }

        public void AddScore(int points)
        {
            Score += points;
        }
    }
}