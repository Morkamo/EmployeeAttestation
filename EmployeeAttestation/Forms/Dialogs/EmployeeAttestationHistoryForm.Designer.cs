using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class EmployeeAttestationHistoryForm
{
    private System.ComponentModel.IContainer components = null!; private TableLayoutPanel layout = null!; private Label titleLabel = null!;
    private Label employeeLabel = null!; private DataGridView historyGrid = null!; private FlowLayoutPanel footer = null!;
    private Button openButton = null!; private Button closeButton = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel(); titleLabel = new Label(); employeeLabel = new Label(); historyGrid = new DataGridView();
        footer = new FlowLayoutPanel(); openButton = new Button(); closeButton = new Button(); SuspendLayout();
        layout.ColumnCount = 1; layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(28, 24, 28, 22); layout.RowCount = 4; layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F)); titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 20F);
        titleLabel.ForeColor = AppColors.TextPrimary; titleLabel.Text = "История аттестаций"; layout.Controls.Add(titleLabel, 0, 0);
        employeeLabel.AutoSize = true; employeeLabel.Font = new Font("Segoe UI", 11F); employeeLabel.ForeColor = AppColors.TextSecondary;
        layout.Controls.Add(employeeLabel, 0, 1); historyGrid.AllowUserToAddRows = false; historyGrid.AllowUserToDeleteRows = false;
        historyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; historyGrid.Dock = DockStyle.Fill; historyGrid.MultiSelect = false;
        historyGrid.ReadOnly = true; historyGrid.RowHeadersVisible = false; historyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        historyGrid.ScrollBars = ScrollBars.Both;
        historyGrid.Columns.AddRange(new DataGridViewTextBoxColumn { HeaderText = "Дата", FillWeight = 70F, MinimumWidth = 110 },
            new DataGridViewTextBoxColumn { HeaderText = "Комиссия", FillWeight = 140F, MinimumWidth = 200 },
            new DataGridViewTextBoxColumn { HeaderText = "Общий результат", FillWeight = 90F, MinimumWidth = 140 },
            new DataGridViewTextBoxColumn { HeaderText = "Решение", FillWeight = 170F, MinimumWidth = 240 },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", FillWeight = 85F, MinimumWidth = 130 });
        historyGrid.SelectionChanged += HistoryGrid_SelectionChanged; historyGrid.CellDoubleClick += HistoryGrid_CellDoubleClick; layout.Controls.Add(historyGrid, 0, 2);
        footer.Dock = DockStyle.Fill; footer.FlowDirection = FlowDirection.RightToLeft; footer.Controls.Add(closeButton); footer.Controls.Add(openButton);
        ConfigureButton(openButton, "Открыть", 120); openButton.Click += OpenButton_Click; ConfigureButton(closeButton, "Закрыть", 120);
        closeButton.DialogResult = DialogResult.Cancel; layout.Controls.Add(footer, 0, 3); CancelButton = closeButton; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Background; ClientSize = new Size(980, 620); Controls.Add(layout); MinimumSize = new Size(640, 420);
        Name = "EmployeeAttestationHistoryForm"; StartPosition = FormStartPosition.CenterParent; Text = "История аттестаций"; ResumeLayout(false);
    }
    private static void ConfigureButton(Button b, string text, int width) { b.AutoSize = true; b.AutoSizeMode = AutoSizeMode.GrowAndShrink; b.Cursor = Cursors.Hand; b.FlatStyle = FlatStyle.Flat; b.Margin = new Padding(10, 6, 0, 4); b.MinimumSize = new Size(width, 42); b.Padding = new Padding(16, 0, 16, 0); b.Text = text; }
}
