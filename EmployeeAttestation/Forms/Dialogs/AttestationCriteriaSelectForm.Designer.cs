using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class AttestationCriteriaSelectForm
{
    private System.ComponentModel.IContainer components = null!; private TableLayoutPanel layout = null!; private Label titleLabel = null!;
    private Label hintLabel = null!; private DataGridView criteriaGrid = null!; private FlowLayoutPanel actionsPanel = null!;
    private Button selectAllButton = null!; private Button clearAllButton = null!; private FlowLayoutPanel footer = null!;
    private Button saveButton = null!; private Button cancelButton = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel(); titleLabel = new Label(); hintLabel = new Label(); criteriaGrid = new DataGridView();
        actionsPanel = new FlowLayoutPanel(); selectAllButton = new Button(); clearAllButton = new Button(); footer = new FlowLayoutPanel();
        saveButton = new Button(); cancelButton = new Button(); SuspendLayout();
        layout.ColumnCount = 1; layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(28, 24, 28, 22); layout.RowCount = 5;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 20F); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Text = "Критерии аттестации"; layout.Controls.Add(titleLabel, 0, 0);
        hintLabel.AutoSize = true; hintLabel.Font = new Font("Segoe UI", 10F); hintLabel.ForeColor = AppColors.TextSecondary;
        hintLabel.Text = "Выберите индивидуальный набор критериев для этой аттестации."; layout.Controls.Add(hintLabel, 0, 1);
        criteriaGrid.AllowUserToAddRows = false; criteriaGrid.AllowUserToDeleteRows = false; criteriaGrid.AllowUserToResizeRows = false;
        criteriaGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; criteriaGrid.Dock = DockStyle.Fill;
        criteriaGrid.RowHeadersVisible = false; criteriaGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        criteriaGrid.Columns.AddRange(new DataGridViewCheckBoxColumn { Name = "selected", HeaderText = "Выбрать", FillWeight = 45F },
            new DataGridViewTextBoxColumn { Name = "name", HeaderText = "Наименование", FillWeight = 170F, ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "category", HeaderText = "Категория", FillWeight = 90F, ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "range", HeaderText = "Баллы", FillWeight = 45F, ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "status", HeaderText = "Статус", FillWeight = 55F, ReadOnly = true });
        layout.Controls.Add(criteriaGrid, 0, 2);
        actionsPanel.Dock = DockStyle.Fill; actionsPanel.Controls.Add(selectAllButton); actionsPanel.Controls.Add(clearAllButton);
        ConfigureButton(selectAllButton, "Выбрать все", 130); selectAllButton.Click += SelectAllButton_Click;
        ConfigureButton(clearAllButton, "Снять все", 120); clearAllButton.Click += ClearAllButton_Click; layout.Controls.Add(actionsPanel, 0, 3);
        footer.Dock = DockStyle.Fill; footer.FlowDirection = FlowDirection.RightToLeft; footer.Controls.Add(cancelButton); footer.Controls.Add(saveButton);
        ConfigureButton(saveButton, "Сохранить", 130); saveButton.Click += SaveButton_Click;
        ConfigureButton(cancelButton, "Отмена", 120); cancelButton.DialogResult = DialogResult.Cancel; layout.Controls.Add(footer, 0, 4);
        AcceptButton = saveButton; CancelButton = cancelButton; AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        ClientSize = new Size(900, 650); Controls.Add(layout); MinimumSize = new Size(760, 520); Name = "AttestationCriteriaSelectForm";
        StartPosition = FormStartPosition.CenterParent; Text = "Выбор критериев"; ResumeLayout(false);
    }
    private static void ConfigureButton(Button button, string text, int width) { button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0, 4, 10, 4); button.Size = new Size(width, 42); button.Text = text; }
}
