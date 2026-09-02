using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class AttestationsControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!; private Label titleLabel = null!; private Label subtitleLabel = null!;
    private TableLayoutPanel toolbarLayout = null!; private TextBox searchTextBox = null!; private Button createButton = null!;
    private Button openButton = null!; private Button editButton = null!; private FlowLayoutPanel filtersPanel = null!;
    private Button allFilterButton = null!; private Button draftFilterButton = null!; private Button plannedFilterButton = null!;
    private Button activeFilterButton = null!; private Button completedFilterButton = null!; private DataGridView attestationsGrid = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); subtitleLabel = new Label(); toolbarLayout = new TableLayoutPanel();
        searchTextBox = new TextBox(); createButton = new Button(); openButton = new Button(); editButton = new Button(); filtersPanel = new FlowLayoutPanel();
        allFilterButton = new Button(); draftFilterButton = new Button(); plannedFilterButton = new Button(); activeFilterButton = new Button();
        completedFilterButton = new Button(); attestationsGrid = new DataGridView(); pageLayout.SuspendLayout(); toolbarLayout.SuspendLayout();
        filtersPanel.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)attestationsGrid).BeginInit(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); pageLayout.Controls.Add(titleLabel, 0, 0);
        pageLayout.Controls.Add(subtitleLabel, 0, 1); pageLayout.Controls.Add(toolbarLayout, 0, 2); pageLayout.Controls.Add(filtersPanel, 0, 3);
        pageLayout.Controls.Add(attestationsGrid, 0, 4); pageLayout.Dock = DockStyle.Fill; pageLayout.Padding = new Padding(36); pageLayout.RowCount = 5;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 6); titleLabel.Text = "Аттестации";
        subtitleLabel.AutoSize = true; subtitleLabel.Font = new Font("Segoe UI", 11F); subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 30); subtitleLabel.Text = "Просмотр и управление аттестациями сотрудников.";
        toolbarLayout.BackColor = AppColors.Surface; toolbarLayout.ColumnCount = 7;
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F)); toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F)); toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); toolbarLayout.Controls.Add(searchTextBox, 0, 0);
        toolbarLayout.Controls.Add(createButton, 2, 0); toolbarLayout.Controls.Add(openButton, 4, 0); toolbarLayout.Controls.Add(editButton, 6, 0);
        toolbarLayout.Dock = DockStyle.Fill; toolbarLayout.Margin = new Padding(0, 0, 0, 16); toolbarLayout.Padding = new Padding(16);
        searchTextBox.BorderStyle = BorderStyle.FixedSingle; searchTextBox.Dock = DockStyle.Fill; searchTextBox.Font = new Font("Segoe UI", 11F);
        searchTextBox.Margin = new Padding(0, 8, 0, 7); searchTextBox.PlaceholderText = "Поиск по сотруднику";
        ConfigureToolbarButton(createButton, "Создать аттестацию"); ConfigureToolbarButton(openButton, "Открыть"); ConfigureToolbarButton(editButton, "Изменить");
        filtersPanel.AutoScroll = true; filtersPanel.BackColor = AppColors.Surface; filtersPanel.Controls.Add(allFilterButton);
        filtersPanel.Controls.Add(draftFilterButton); filtersPanel.Controls.Add(plannedFilterButton); filtersPanel.Controls.Add(activeFilterButton);
        filtersPanel.Controls.Add(completedFilterButton); filtersPanel.Dock = DockStyle.Fill; filtersPanel.Margin = new Padding(0, 0, 0, 16);
        filtersPanel.Padding = new Padding(12, 4, 12, 4); filtersPanel.WrapContents = false;
        ConfigureFilterButton(allFilterButton, "Все", true); ConfigureFilterButton(draftFilterButton, "Черновики", false);
        ConfigureFilterButton(plannedFilterButton, "Запланированные", false); ConfigureFilterButton(activeFilterButton, "В работе", false);
        ConfigureFilterButton(completedFilterButton, "Завершенные", false);
        attestationsGrid.AllowUserToAddRows = false; attestationsGrid.AllowUserToDeleteRows = false; attestationsGrid.AllowUserToResizeRows = false;
        attestationsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        attestationsGrid.Columns.AddRange(new DataGridViewTextBoxColumn { HeaderText = "Сотрудник", Name = "employeeColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Должность", Name = "positionColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Комиссия", Name = "commissionColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Дата", Name = "dateColumn", FillWeight = 65 },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", Name = "statusColumn", FillWeight = 75 },
            new DataGridViewTextBoxColumn { HeaderText = "Итог", Name = "resultColumn", FillWeight = 75 });
        attestationsGrid.Dock = DockStyle.Fill; attestationsGrid.MultiSelect = false; attestationsGrid.ReadOnly = true;
        attestationsGrid.RowHeadersVisible = false; attestationsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F); Name = "AttestationsControl"; Size = new Size(1340, 900);
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); toolbarLayout.ResumeLayout(false); toolbarLayout.PerformLayout();
        filtersPanel.ResumeLayout(false); filtersPanel.PerformLayout(); ((System.ComponentModel.ISupportInitialize)attestationsGrid).EndInit(); ResumeLayout(false);
    }

    private static void ConfigureToolbarButton(Button button, string text)
    { button.Cursor = Cursors.Hand; button.Dock = DockStyle.Fill; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0); button.Text = text; button.UseVisualStyleBackColor = false; }

    private static void ConfigureFilterButton(Button button, string text, bool selected)
    {
        button.AutoSize = true; button.BackColor = selected ? AppColors.ActiveBackground : AppColors.Surface; button.Cursor = Cursors.Hand;
        button.FlatStyle = FlatStyle.Flat; button.Font = new Font("Segoe UI", 9.5F); button.ForeColor = selected ? AppColors.Primary : AppColors.TextSecondary;
        button.Margin = new Padding(0, 0, 8, 0); button.MinimumSize = new Size(0, 40); button.Padding = new Padding(14, 0, 14, 0);
        button.Text = text; button.UseVisualStyleBackColor = false; button.FlatAppearance.BorderColor = selected ? AppColors.Primary : AppColors.Border;
    }
}
