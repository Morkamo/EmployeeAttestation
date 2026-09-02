using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class EmployeesControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private TableLayoutPanel toolbarLayout = null!;
    private TextBox searchTextBox = null!;
    private Button addButton = null!;
    private Button editButton = null!;
    private Button archiveButton = null!;
    private DataGridView employeesGrid = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); subtitleLabel = new Label();
        toolbarLayout = new TableLayoutPanel(); searchTextBox = new TextBox(); addButton = new Button();
        editButton = new Button(); archiveButton = new Button(); employeesGrid = new DataGridView();
        pageLayout.SuspendLayout(); toolbarLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)employeesGrid).BeginInit(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(titleLabel, 0, 0); pageLayout.Controls.Add(subtitleLabel, 0, 1);
        pageLayout.Controls.Add(toolbarLayout, 0, 2); pageLayout.Controls.Add(employeesGrid, 0, 3);
        pageLayout.Dock = DockStyle.Fill; pageLayout.Padding = new Padding(36); pageLayout.RowCount = 4;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold);
        titleLabel.ForeColor = AppColors.TextPrimary; titleLabel.Margin = new Padding(0, 0, 0, 6); titleLabel.Text = "Сотрудники";
        subtitleLabel.AutoSize = true; subtitleLabel.Font = new Font("Segoe UI", 11F); subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 30); subtitleLabel.Text = "Просмотр и управление списком сотрудников организации.";
        ConfigureToolbar(toolbarLayout, searchTextBox, addButton, editButton, archiveButton);
        searchTextBox.PlaceholderText = "Поиск сотрудников";
        ConfigureToolbarButton(addButton, "Добавить"); ConfigureToolbarButton(editButton, "Изменить"); ConfigureToolbarButton(archiveButton, "Архивировать");
        employeesGrid.AllowUserToAddRows = false; employeesGrid.AllowUserToDeleteRows = false; employeesGrid.AllowUserToResizeRows = false;
        employeesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        employeesGrid.Columns.AddRange(new DataGridViewTextBoxColumn { HeaderText = "ФИО", Name = "fullNameColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Подразделение", Name = "departmentColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Должность", Name = "positionColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Руководитель", Name = "managerColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", Name = "statusColumn" });
        ConfigureGrid(employeesGrid);
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F); Name = "EmployeesControl"; Size = new Size(1340, 900);
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); toolbarLayout.ResumeLayout(false); toolbarLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)employeesGrid).EndInit(); ResumeLayout(false);
    }

    private static void ConfigureToolbar(TableLayoutPanel toolbar, TextBox search, Button first, Button second, Button third)
    {
        toolbar.BackColor = AppColors.Surface; toolbar.ColumnCount = 7;
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F)); toolbar.Controls.Add(search, 0, 0); toolbar.Controls.Add(first, 2, 0);
        toolbar.Controls.Add(second, 4, 0); toolbar.Controls.Add(third, 6, 0); toolbar.Dock = DockStyle.Fill;
        toolbar.Margin = new Padding(0, 0, 0, 16); toolbar.Padding = new Padding(16);
        search.BorderStyle = BorderStyle.FixedSingle; search.Dock = DockStyle.Fill; search.Font = new Font("Segoe UI", 11F); search.Margin = new Padding(0, 8, 0, 7);
    }

    private static void ConfigureToolbarButton(Button button, string text)
    { button.Cursor = Cursors.Hand; button.Dock = DockStyle.Fill; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0); button.Text = text; button.UseVisualStyleBackColor = false; }

    private static void ConfigureGrid(DataGridView grid)
    { grid.Dock = DockStyle.Fill; grid.MultiSelect = false; grid.ReadOnly = true; grid.RowHeadersVisible = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; }
}
