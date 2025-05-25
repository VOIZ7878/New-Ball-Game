using BallGame.Rendering;

namespace BallGame
{
    public class BossEnemy : Enemy
    {
        public BossEnemy(int x, int y) : base(x, y) { }

        public override void Move(GameField field, ConsoleKey? key = null)
        {
            var targetX = field.Player.X;
            var targetY = field.Player.Y;

            var nextStep = Pathfinding.FindNextStep(field, X, Y, targetX, targetY);

            if (nextStep != null)
            {
                (int newX, int newY) = nextStep.Value;

                if (field.IsWall(newX, newY))
                {
                    field[newX, newY] = null;
                    X = newX;
                    Y = newY;
                }
                else if (field.IsMoveable(newX, newY) && !field.IsEnemy(newX, newY) && !field.IsEnergyBall(newX, newY))
                {
                    int oldX = X, oldY = Y;
                    X = newX;
                    Y = newY;
                }
            }
        }
    }
}