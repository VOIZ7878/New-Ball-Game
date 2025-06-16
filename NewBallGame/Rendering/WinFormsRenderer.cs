namespace BallGame.Rendering
{
    public class WinFormsRenderer : IRenderer
    {
        private readonly Panel panel;
        private readonly RichTextBox consoleBox;
        private readonly Label? scoreLabel;

        private readonly int cellSize = 20;
        private readonly Font renderFont = new Font("Consolas", 12, FontStyle.Bold);
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

        private (string symbol, Brush brush) GetVisual(GameElement element, string? overrideSymbol = null)
        {
            if (element == null) return (" ", Brushes.Black);
            var visual = ElementVisuals.Get(element);
            string symbol = overrideSymbol ?? visual.Symbol;
            return (symbol, visual.Brush);
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
                    if (player.X == x && player.Y == y)
                    {
                        var (symbol, brush) = GetVisual(player);
                        DrawElement(g, symbol, brush, x, y);
                        continue;
                    }
                    if (ball != null && ball.X == x && ball.Y == y)
                    {
                        var (symbol, brush) = GetVisual(ball);
                        DrawElement(g, symbol, brush, x, y);
                        continue;
                    }
                    var element = field[x, y];
                    if (element is Enemy)
                    {
                        var (symbol, brush) = GetVisual(element);
                        DrawElement(g, symbol, brush, x, y);
                        continue;
                    }
                    if (field.IsEnergyBall(x, y))
                    {
                        DrawElement(g, "@", Brushes.Yellow, x, y);
                        continue;
                    }
                    if (hintPos.HasValue && hintPos.Value.x == x && hintPos.Value.y == y && hintDir.HasValue)
                    {
                        var (symbol, brush) = GetVisual(hint, hintDir.Value.ToString());
                        DrawElement(g, symbol, brush, x, y);
                        continue;
                    }
                    if (rayPath.Any(p => p.x == x && p.y == y))
                    {
                        DrawElement(g, ".", Brushes.Cyan, x, y);
                        continue;
                    }
                    if (element is GameElement ge)
                    {
                        var (symbol, brush) = GetVisual(ge);
                        DrawElement(g, symbol, brush, x, y);
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