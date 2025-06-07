namespace BallGame
{
    public class SmartEnemy : Enemy
    {
        public SmartEnemy(int x, int y) : base(x, y) { }

        public override void Move(GameField field, ConsoleKey? key = null)
        {
            var targetX = field.Player.X;
            var targetY = field.Player.Y;

            var nextStep = Pathfinding.FindNextStep(field, X, Y, targetX, targetY);

            if (nextStep != null)
            {
                (int newX, int newY) = nextStep.Value;

                if (field.IsOpenToMove(newX, newY) && !field.IsEnemy(newX, newY) && !field.IsEnergyBall(newX, newY))
                {
                    X = newX;
                    Y = newY;
                }
            }
        }
    }
}