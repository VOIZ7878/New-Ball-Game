using BallGame.Rendering;

namespace BallGame
{
    public class Hint : GameElement
    {
        private (int x, int y)? hintPosition;
        private char? hintDirection;

        public (int x, int y)? GetHintPosition() => hintPosition;
        public char? GetHintDirection() => hintDirection;
        public override bool IsMoveable() => true;

        public void CalculateHint(GameField gameField)
        {
            if (gameField.Ball == null) return;

            int x = gameField.Ball.X, y = gameField.Ball.Y;
            int dx = gameField.Ball.Dx, dy = gameField.Ball.Dy;

            var bestHint = (position: (x: -1, y: -1), direction: (char?)null, priority: int.MinValue);

            while (true)
            {
                x += dx;
                y += dy;

                if (gameField.IsWall(x, y) || gameField[x, y] is Shield)
                {
                    var potentialPositions = new List<((int x, int y) pos, char dir, int priority)>
                    {
                        ((x - dx, y), '/', CalculatePriority(gameField, x - dx, y)),
                        ((x, y - dy), '\\', CalculatePriority(gameField, x, y - dy)),
                        ((x - dx, y - dy), '/', CalculatePriority(gameField, x - dx, y - dy))
                    };

                    foreach (var (pos, dir, priority) in potentialPositions)
                    {
                        if (IsValidShieldPosition(gameField, pos.x, pos.y) && priority > bestHint.priority)
                        {
                            bestHint = (pos, dir, priority);
                        }
                    }

                    break;
                }
            }

            if (bestHint.priority > int.MinValue)
            {
                hintPosition = bestHint.position;
                hintDirection = bestHint.direction;
            }
            else
            {
                hintPosition = null;
                hintDirection = null;
            }
        }

        private int CalculatePriority(GameField gameField, int x, int y)
        {
            int distanceToBall = Math.Abs(gameField.Ball.X - x) + Math.Abs(gameField.Ball.Y - y);
            return gameField[x, y] == null ? 100 - distanceToBall : int.MinValue;
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