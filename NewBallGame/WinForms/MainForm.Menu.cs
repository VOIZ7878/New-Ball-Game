namespace BallGame
{
    public partial class MainForm
    {
        private const float FONT_SIZE = 14f;

        private void SetButtonActive(Button btn)
        {
            btn.BackColor = Color.FromArgb(60, 60, 60);
            btn.ForeColor = Color.Orange;
        }

        private void SetButtonInactive(Button btn)
        {
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.White;
        }

        private void AddMenuButtons()
        {
            var menuItems = new (string Text, MenuChoice Choice)[]
            {
                ("New Game", MenuChoice.StartGame),
                ("Continue", MenuChoice.LoadGame),
                ("Show Results", MenuChoice.ShowResults),
                ("Settings", MenuChoice.Settings),
                ("Manual Levels", MenuChoice.ManualLevel),
                ("Exit", MenuChoice.Exit)
            };

            buttonPanel.Controls.Clear();
            foreach (var item in menuItems)
            {
                var btn = new Button
                {
                    Text = item.Text,
                    Height = 40,
                    Width = buttonPanel.ClientSize.Width - buttonPanel.Padding.Left - buttonPanel.Padding.Right - 10,
                    AutoSize = false,
                    Tag = item.Choice,
                    Margin = new Padding(1, 5, 1, 5),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Consolas", FONT_SIZE, FontStyle.Bold),
                    FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(40, 40, 40) }
                };

                btn.MouseEnter += (s, e) => SetButtonActive(btn);
                btn.GotFocus   += (s, e) => SetButtonActive(btn);
                btn.MouseLeave += (s, e) => SetButtonInactive(btn);
                btn.LostFocus  += (s, e) => SetButtonInactive(btn);

                btn.Click += (s, e) =>
                {
                    if (btn.Tag is MenuChoice choice)
                    {
                        menuManager.SelectOption(choice);
                    }
                };

                buttonPanel.Controls.Add(btn);
            }

            buttonPanel.Layout += (s, e) =>
            {
                int totalButtonsHeight = 0;
                foreach (Control c in buttonPanel.Controls)
                    totalButtonsHeight += c.Height + c.Margin.Top + c.Margin.Bottom;

                int availableHeight = buttonPanel.ClientSize.Height;
                int topPadding = Math.Max((availableHeight - totalButtonsHeight) / 2, 0);

                buttonPanel.Padding = new Padding(
                    buttonPanel.Padding.Left,
                    topPadding,
                    buttonPanel.Padding.Right,
                    buttonPanel.Padding.Bottom
                );
            };

            buttonPanel.PerformLayout();
            buttonPanel.Width = buttonPanel.Width;
        }
    }
}