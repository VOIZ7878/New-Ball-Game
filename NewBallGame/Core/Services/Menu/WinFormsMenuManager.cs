namespace BallGame
{
    public class WinFormsMenuManager : IMenuManager
    {
        private readonly Form form;
        private TaskCompletionSource<MenuChoice>? menuTcs;
        private Label? lastScoreLabel;
        public event Action<MenuChoice>? MenuChoiceSelected;

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
            if (lastScoreLabel != null)
            {
                if (lastScoreLabel.InvokeRequired)
                    lastScoreLabel.Invoke(() => lastScoreLabel.Text = lastScoreDisplay);
                else
                    lastScoreLabel.Text = lastScoreDisplay;
            }
            menuTcs = new TaskCompletionSource<MenuChoice>();
            if (form is MainForm mainForm)
                mainForm.ShowMenuPanel();
            return menuTcs.Task;
        }

        public void SelectOption(MenuChoice choice)
        {
            MenuChoiceSelected?.Invoke(choice);
            if (menuTcs != null && !menuTcs.Task.IsCompleted)
            {
                menuTcs.SetResult(choice);
            }
        }

        public async Task ShowSettingsMenuAsync(GenerationSettings settings)
        {
            await Task.Yield();
            if (form is not null)
            {
                var dlg = new SettingsForm(settings);
                if (dlg.ShowDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    var updated = dlg.GenerationSettings;
                    settings.EnergyBallRange = updated.EnergyBallRange;
                    settings.WallRange = updated.WallRange;
                    settings.EnemyRange = updated.EnemyRange;
                    settings.FieldWidth = updated.FieldWidth;
                    settings.FieldHeight = updated.FieldHeight;
                }
            }
        }

        public async Task<string?> ShowLevelSelectMenuAsync()
        {
            await Task.Yield();
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Title = "Choose Level File";
                dialog.Filter = "Level files (*.txt)|*.txt";
                dialog.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Levels");
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    return System.IO.Path.GetFileName(dialog.FileName);
                }
            }
            return null;
        }
    }
}