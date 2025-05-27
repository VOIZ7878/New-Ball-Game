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
            if (BallGame.Input.KeyMap.ShieldKeys.TryGetValue(key, out char shieldChar))
            {
                if (field.PlaceShield(X, Y, shieldChar)) Score -= 1;
                return false;
            }

            if (BallGame.Input.KeyMap.MovementKeys.TryGetValue(key, out var delta))
            {
                int newX = X + delta.dx;
                int newY = Y + delta.dy;
                if (field.IsMoveable(newX, newY))
                {
                    X = newX;
                    Y = newY;
                    return true;
                }
                return false;
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