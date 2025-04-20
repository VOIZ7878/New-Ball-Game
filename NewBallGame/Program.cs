namespace BallGame
{
    class Program
    {
        static void Main()
        {
            GameField field = new GameField(10, 10);
            GameManager manager = new GameManager(field);
            ControlsManager controls = new ControlsManager(field, manager);

            while (field.StateRun)
            {
                field.RenderField();
                controls.HandleInput();
                field.Update(false);
                System.Threading.Thread.Sleep(40);
            }
        }
    }
}