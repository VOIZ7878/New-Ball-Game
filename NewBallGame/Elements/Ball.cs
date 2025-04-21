using BallGame.Rendering;

namespace BallGame
{
    public class Ball : GameElement
    {
        public int X, Y;
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
                Dx *= field.IsWall(newX, Y) ? -1 : 1;
                Dy *= field.IsWall(X, newY) ? -1 : 1;
            }
            else if (field.IsShield(newX, newY, out char shieldDir))
            {
                if (shieldDir == '/')
                {
                    int temp = Dx;
                    Dx = -Dy;
                    Dy = -temp;

                    X = newX + Dx;
                    Y = newY + Dy;
                    return;
                }
                else if (shieldDir == '\\')
                {
                    int temp = Dx;
                    Dx = Dy;
                    Dy = temp;

                    X = newX + Dx;
                    Y = newY + Dy;
                    return;
                }
            }
            else if (field.IsEnergyBall(newX, newY))
            {
                field.CollectEnergyBall(newX, newY);
            }

            X = newX;
            Y = newY;
        }
    }
}