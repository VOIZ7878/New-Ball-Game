using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BallGame.Rendering;
using BallGame.Input;
using BallGame.Utils;

namespace BallGame
{
    public partial class MainForm : Form
    {
        private DoubleBufferedPanel panel1;
        private RichTextBox consoleBox;
        private GameRunner? runner;
        private WinFormsInputManager inputManager;
        private WinFormsMenuManager menuManager;
        private IRenderer renderer;
        private ISoundManager soundManager;
        private TableLayoutPanel? mainLayout;
        private FlowLayoutPanel? buttonPanel;

        public MainForm(ISoundManager soundManager)
        {
            this.soundManager = soundManager;

            InitializeComponent();
            InitializeUI();

            if (panel1 == null) throw new ArgumentNullException(nameof(panel1), "panel1 was not initialized.");
            if (consoleBox == null) throw new ArgumentNullException(nameof(consoleBox), "consoleBox was not initialized.");

            renderer = new WinFormsRenderer(panel1, consoleBox);

            inputManager = new WinFormsInputManager();
            menuManager = new WinFormsMenuManager(this);

            this.KeyPreview = true;
            this.KeyDown += (s, e) => inputManager.OnKeyDown(s, e);

            this.Load += MainForm_Load;
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            runner = new GameRunner(renderer, inputManager, soundManager, menuManager);
            await runner.Run();
        }

        private void InitializeUI()
        {
            this.Text = "Ball Game";
            this.MinimumSize = new Size(800, 700);

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));

            panel1 = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle // Add a border for visual distinction
            };
            mainLayout.Controls.Add(panel1, 0, 0);
            mainLayout.SetRowSpan(panel1, 1);

            consoleBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 10),
                Margin = new Padding(10)
            };
            mainLayout.Controls.Add(consoleBox, 1, 0);

            buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(10, 0, 0, 0)
            };
            mainLayout.Controls.Add(buttonPanel, 0, 1);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);

            AddMenuButtons();
        }

        private void AddMenuButtons()
        {
            var menuItems = new (string Text, MenuChoice Choice)[]
            {
                ("1. New Game", MenuChoice.StartGame),
                ("2. Continue", MenuChoice.LoadGame),
                ("3. Show Results", MenuChoice.ShowResults),
                ("4. Settings", MenuChoice.Settings),
                ("5. Exit", MenuChoice.Exit),
                ("6. Test Level", MenuChoice.TestLevel)
            };

            foreach (var item in menuItems)
            {
                var btn = new Button
                {
                    Text = item.Text,
                    Size = new Size(120, 30),
                    Tag = item.Choice,
                    Margin = new Padding(5, 10, 5, 10)
                };

                btn.Click += (s, e) =>
                {
                    if (btn.Tag is MenuChoice choice)
                    {
                        menuManager.SelectOption(choice);
                    }
                };

                if (buttonPanel != null)
                    buttonPanel.Controls.Add(btn);
            }
        }
    }
}