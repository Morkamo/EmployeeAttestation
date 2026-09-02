using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class CommissionMembersForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!;
    private Label titleLabel = null!;
    private TableLayoutPanel toolbarLayout = null!;
    private TextBox searchTextBox = null!;
    private ComboBox statusFilterComboBox = null!;
    private Button addButton = null!;
    private Button editButton = null!;
    private Button archiveButton = null!;
    private DataGridView membersGrid = null!;
    private FlowLayoutPanel footerPanel = null!;
    private Button closeButton = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); toolbarLayout = new TableLayoutPanel(); searchTextBox = new TextBox();
        statusFilterComboBox = new ComboBox(); addButton = new Button(); editButton = new Button(); archiveButton = new Button();
        membersGrid = new DataGridView(); footerPanel = new FlowLayoutPanel(); closeButton = new Button();
        pageLayout.SuspendLayout(); toolbarLayout.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)membersGrid).BeginInit();
        footerPanel.SuspendLayout(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(titleLabel, 0, 0); pageLayout.Controls.Add(toolbarLayout, 0, 1); pageLayout.Controls.Add(membersGrid, 0, 2);
        pageLayout.Controls.Add(footerPanel, 0, 3); pageLayout.Dock = DockStyle.Fill; pageLayout.Padding = new Padding(28);
        pageLayout.RowCount = 4; pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 18); titleLabel.Text = "Члены комиссии";
        toolbarLayout.BackColor = AppColors.Surface; toolbarLayout.ColumnCount = 9; toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F)); toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F)); toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F)); toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F)); toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
        toolbarLayout.Controls.Add(searchTextBox, 0, 0); toolbarLayout.Controls.Add(statusFilterComboBox, 2, 0); toolbarLayout.Controls.Add(addButton, 4, 0);
        toolbarLayout.Controls.Add(editButton, 6, 0); toolbarLayout.Controls.Add(archiveButton, 8, 0); toolbarLayout.Dock = DockStyle.Fill;
        toolbarLayout.Margin = new Padding(0, 0, 0, 14); toolbarLayout.Padding = new Padding(12);
        searchTextBox.BorderStyle = BorderStyle.FixedSingle; searchTextBox.Dock = DockStyle.Fill; searchTextBox.Font = new Font("Segoe UI", 10F);
        searchTextBox.Margin = new Padding(0, 5, 0, 5); searchTextBox.PlaceholderText = "Поиск"; searchTextBox.TextChanged += SearchTextBox_TextChanged;
        statusFilterComboBox.Dock = DockStyle.Fill; statusFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList; statusFilterComboBox.Font = new Font("Segoe UI", 10F);
        statusFilterComboBox.Items.AddRange(new object[] { "Все", "Активные", "Архивированные" }); statusFilterComboBox.Margin = new Padding(0, 4, 0, 4);
        statusFilterComboBox.SelectedIndex = 1; statusFilterComboBox.SelectedIndexChanged += StatusFilterComboBox_SelectedIndexChanged;
        ConfigureToolbarButton(addButton, "Добавить"); addButton.Click += AddButton_Click;
        ConfigureToolbarButton(editButton, "Изменить"); editButton.Click += EditButton_Click;
        ConfigureToolbarButton(archiveButton, "Архивировать"); archiveButton.Click += ArchiveButton_Click;
        membersGrid.AllowUserToAddRows = false; membersGrid.AllowUserToDeleteRows = false; membersGrid.AllowUserToResizeRows = false;
        membersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; membersGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "ФИО", Name = "fullNameColumn", FillWeight = 160 },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", Name = "statusColumn", FillWeight = 70 });
        membersGrid.Dock = DockStyle.Fill; membersGrid.MultiSelect = false; membersGrid.ReadOnly = true; membersGrid.RowHeadersVisible = false;
        membersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; membersGrid.SelectionChanged += MembersGrid_SelectionChanged;
        membersGrid.CellDoubleClick += MembersGrid_CellDoubleClick;
        footerPanel.Controls.Add(closeButton); footerPanel.Dock = DockStyle.Fill; footerPanel.FlowDirection = FlowDirection.RightToLeft;
        footerPanel.Margin = new Padding(0, 12, 0, 0); footerPanel.WrapContents = false; ConfigureFooterButton(closeButton, "Закрыть");
        closeButton.DialogResult = DialogResult.Cancel;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        CancelButton = closeButton; ClientSize = new Size(900, 600); Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F);
        MinimizeBox = false; Name = "CommissionMembersForm"; StartPosition = FormStartPosition.CenterParent; Text = "Члены комиссии";
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); toolbarLayout.ResumeLayout(false); toolbarLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)membersGrid).EndInit(); footerPanel.ResumeLayout(false); ResumeLayout(false);
    }

    private static void ConfigureToolbarButton(Button button, string text)
    {
        button.Cursor = Cursors.Hand; button.Dock = DockStyle.Fill; button.FlatStyle = FlatStyle.Flat;
        button.Margin = new Padding(0); button.Text = text; button.UseVisualStyleBackColor = false;
    }

    private static void ConfigureFooterButton(Button button, string text)
    {
        button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0);
        button.Size = new Size(130, 44); button.Text = text; button.UseVisualStyleBackColor = false;
    }
}
