namespace BallGame
{
    public class Player : GameElement
    {
        public int Score { get; set; }
        public override bool IsOpenToMove() => true;

        public Player(int x, int y)
        {
            X = x;
            Y = y;
            Score = 100;
        }

        public bool Move(GameField field, ConsoleKey key)
        {
            if (BallGame.Input.KeyMap.ShieldKeys.TryGetValue(key, out char shieldChar))
            {
                if (field.PlaceShield(X, Y, shieldChar)) {
                    Score -= 10;
                }
                return false;
            }

            if (BallGame.Input.KeyMap.MovementKeys.TryGetValue(key, out var delta))
            {
                int newX = X + delta.dx;
                int newY = Y + delta.dy;
                if (field.IsOpenToMove(newX, newY))
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
            Score = 100;
        }
    }
}