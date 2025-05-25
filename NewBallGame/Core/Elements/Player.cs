namespace BallGame
{
    public class Player : GameElement
    {
        public override bool IsMoveable()
        {
            return true;
        }
        public int Score { get; set; }

        public Player(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Move(GameField field, ConsoleKey key)
        {
            int dx = 0, dy = 0;

            switch (key)
            {
                case ConsoleKey.Spacebar:
                    if (field.PlaceShield(X, Y, '/')) Score -= 1;
                    return false;

                case ConsoleKey.Enter:
                    if (field.PlaceShield(X, Y, '\\')) Score -= 1;
                    return false;

                case ConsoleKey.W: dy = -1; break;
                case ConsoleKey.S: dy = 1; break;
                case ConsoleKey.A: dx = -1; break;
                case ConsoleKey.D: dx = 1; break;
                case ConsoleKey.UpArrow: dy = -1; break;
                case ConsoleKey.DownArrow: dy = 1; break;
                case ConsoleKey.LeftArrow: dx = -1; break;
                case ConsoleKey.RightArrow: dx = 1; break;

                default:
                    return false;
            }

            int newX = X + dx;
            int newY = Y + dy;

            if (field.IsMoveable(newX, newY))
            {
                X = newX;
                Y = newY;
                return true;
            }

            return false;
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }
}