using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class EvaluationCriteriaForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel layout = null!; private Label titleLabel = null!; private TableLayoutPanel toolbar = null!;
    private TextBox searchTextBox = null!; private ComboBox categoryFilterComboBox = null!; private ComboBox activityFilterComboBox = null!;
    private Button addButton = null!; private Button editButton = null!; private Button deleteButton = null!; private DataGridView criteriaGrid = null!;
    private FlowLayoutPanel footer = null!; private Button closeButton = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel(); titleLabel = new Label(); toolbar = new TableLayoutPanel(); searchTextBox = new TextBox();
        categoryFilterComboBox = new ComboBox(); activityFilterComboBox = new ComboBox(); addButton = new Button(); editButton = new Button();
        deleteButton = new Button(); criteriaGrid = new DataGridView(); footer = new FlowLayoutPanel(); closeButton = new Button();
        SuspendLayout();
        layout.ColumnCount = 1; layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(28, 24, 28, 22); layout.RowCount = 4;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 20F); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Text = "Критерии аттестации"; layout.Controls.Add(titleLabel, 0, 0);
        toolbar.ColumnCount = 11; toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 175F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F)); toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128F));
        toolbar.Dock = DockStyle.Fill; toolbar.Padding = new Padding(12); toolbar.BackColor = AppColors.Surface;
        ConfigureInput(searchTextBox); searchTextBox.PlaceholderText = "Поиск критерия"; searchTextBox.TextChanged += FilterChanged;
        ConfigureCombo(categoryFilterComboBox); categoryFilterComboBox.SelectedIndexChanged += FilterChanged;
        ConfigureCombo(activityFilterComboBox); activityFilterComboBox.SelectedIndexChanged += FilterChanged;
        ConfigureButton(addButton, "Добавить"); addButton.Click += AddButton_Click;
        ConfigureButton(editButton, "Изменить"); editButton.Click += EditButton_Click;
        ConfigureButton(deleteButton, "Удалить"); deleteButton.Click += DeleteButton_Click;
        toolbar.Controls.Add(searchTextBox, 0, 0); toolbar.Controls.Add(categoryFilterComboBox, 2, 0); toolbar.Controls.Add(activityFilterComboBox, 4, 0);
        toolbar.Controls.Add(addButton, 6, 0); toolbar.Controls.Add(editButton, 8, 0); toolbar.Controls.Add(deleteButton, 10, 0); layout.Controls.Add(toolbar, 0, 1);
        criteriaGrid.AllowUserToAddRows = false; criteriaGrid.AllowUserToDeleteRows = false; criteriaGrid.AllowUserToResizeRows = false;
        criteriaGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; criteriaGrid.Dock = DockStyle.Fill; criteriaGrid.MultiSelect = false;
        criteriaGrid.ReadOnly = true; criteriaGrid.RowHeadersVisible = false; criteriaGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        criteriaGrid.Columns.AddRange(Column("name", "Наименование", 160), Column("category", "Категория", 100), Column("range", "Баллы", 55),
            Column("manager", "Только руководителям", 90), Column("status", "Статус", 70), Column("order", "Порядок", 55));
        criteriaGrid.SelectionChanged += CriteriaGrid_SelectionChanged; criteriaGrid.CellDoubleClick += CriteriaGrid_CellDoubleClick; layout.Controls.Add(criteriaGrid, 0, 2);
        footer.Dock = DockStyle.Fill; footer.FlowDirection = FlowDirection.RightToLeft; ConfigureButton(closeButton, "Закрыть");
        closeButton.DialogResult = DialogResult.Cancel; footer.Controls.Add(closeButton); layout.Controls.Add(footer, 0, 3);
        AppControlStyles.ApplySecondaryButton(closeButton); CancelButton = closeButton; AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Background; ClientSize = new Size(1120, 680); Controls.Add(layout); MinimumSize = new Size(920, 560);
        Name = "EvaluationCriteriaForm"; StartPosition = FormStartPosition.CenterParent; Text = "Критерии аттестации"; ResumeLayout(false);
    }
    private static DataGridViewTextBoxColumn Column(string name, string header, float weight) => new() { Name = name, HeaderText = header, FillWeight = weight };
    private static void ConfigureInput(TextBox c) { c.Dock = DockStyle.Fill; c.Font = new Font("Segoe UI", 10F); c.Margin = new Padding(0, 6, 0, 5); }
    private static void ConfigureCombo(ComboBox c) { c.Dock = DockStyle.Fill; c.DropDownStyle = ComboBoxStyle.DropDownList; c.Font = new Font("Segoe UI", 10F); c.Margin = new Padding(0, 5, 0, 5); }
    private static void ConfigureButton(Button b, string text) { b.Dock = DockStyle.Fill; b.FlatStyle = FlatStyle.Flat; b.Margin = new Padding(0); b.Text = text; b.Cursor = Cursors.Hand; }
}
