namespace BallGame
{
    public partial class MainForm
    {
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
                ("1. New Game", MenuChoice.StartGame),
                ("2. Continue", MenuChoice.LoadGame),
                ("3. Show Results", MenuChoice.ShowResults),
                ("4. Settings", MenuChoice.Settings),
                ("5. Exit", MenuChoice.Exit),
                ("6. Manual Level", MenuChoice.ManualLevel)
            };

            foreach (var item in menuItems)
            {
                var btn = new Button
                {
                    Text = item.Text,
                    Size = new Size(120, 30),
                    Tag = item.Choice,
                    Margin = new Padding(5, 10, 5, 10),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
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

                if (buttonPanel != null)
                    buttonPanel.Controls.Add(btn);
            }
        }
    }
}