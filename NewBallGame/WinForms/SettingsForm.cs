using System.ComponentModel;
using System.Drawing;

namespace BallGame
{
    public class SettingsForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GenerationSettings GenerationSettings { get; private set; }

        private NumericUpDown? numEnergyMin, numEnergyMax, numWallMin, numWallMax, numEnemyMin, numEnemyMax;
        private NumericUpDown? numFieldWidthMin, numFieldWidthMax, numFieldHeightMin, numFieldHeightMax;
        private Button? btnSave, btnCancel;

        private readonly Color DarkBackColor = Color.FromArgb(30, 30, 30);
        private readonly Color DarkForeColor = Color.WhiteSmoke;
        private readonly Color DarkControlBack = Color.FromArgb(45, 45, 45);
        private readonly Color AccentColor = Color.Orange;

        public SettingsForm(GenerationSettings settings)
        {
            GenerationSettings = new GenerationSettings
            {
                EnergyBallRange = new Range(settings.EnergyBallRange.Min, settings.EnergyBallRange.Max),
                WallRange = new Range(settings.WallRange.Min, settings.WallRange.Max),
                EnemyRange = new Range(settings.EnemyRange.Min, settings.EnemyRange.Max),
                FieldWidth = new Range(settings.FieldWidth.Min, settings.FieldWidth.Max),
                FieldHeight = new Range(settings.FieldHeight.Min, settings.FieldHeight.Max)
            };
            InitializeUI();
        }

        private void AddRangeRow(TableLayoutPanel layout, string label, int row, int min, int max, int valueMin, int valueMax, out NumericUpDown numMin, out NumericUpDown numMax)
        {
            var lbl = new Label { Text = label, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = DarkForeColor, BackColor = DarkBackColor };
            layout.Controls.Add(lbl, 0, row);
            numMin = new NumericUpDown { Minimum = min, Maximum = max, Value = valueMin, Dock = DockStyle.Fill, BackColor = DarkControlBack, ForeColor = DarkForeColor, BorderStyle = BorderStyle.FixedSingle };
            numMax = new NumericUpDown { Minimum = min, Maximum = max, Value = valueMax, Dock = DockStyle.Fill, BackColor = DarkControlBack, ForeColor = DarkForeColor, BorderStyle = BorderStyle.FixedSingle };
            layout.Controls.Add(numMin, 1, row);
            layout.Controls.Add(numMax, 2, row);
        }

        private void InitializeUI()
        {
            this.Text = "Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = DarkBackColor;
            this.ForeColor = DarkForeColor;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 6, ColumnCount = 3, Padding = new Padding(10), BackColor = DarkBackColor, ForeColor = DarkForeColor };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            layout.Controls.Add(new Label { Text = "Type", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, ForeColor = AccentColor, BackColor = DarkBackColor }, 0, 0);
            layout.Controls.Add(new Label { Text = "Min", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, ForeColor = AccentColor, BackColor = DarkBackColor }, 1, 0);
            layout.Controls.Add(new Label { Text = "Max", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, ForeColor = AccentColor, BackColor = DarkBackColor }, 2, 0);

            AddRangeRow(layout, "Energyballs", 1, 0, 99, GenerationSettings.EnergyBallRange.Min, GenerationSettings.EnergyBallRange.Max, out numEnergyMin, out numEnergyMax);
            AddRangeRow(layout, "Walls", 2, 0, 99, GenerationSettings.WallRange.Min, GenerationSettings.WallRange.Max, out numWallMin, out numWallMax);
            AddRangeRow(layout, "Enemies", 3, 0, 99, GenerationSettings.EnemyRange.Min, GenerationSettings.EnemyRange.Max, out numEnemyMin, out numEnemyMax);
            AddRangeRow(layout, "Field Width", 4, 1, 99, GenerationSettings.FieldWidth.Min, GenerationSettings.FieldWidth.Max, out numFieldWidthMin, out numFieldWidthMax);
            AddRangeRow(layout, "Field Height", 5, 1, 99, GenerationSettings.FieldHeight.Min, GenerationSettings.FieldHeight.Max, out numFieldHeightMin, out numFieldHeightMax);

            btnSave = new Button { Text = "Save", Dock = DockStyle.Bottom, DialogResult = DialogResult.OK, BackColor = DarkControlBack, ForeColor = AccentColor, FlatStyle = FlatStyle.Flat };
            btnCancel = new Button { Text = "Cancel", Dock = DockStyle.Bottom, DialogResult = DialogResult.Cancel, BackColor = DarkControlBack, ForeColor = DarkForeColor, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) => SaveSettings();

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40, BackColor = DarkBackColor };
            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(btnCancel);

            this.Controls.Add(layout);
            this.Controls.Add(btnPanel);
        }

        private void SaveSettings()
        {
            if (numEnergyMin != null && numEnergyMax != null && numWallMin != null && numWallMax != null && numEnemyMin != null && numEnemyMax != null && numFieldWidthMin != null && numFieldWidthMax != null && numFieldHeightMin != null && numFieldHeightMax != null)
            {
                if (numEnergyMin.Value > numEnergyMax.Value || numWallMin.Value > numWallMax.Value || numEnemyMin.Value > numEnemyMax.Value || numFieldWidthMin.Value > numFieldWidthMax.Value || numFieldHeightMin.Value > numFieldHeightMax.Value)
                {
                    MessageBox.Show("Min value cannot be greater than Max value!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                GenerationSettings.EnergyBallRange = new Range((int)numEnergyMin.Value, (int)numEnergyMax.Value);
                GenerationSettings.WallRange = new Range((int)numWallMin.Value, (int)numWallMax.Value);
                GenerationSettings.EnemyRange = new Range((int)numEnemyMin.Value, (int)numEnemyMax.Value);
                GenerationSettings.FieldWidth = new Range((int)numFieldWidthMin.Value, (int)numFieldWidthMax.Value);
                GenerationSettings.FieldHeight = new Range((int)numFieldHeightMin.Value, (int)numFieldHeightMax.Value);
            }
        }
    }
}
