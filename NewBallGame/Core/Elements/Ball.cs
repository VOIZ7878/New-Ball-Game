namespace BallGame
{
    public class Ball : GameElement
    {
        public int Dx = -1, Dy = 0;

        public Ball(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool IsOpenToMove() => false;

        public void Move(GameField field)
        {
            int newX = X + Dx;
            int newY = Y + Dy;

            if (field.IsWall(newX, newY))
            {
                if (field.IsWall(newX, Y)) Dx *= -1;
                if (field.IsWall(X, newY)) Dy *= -1;

                return;
            }

            if (field.IsShield(newX, newY, out char shieldDir))
            {
                int oldDx = Dx;
                int oldDy = Dy;

                if (shieldDir == '/' || shieldDir == '\\')
                {
                    GameField.Reflect(shieldDir, ref Dx, ref Dy);
                }

                int reflectedX = newX + Dx;
                int reflectedY = newY + Dy;

                if (field.IsWall(reflectedX, reflectedY))
                {
                    Dx = -oldDx;
                    Dy = -oldDy;
                    X = X + Dx;
                    Y = Y + Dy;
                    return;
                }

                X = reflectedX;
                Y = reflectedY;

                if (field.IsEnergyBall(X, Y))
                {
                    field.CollectEnergyBall(X, Y);
                }
                return;
            }

            if (field.IsEnergyBall(newX, newY))
            {
                field.CollectEnergyBall(newX, newY);
            }

            X = newX;
            Y = newY;
        }

    }
}