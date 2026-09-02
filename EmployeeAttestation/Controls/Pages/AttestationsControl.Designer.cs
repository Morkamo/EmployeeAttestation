using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class AttestationsControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private TableLayoutPanel toolbarLayout = null!;
    private TextBox searchTextBox = null!;
    private Button createButton = null!;
    private Button openButton = null!;
    private Button startButton = null!;
    private Button cancelAttestationButton = null!;
    private Panel filtersPanel = null!;
    private Label statusFilterLabel = null!;
    private ComboBox statusFilterComboBox = null!;
    private DataGridView attestationsGrid = null!;
    private DataGridViewTextBoxColumn employeeColumn = null!;
    private DataGridViewTextBoxColumn departmentColumn = null!;
    private DataGridViewTextBoxColumn positionColumn = null!;
    private DataGridViewTextBoxColumn dateColumn = null!;
    private DataGridViewTextBoxColumn commissionColumn = null!;
    private DataGridViewTextBoxColumn statusColumn = null!;
    private DataGridViewTextBoxColumn managerialColumn = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel();
        titleLabel = new Label();
        subtitleLabel = new Label();
        toolbarLayout = new TableLayoutPanel();
        searchTextBox = new TextBox();
        createButton = new Button();
        openButton = new Button();
        startButton = new Button();
        cancelAttestationButton = new Button();
        filtersPanel = new Panel();
        statusFilterLabel = new Label();
        statusFilterComboBox = new ComboBox();
        attestationsGrid = new DataGridView();
        employeeColumn = new DataGridViewTextBoxColumn();
        departmentColumn = new DataGridViewTextBoxColumn();
        positionColumn = new DataGridViewTextBoxColumn();
        dateColumn = new DataGridViewTextBoxColumn();
        commissionColumn = new DataGridViewTextBoxColumn();
        statusColumn = new DataGridViewTextBoxColumn();
        managerialColumn = new DataGridViewTextBoxColumn();
        pageLayout.SuspendLayout();
        toolbarLayout.SuspendLayout();
        filtersPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)attestationsGrid).BeginInit();
        SuspendLayout();
        pageLayout.ColumnCount = 1;
        pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(titleLabel, 0, 0);
        pageLayout.Controls.Add(subtitleLabel, 0, 1);
        pageLayout.Controls.Add(toolbarLayout, 0, 2);
        pageLayout.Controls.Add(filtersPanel, 0, 3);
        pageLayout.Controls.Add(attestationsGrid, 0, 4);
        pageLayout.Dock = DockStyle.Fill;
        pageLayout.Padding = new Padding(36);
        pageLayout.RowCount = 5;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold);
        titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 6);
        titleLabel.Text = "Аттестации";
        subtitleLabel.AutoSize = true;
        subtitleLabel.Font = new Font("Segoe UI", 11F);
        subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 30);
        subtitleLabel.Text = "Планирование и управление жизненным циклом аттестаций сотрудников.";
        toolbarLayout.BackColor = AppColors.Surface;
        toolbarLayout.ColumnCount = 9;
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 176F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        toolbarLayout.Controls.Add(searchTextBox, 0, 0);
        toolbarLayout.Controls.Add(createButton, 2, 0);
        toolbarLayout.Controls.Add(openButton, 4, 0);
        toolbarLayout.Controls.Add(startButton, 6, 0);
        toolbarLayout.Controls.Add(cancelAttestationButton, 8, 0);
        toolbarLayout.Dock = DockStyle.Fill;
        toolbarLayout.Margin = new Padding(0, 0, 0, 16);
        toolbarLayout.Padding = new Padding(16);
        searchTextBox.BorderStyle = BorderStyle.FixedSingle;
        searchTextBox.Dock = DockStyle.Fill;
        searchTextBox.Font = new Font("Segoe UI", 11F);
        searchTextBox.Margin = new Padding(0, 8, 0, 7);
        searchTextBox.PlaceholderText = "Поиск по сотруднику, подразделению, должности или комиссии";
        searchTextBox.TextChanged += SearchTextBox_TextChanged;
        ConfigureToolbarButton(createButton, "Создать аттестацию");
        createButton.Click += CreateButton_Click;
        ConfigureToolbarButton(openButton, "Открыть");
        openButton.Click += OpenButton_Click;
        ConfigureToolbarButton(startButton, "Начать аттестацию");
        startButton.Click += StartButton_Click;
        ConfigureToolbarButton(cancelAttestationButton, "Отменить");
        cancelAttestationButton.Click += CancelAttestationButton_Click;
        filtersPanel.BackColor = AppColors.Surface;
        filtersPanel.Controls.Add(statusFilterComboBox);
        filtersPanel.Controls.Add(statusFilterLabel);
        filtersPanel.Dock = DockStyle.Fill;
        filtersPanel.Margin = new Padding(0, 0, 0, 16);
        filtersPanel.Padding = new Padding(16, 10, 16, 10);
        statusFilterLabel.AutoSize = true;
        statusFilterLabel.Font = new Font("Segoe UI Semibold", 10F);
        statusFilterLabel.ForeColor = AppColors.TextSecondary;
        statusFilterLabel.Location = new Point(16, 20);
        statusFilterLabel.Text = "Статус:";
        statusFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        statusFilterComboBox.Font = new Font("Segoe UI", 10F);
        statusFilterComboBox.Location = new Point(84, 14);
        statusFilterComboBox.Size = new Size(230, 25);
        statusFilterComboBox.SelectedIndexChanged += StatusFilterComboBox_SelectedIndexChanged;
        attestationsGrid.AllowUserToAddRows = false;
        attestationsGrid.AllowUserToDeleteRows = false;
        attestationsGrid.AllowUserToResizeRows = false;
        attestationsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        attestationsGrid.Columns.AddRange(employeeColumn, departmentColumn, positionColumn, dateColumn,
            commissionColumn, statusColumn, managerialColumn);
        attestationsGrid.Dock = DockStyle.Fill;
        attestationsGrid.MultiSelect = false;
        attestationsGrid.ReadOnly = true;
        attestationsGrid.RowHeadersVisible = false;
        attestationsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        attestationsGrid.SelectionChanged += AttestationsGrid_SelectionChanged;
        attestationsGrid.CellDoubleClick += AttestationsGrid_CellDoubleClick;
        ConfigureGridColumn(employeeColumn, "Сотрудник", "employeeColumn", 130F);
        ConfigureGridColumn(departmentColumn, "Подразделение", "departmentColumn", 105F);
        ConfigureGridColumn(positionColumn, "Должность", "positionColumn", 105F);
        ConfigureGridColumn(dateColumn, "Дата", "dateColumn", 65F);
        ConfigureGridColumn(commissionColumn, "Комиссия", "commissionColumn", 120F);
        ConfigureGridColumn(statusColumn, "Статус", "statusColumn", 85F);
        ConfigureGridColumn(managerialColumn, "Руководитель", "managerialColumn", 65F);
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Background;
        Controls.Add(pageLayout);
        Font = new Font("Segoe UI", 9F);
        Name = "AttestationsControl";
        Size = new Size(1340, 900);
        pageLayout.ResumeLayout(false);
        pageLayout.PerformLayout();
        toolbarLayout.ResumeLayout(false);
        toolbarLayout.PerformLayout();
        filtersPanel.ResumeLayout(false);
        filtersPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)attestationsGrid).EndInit();
        ResumeLayout(false);
    }

    private static void ConfigureToolbarButton(Button button, string text)
    {
        button.Cursor = Cursors.Hand;
        button.Dock = DockStyle.Fill;
        button.FlatStyle = FlatStyle.Flat;
        button.Margin = new Padding(0);
        button.Text = text;
        button.UseVisualStyleBackColor = false;
    }

    private static void ConfigureGridColumn(
        DataGridViewTextBoxColumn column,
        string headerText,
        string name,
        float fillWeight)
    {
        column.HeaderText = headerText;
        column.Name = name;
        column.FillWeight = fillWeight;
    }
}
