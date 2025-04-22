using BallGame.Rendering;

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

        public override bool IsMoveable() => false;
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
                if (shieldDir == '/')
                {
                    int temp = Dx;
                    Dx = -Dy;
                    Dy = -temp;
                }
                else if (shieldDir == '\\')
                {
                    int temp = Dx;
                    Dx = Dy;
                    Dy = temp;
                }

                X = newX + Dx;
                Y = newY + Dy;
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