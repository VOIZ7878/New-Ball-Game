using BallGame.Rendering;
using BallGame.Input;
using BallGame.Audio;

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
        private Label? scoreLabel;
        private Panel? menuPanel;
        private Panel? gamePanel;

        public MainForm(ISoundManager soundManager)
        {
            this.soundManager = soundManager;

            InitializeComponent();
            InitializeUI();

            if (panel1 == null) throw new ArgumentNullException(nameof(panel1), "panel1 was not initialized.");
            if (consoleBox == null) throw new ArgumentNullException(nameof(consoleBox), "consoleBox was not initialized.");

            renderer = new WinFormsRenderer(panel1, consoleBox, scoreLabel);

            inputManager = new WinFormsInputManager();
            menuManager = new WinFormsMenuManager(this);
            menuManager.MenuChoiceSelected += OnMenuChoiceSelected;

            this.KeyPreview = true;
            this.KeyDown += (s, e) => inputManager.OnKeyDown(s, e);

            this.Load += MainForm_Load;
        }

        private void OnMenuChoiceSelected(MenuChoice choice)
        {
            if (choice == MenuChoice.StartGame || choice == MenuChoice.LoadGame || choice == MenuChoice.ManualLevel)
                ShowGamePanel();
            else
                ShowMenuPanel();
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            ShowMenuPanel();
            runner = new GameRunner(renderer, inputManager, soundManager, menuManager);
            await runner.Run();
        }

        public void ShowMenuPanel()
        {
            if (menuPanel != null) menuPanel.Visible = true;
            if (gamePanel != null) gamePanel.Visible = false;
            menuPanel?.BringToFront();
        }

        public void ShowGamePanel()
        {
            if (menuPanel != null) menuPanel.Visible = false;
            if (gamePanel != null) gamePanel.Visible = true;
            gamePanel?.BringToFront();
        }

        
    }
}