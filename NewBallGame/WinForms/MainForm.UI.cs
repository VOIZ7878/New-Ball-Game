namespace BallGame
{
    public partial class MainForm
    {
        private FlowLayoutPanel? buttonPanel;
        private Label? lastScoreLabel;
        private void InitializeUI()
        {
            this.Text = "Ball Game";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(800, 700);
            this.BackColor = Color.Black;

            gamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            scoreLabel = new Label
            {
                Text = "Score: 0",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Consolas", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Black,
                Padding = new Padding(10, 0, 0, 0),
                Height = 40
            };
            panel1 = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
            gamePanel.Controls.Add(panel1);
            gamePanel.Controls.Add(scoreLabel);
            this.Controls.Add(gamePanel);

            menuPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = true,
                BackColor = Color.Black
            };
            var menuLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Black
            };
            menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            menuLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            menuLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(10, 10, 0, 0),
                BackColor = Color.Black
            };
            menuLayout.Controls.Add(buttonPanel, 0, 1);
            menuLayout.SetRowSpan(buttonPanel, 2);

            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Black };
            lastScoreLabel = new Label
            {
                Name = "lastScoreLabel",
                Text = "Last Score: 0",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Black,
                Padding = new Padding(10, 0, 0, 0),
                Height = 40
            };
            consoleBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ForeColor = Color.Gold,
                BackColor = Color.Black,
                Font = new Font("Consolas", 10),
                Margin = new Padding(10),
                BorderStyle = BorderStyle.None
            };
            rightPanel.Controls.Add(consoleBox);
            rightPanel.Controls.Add(lastScoreLabel);
            menuLayout.Controls.Add(rightPanel, 1, 1);
            menuLayout.SetRowSpan(rightPanel, 2);

            menuPanel.Controls.Add(menuLayout);
            this.Controls.Add(menuPanel);

            AddMenuButtons();
        }
    }
}