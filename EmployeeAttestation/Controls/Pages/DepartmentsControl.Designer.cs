using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class DepartmentsControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!; private Label titleLabel = null!; private Label subtitleLabel = null!;
    private TableLayoutPanel toolbarLayout = null!; private TextBox searchTextBox = null!; private Button addButton = null!;
    private Button editButton = null!; private Button deleteButton = null!; private DataGridView departmentsGrid = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); subtitleLabel = new Label(); toolbarLayout = new TableLayoutPanel();
        searchTextBox = new TextBox(); addButton = new Button(); editButton = new Button(); deleteButton = new Button(); departmentsGrid = new DataGridView();
        pageLayout.SuspendLayout(); toolbarLayout.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)departmentsGrid).BeginInit(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); pageLayout.Controls.Add(titleLabel, 0, 0);
        pageLayout.Controls.Add(subtitleLabel, 0, 1); pageLayout.Controls.Add(toolbarLayout, 0, 2); pageLayout.Controls.Add(departmentsGrid, 0, 3);
        pageLayout.Dock = DockStyle.Fill; pageLayout.Padding = new Padding(36); pageLayout.RowCount = 4;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 102F)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 6); titleLabel.Text = "Подразделения";
        subtitleLabel.AutoSize = true; subtitleLabel.Font = new Font("Segoe UI", 11F); subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 30); subtitleLabel.Text = "Управление структурными подразделениями организации.";
        ConfigureToolbar(toolbarLayout, searchTextBox, addButton, editButton, deleteButton); searchTextBox.PlaceholderText = "Поиск подразделений";
        ConfigureToolbarButton(addButton, "Добавить"); ConfigureToolbarButton(editButton, "Изменить"); ConfigureToolbarButton(deleteButton, "Удалить");
        searchTextBox.TextChanged += SearchTextBox_TextChanged; addButton.Click += AddButton_Click;
        editButton.Click += EditButton_Click; deleteButton.Click += DeleteButton_Click;
        departmentsGrid.AllowUserToAddRows = false; departmentsGrid.AllowUserToDeleteRows = false; departmentsGrid.AllowUserToResizeRows = false;
        departmentsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        departmentsGrid.Columns.AddRange(new DataGridViewTextBoxColumn { HeaderText = "Код", Name = "codeColumn", FillWeight = 35 },
            new DataGridViewTextBoxColumn { HeaderText = "Наименование", Name = "nameColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Наименование в документах", Name = "documentNameColumn", FillWeight = 130 });
        SetColumnWidth(departmentsGrid.Columns["codeColumn"], 110);
        SetColumnWidth(departmentsGrid.Columns["nameColumn"], 320);
        SetColumnWidth(departmentsGrid.Columns["documentNameColumn"], 380);
        ConfigureGrid(departmentsGrid);
        departmentsGrid.CellDoubleClick += DepartmentsGrid_CellDoubleClick;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F); Name = "DepartmentsControl"; Size = new Size(1340, 900);
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); toolbarLayout.ResumeLayout(false); toolbarLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)departmentsGrid).EndInit(); ResumeLayout(false);
    }

    private static void ConfigureToolbar(TableLayoutPanel toolbar, TextBox search, Button first, Button second, Button third)
    {
        toolbar.AutoScroll = true; toolbar.BackColor = AppColors.Surface; toolbar.ColumnCount = 7;
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); toolbar.Controls.Add(search, 0, 0); toolbar.Controls.Add(first, 2, 0);
        toolbar.Controls.Add(second, 4, 0); toolbar.Controls.Add(third, 6, 0); toolbar.Dock = DockStyle.Fill;
        toolbar.Margin = new Padding(0, 0, 0, 16); toolbar.MinimumSize = new Size(560, 0); toolbar.Padding = new Padding(16);
        search.BorderStyle = BorderStyle.FixedSingle; search.Dock = DockStyle.Fill; search.Font = new Font("Segoe UI", 11F); search.Margin = new Padding(0, 8, 0, 7);
    }
    private static void ConfigureToolbarButton(Button button, string text)
    { button.AutoSize = true; button.AutoSizeMode = AutoSizeMode.GrowAndShrink; button.Cursor = Cursors.Hand; button.Dock = DockStyle.Fill; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0); button.MinimumSize = new Size(110, 42); button.Padding = new Padding(14, 0, 14, 0); button.Text = text; button.UseVisualStyleBackColor = false; }
    private static void ConfigureGrid(DataGridView grid)
    { grid.Dock = DockStyle.Fill; grid.MultiSelect = false; grid.ReadOnly = true; grid.RowHeadersVisible = false; grid.ScrollBars = ScrollBars.Both; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; }

    private static void SetColumnWidth(DataGridViewColumn column, int width)
    { column.FillWeight = width; column.MinimumWidth = Math.Min(width, 180); }
}
