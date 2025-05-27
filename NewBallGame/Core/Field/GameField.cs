using BallGame.Rendering;
using BallGame.Utils;
using BallGame.Input;

namespace BallGame
{
    public partial class GameField
    {
        private readonly int width, height;
        private readonly GameElement?[,] grid;
        public int Width => width;
        public int Height => height;
        private readonly ISoundManager soundManager;
        public IInputManager InputManager { get; }

        public GameField(int w, int h, IInputManager inputManager, ISoundManager soundManager, bool initialize = true)
        {
            width = w;
            height = h;
            grid = new GameElement?[w, h];

            this.InputManager = inputManager;
            this.soundManager = soundManager;

            if (initialize)
            {
                var initializer = new LevelBuilder(this);
                initializer.InitializeField();
            }

            Player = new Player(1, 1);
        }

        public GameElement? this[int x, int y]
        {
            get => grid[x, y];
            set => grid[x, y] = value;
        }
        public GameElement?[,] Grid => grid;

        public void Update(bool playerMoved)
        {
            if (Ball == null) return;
            Ball.Move(this);
        }
    }
}
