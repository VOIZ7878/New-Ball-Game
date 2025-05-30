namespace BallGame
{
    public class WinFormsMenuManager : IMenuManager
    {
        private readonly Form form;
        private TaskCompletionSource<MenuChoice>? menuTcs;
        private Label? lastScoreLabel;

        public WinFormsMenuManager(Form form)
        {
            this.form = form;
            lastScoreLabel = form.Controls.Find("lastScoreLabel", true).FirstOrDefault() as Label;
            if (lastScoreLabel == null)
            {
                lastScoreLabel = new Label
                {
                    Name = "lastScoreLabel",
                    Text = "Last Score: 0",
                    Dock = System.Windows.Forms.DockStyle.Top,
                    Font = new System.Drawing.Font("Consolas", 14, System.Drawing.FontStyle.Bold),
                    ForeColor = System.Drawing.Color.Orange,
                    BackColor = System.Drawing.Color.Black,
                    Height = 32,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };
                form.Controls.Add(lastScoreLabel);
                lastScoreLabel.BringToFront();
            }
        }

        public Task<MenuChoice> ShowMainMenuAsync(string lastScoreDisplay = "Last Score: 0")
        {
            if (form is MainForm mainForm)
                mainForm.ShowMenuPanel();
            if (lastScoreLabel != null)
            {
                if (lastScoreLabel.InvokeRequired)
                    lastScoreLabel.Invoke(() => lastScoreLabel.Text = lastScoreDisplay);
                else
                    lastScoreLabel.Text = lastScoreDisplay;
            }
            menuTcs = new TaskCompletionSource<MenuChoice>();
            return menuTcs.Task;
        }

        public void SelectOption(MenuChoice choice)
        {
            if (form is MainForm mainForm && ((choice == MenuChoice.StartGame) || (choice == MenuChoice.LoadGame) || (choice == MenuChoice.ManualLevel)))
            {
                mainForm.ShowGamePanel();
            }
            if (menuTcs != null && !menuTcs.Task.IsCompleted)
            {
                menuTcs.SetResult(choice);
            }
        }
    }
}