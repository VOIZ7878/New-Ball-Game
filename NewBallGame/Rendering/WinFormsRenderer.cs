namespace BallGame.Rendering
{
    public class WinFormsRenderer : IRenderer
    {
        private readonly Panel panel;
        private readonly RichTextBox consoleBox;
        private readonly Label? scoreLabel;

        private readonly int cellSize = 20;
        private readonly Font renderFont = new Font("Courier New", 12, FontStyle.Bold);
        private int? lastScore = null;

        public WinFormsRenderer(DoubleBufferedPanel panel, RichTextBox consoleBox, Label? scoreLabel = null)
        {
            this.panel = panel;
            this.consoleBox = consoleBox;
            this.scoreLabel = scoreLabel;
            this.panel.Paint += OnPaint;
        }

        private GameField? fieldToRender;
        public GameField? FieldToRender
        {
            get => fieldToRender;
            private set => fieldToRender = value;
        }

        private void DrawElement(Graphics g, string symbol, Brush brush, int x, int y)
        {
            g.DrawString(symbol, renderFont, brush, x * cellSize, y * cellSize);
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (FieldToRender == null) return;
            var g = e.Graphics;
            var field = FieldToRender;
            var player = field.Player;
            var ball = field.Ball;
            if (player == null) return;

            var hint = field.Hint;
            var hintPos = hint.HintPosition;
            var hintDir = hint.HintDirection;
            var rayPath = hint.RayPathPoints;

            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    // 1. Player
                    if (player.X == x && player.Y == y)
                    {
                        var playerVisual = ElementVisuals.Get(player);
                        DrawElement(g, playerVisual.Symbol, playerVisual.Brush, x, y);
                        continue;
                    }
                    // 2. Ball
                    if (ball != null && ball.X == x && ball.Y == y)
                    {
                        var ballVisual = ElementVisuals.Get(ball);
                        DrawElement(g, ballVisual.Symbol, ballVisual.Brush, x, y);
                        continue;
                    }
                    // 3. Enemy
                    var element = field[x, y];
                    if (element != null)
                    {
                        var visual = ElementVisuals.Get(element);
                        DrawElement(g, visual.Symbol, visual.Brush, x, y);
                        continue;
                    }
                    // 4. Hint direction
                    if (hintPos.HasValue && hintPos.Value.x == x && hintPos.Value.y == y && hintDir.HasValue)
                    {
                        var hintVisual = ElementVisuals.Get(hint);
                        DrawElement(g, hintDir.Value.ToString(), hintVisual.Brush, x, y);
                        continue;
                    }
                    // 5. Hint ray path
                    if (rayPath.Any(p => p.x == x && p.y == y))
                    {
                        DrawElement(g, ".", Brushes.Cyan, x, y);
                        continue;
                    }
                }
            }
        }

        public void Clear()
        {
            Invoke(panel, () => panel.Refresh());
            Invoke(consoleBox, () => consoleBox.Clear());
        }

        public void WriteLine(string message)
        {
            Invoke(consoleBox, () => consoleBox.AppendText(message + "\n"));
        }

        public void Pause(int milliseconds)
        {
            if (System.Threading.SynchronizationContext.Current != null)
            {
                var t = Task.Delay(milliseconds);
                t.Wait();
            }
            else
            {
                Thread.Sleep(milliseconds);
            }
        }
        
        private void UpdateScoreLabel(GameField field)
        {
            if (scoreLabel != null && field.Player != null)
            {
                if (lastScore != field.Player.Score)
                {
                    Invoke(scoreLabel, () => scoreLabel.Text = $"Score: {field.Player.Score}");
                    lastScore = field.Player.Score;
                }
            }
        }

        public void PreRender(GameField field)
        {
            Clear();
            UpdateScoreLabel(field);
        }

        public void PostRender(GameField field)
        {
            FieldToRender = field;
            UpdateScoreLabel(field);
            Invoke(panel, () => panel.Invalidate());
        }

        private void Invoke(Control control, Action action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }
    }
}