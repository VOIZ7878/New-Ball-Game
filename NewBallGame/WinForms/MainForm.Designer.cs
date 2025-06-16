namespace BallGame;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Form1";
    }
    private FlowLayoutPanel buttonPanel;
    private Label lastScoreLabel;
    private void InitializeUI()
    {
        this.Text = "Ball Game";
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MinimumSize = new Size(800, 450);
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
            RowCount = 1,
            BackColor = Color.Black
        };
        menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        menuLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = false,
            WrapContents = false,
            Padding = new Padding(10, 30, 0, 0),
            BackColor = Color.Black
        };

        var rightPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Black
        };
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // lastScoreLabel
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // consoleBox
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        lastScoreLabel = new Label
        {
            Name = "lastScoreLabel",
            Text = "Last Score: 0",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Consolas", 10, FontStyle.Bold),
            ForeColor = Color.Orange,
            BackColor = Color.Black,
            Padding = new Padding(10, 0, 0, 0),
            Height = 40
        };
        consoleBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            ForeColor = Color.Gray,
            BackColor = Color.Black,
            Font = new Font("Consolas", 10, FontStyle.Bold),
            Margin = new Padding(10, 0, 10, 10),
            BorderStyle = BorderStyle.FixedSingle,
            WordWrap = true,
            ScrollBars = RichTextBoxScrollBars.Vertical
        };
        rightPanel.Controls.Add(lastScoreLabel, 0, 0);
        rightPanel.Controls.Add(consoleBox, 0, 1);

        menuLayout.Controls.Add(buttonPanel, 0, 0);
        menuLayout.Controls.Add(rightPanel, 1, 0);
        menuPanel.Controls.Add(menuLayout);
        this.Controls.Add(menuPanel);

        AddMenuButtons();
    }

    #endregion
}
