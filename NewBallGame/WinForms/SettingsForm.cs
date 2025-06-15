using System.ComponentModel;

namespace BallGame
{
    public class SettingsForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GenerationSettings GenerationSettings { get; private set; }

        private NumericUpDown? numEnergyMin, numEnergyMax, numWallMin, numWallMax, numEnemyMin, numEnemyMax;
        private Button? btnSave, btnCancel;

        public SettingsForm(GenerationSettings settings)
        {
            GenerationSettings = new GenerationSettings
            {
                EnergyBallRange = new Range(settings.EnergyBallRange.Min, settings.EnergyBallRange.Max),
                WallRange = new Range(settings.WallRange.Min, settings.WallRange.Max),
                EnemyRange = new Range(settings.EnemyRange.Min, settings.EnemyRange.Max)
            };
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Generation Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 350;
            this.Height = 300;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 3, Padding = new Padding(10) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            layout.Controls.Add(new Label { Text = "Type", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter }, 0, 0);
            layout.Controls.Add(new Label { Text = "Min", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter }, 1, 0);
            layout.Controls.Add(new Label { Text = "Max", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter }, 2, 0);

            layout.Controls.Add(new Label { Text = "Energyballs", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 1);
            numEnergyMin = new NumericUpDown { Minimum = 0, Maximum = 99, Value = GenerationSettings.EnergyBallRange.Min, Dock = DockStyle.Fill };
            numEnergyMax = new NumericUpDown { Minimum = 0, Maximum = 99, Value = GenerationSettings.EnergyBallRange.Max, Dock = DockStyle.Fill };
            layout.Controls.Add(numEnergyMin, 1, 1);
            layout.Controls.Add(numEnergyMax, 2, 1);

            layout.Controls.Add(new Label { Text = "Walls", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 2);
            numWallMin = new NumericUpDown { Minimum = 0, Maximum = 99, Value = GenerationSettings.WallRange.Min, Dock = DockStyle.Fill };
            numWallMax = new NumericUpDown { Minimum = 0, Maximum = 99, Value = GenerationSettings.WallRange.Max, Dock = DockStyle.Fill };
            layout.Controls.Add(numWallMin, 1, 2);
            layout.Controls.Add(numWallMax, 2, 2);

            layout.Controls.Add(new Label { Text = "Enemies", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 3);
            numEnemyMin = new NumericUpDown { Minimum = 0, Maximum = 99, Value = GenerationSettings.EnemyRange.Min, Dock = DockStyle.Fill };
            numEnemyMax = new NumericUpDown { Minimum = 0, Maximum = 99, Value = GenerationSettings.EnemyRange.Max, Dock = DockStyle.Fill };
            layout.Controls.Add(numEnemyMin, 1, 3);
            layout.Controls.Add(numEnemyMax, 2, 3);

            btnSave = new Button { Text = "Save", Dock = DockStyle.Bottom, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Dock = DockStyle.Bottom, DialogResult = DialogResult.Cancel };
            btnSave.Click += (s, e) => SaveSettings();

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40 };
            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(btnCancel);

            this.Controls.Add(layout);
            this.Controls.Add(btnPanel);
        }

        private void SaveSettings()
        {
            if (numEnergyMin != null && numEnergyMax != null && numWallMin != null && numWallMax != null && numEnemyMin != null && numEnemyMax != null)
            {
                if (numEnergyMin.Value > numEnergyMax.Value || numWallMin.Value > numWallMax.Value || numEnemyMin.Value > numEnemyMax.Value)
                {
                    MessageBox.Show("Min value cannot be greater than Max value!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                GenerationSettings.EnergyBallRange = new Range((int)numEnergyMin.Value, (int)numEnergyMax.Value);
                GenerationSettings.WallRange = new Range((int)numWallMin.Value, (int)numWallMax.Value);
                GenerationSettings.EnemyRange = new Range((int)numEnemyMin.Value, (int)numEnemyMax.Value);
            }
        }
    }
}
