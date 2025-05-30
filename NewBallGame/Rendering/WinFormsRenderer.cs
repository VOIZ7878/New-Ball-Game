using BallGame.Rendering;

namespace BallGame
{
    public class WinFormsRenderer : IRenderer
    {
        private readonly Panel panel;
        private readonly RichTextBox consoleBox;
        private readonly Label? scoreLabel;

        public WinFormsRenderer(DoubleBufferedPanel panel, RichTextBox consoleBox, Label? scoreLabel = null)
        {
            this.panel = panel;
            this.consoleBox = consoleBox;
            this.scoreLabel = scoreLabel;
            this.panel.Paint += OnPaint;
        }

        public GameField? FieldToRender { get; set; }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (FieldToRender == null) return;
            var g = e.Graphics;
            int cellSize = 20;
            var font = new Font("Courier New", 12, FontStyle.Bold);

            for (int y = 0; y < FieldToRender.Height; y++)
            {
                for (int x = 0; x < FieldToRender.Width; x++)
                {
                    var element = FieldToRender[x, y];
                    if (element is Wall)
                    {
                        var visual = BallGame.Rendering.ElementVisuals.Get(element);
                        g.DrawString(visual.Symbol, font, visual.Brush, x * cellSize, y * cellSize);
                    }
                }
            }

            var player = FieldToRender.Player;
            var playerVisual = BallGame.Rendering.ElementVisuals.Get(player);
            g.DrawString(playerVisual.Symbol, font, playerVisual.Brush, player.X * cellSize, player.Y * cellSize);

            if (FieldToRender.Ball is { X: var bx, Y: var by })
            {
                var ballVisual = BallGame.Rendering.ElementVisuals.Get(FieldToRender.Ball);
                g.DrawString(ballVisual.Symbol, font, ballVisual.Brush, bx * cellSize, by * cellSize);
            }

            for (int y = 0; y < FieldToRender.Height; y++)
            {
                for (int x = 0; x < FieldToRender.Width; x++)
                {
                    var element = FieldToRender[x, y];
                    if (element != null && !(element is Wall) && element != player && element != FieldToRender.Ball)
                    {
                        var visual = BallGame.Rendering.ElementVisuals.Get(element);
                        g.DrawString(visual.Symbol, font, visual.Brush, x * cellSize, y * cellSize);
                    }
                }
            }

            var hintPos = FieldToRender.Hint.GetHintPosition();
            var hintDir = FieldToRender.Hint.GetHintDirection();
            if (hintPos.HasValue && hintDir.HasValue)
            {
                var x = hintPos.Value.x;
                var y = hintPos.Value.y;
                var hintVisual = BallGame.Rendering.ElementVisuals.Get(FieldToRender.Hint);
                g.DrawString(hintDir.Value.ToString(), font, hintVisual.Brush, x * cellSize, y * cellSize);
            }
        }
        public void Clear()
        {
            if (panel.InvokeRequired)
                panel.Invoke(() => panel.Refresh());
            else
                panel.Refresh();

            if (consoleBox.InvokeRequired)
                consoleBox.Invoke(() => consoleBox.Clear());
            else
                consoleBox.Clear();
        }

        public void WriteLine(string message)
        {
            if (consoleBox.InvokeRequired)
                consoleBox.Invoke(() => consoleBox.AppendText(message + "\n"));
            else
                consoleBox.AppendText(message + "\n");
        }

        public void Pause(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
        
        private void UpdateScoreLabel(GameField field)
        {
            if (scoreLabel != null)
            {
                if (scoreLabel.InvokeRequired)
                    scoreLabel.Invoke(() => scoreLabel.Text = $"Score: {field.Player.Score}");
                else
                    scoreLabel.Text = $"Score: {field.Player.Score}";
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
            if (panel.InvokeRequired)
                panel.Invoke(() => panel.Invalidate());
            else
                panel.Invalidate();
        }
    }
}