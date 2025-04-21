using BallGame.Rendering;

namespace BallGame
{
    public class Hint
    {
        private (int x, int y)? hintPosition;
        private char? hintDirection;

        public (int x, int y)? GetHintPosition() => hintPosition;
        public char? GetHintDirection() => hintDirection;

        public void CalculateHint(GameField gameField)
        {
            if (gameField.Ball == null) return;

            int x = gameField.Ball.X, y = gameField.Ball.Y;
            int dx = gameField.Ball.Dx, dy = gameField.Ball.Dy;

            while (true)
            {
                x += dx;
                y += dy;

                if (gameField.IsWall(x, y) || gameField[x, y] is Shield)
                {
                    var potentialPositions = new List<((int x, int y) pos, char dir)>
                    {
                        ((x - dx, y), '/'),
                        ((x, y - dy), '\\'),
                        ((x - dx, y - dy), '/')
                    };

                    foreach (var (pos, dir) in potentialPositions)
                    {
                        if (IsValidShieldPosition(gameField, pos.x, pos.y))
                        {
                            hintPosition = pos;
                            hintDirection = dir;
                            return;
                        }
                    }

                    hintPosition = null;
                    hintDirection = null;
                    return;
                }
            }
        }

        private bool IsValidShieldPosition(GameField gameField, int x, int y)
        {
            return x >= 0 && x < gameField.Width &&
                   y >= 0 && y < gameField.Height &&
                   gameField[x, y] == null;
        }

        public void ClearHint()
        {
            hintPosition = null;
            hintDirection = null;
        }
    }
}