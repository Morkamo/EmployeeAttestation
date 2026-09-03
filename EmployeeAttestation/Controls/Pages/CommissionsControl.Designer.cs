using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class CommissionsControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private TableLayoutPanel workspaceLayout = null!;
    private TableLayoutPanel commissionsPanel = null!;
    private Label commissionsTitleLabel = null!;
    private TableLayoutPanel filterLayout = null!;
    private TextBox searchTextBox = null!;
    private ComboBox statusFilterComboBox = null!;
    private FlowLayoutPanel commissionsButtonsPanel = null!;
    private Button addButton = null!;
    private Button editButton = null!;
    private Button archiveButton = null!;
    private DataGridView commissionsGrid = null!;
    private TableLayoutPanel membersPanel = null!;
    private Label membersTitleLabel = null!;
    private FlowLayoutPanel membersButtonsPanel = null!;
    private Button editMembersButton = null!;
    private Button manageMembersButton = null!;
    private DataGridView membersGrid = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); subtitleLabel = new Label(); workspaceLayout = new TableLayoutPanel();
        commissionsPanel = new TableLayoutPanel(); commissionsTitleLabel = new Label(); filterLayout = new TableLayoutPanel();
        searchTextBox = new TextBox(); statusFilterComboBox = new ComboBox(); commissionsButtonsPanel = new FlowLayoutPanel();
        addButton = new Button(); editButton = new Button(); archiveButton = new Button(); commissionsGrid = new DataGridView();
        membersPanel = new TableLayoutPanel(); membersTitleLabel = new Label(); membersButtonsPanel = new FlowLayoutPanel();
        editMembersButton = new Button(); manageMembersButton = new Button(); membersGrid = new DataGridView();
        pageLayout.SuspendLayout(); workspaceLayout.SuspendLayout(); commissionsPanel.SuspendLayout(); filterLayout.SuspendLayout();
        commissionsButtonsPanel.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)commissionsGrid).BeginInit(); membersPanel.SuspendLayout();
        membersButtonsPanel.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)membersGrid).BeginInit(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(titleLabel, 0, 0); pageLayout.Controls.Add(subtitleLabel, 0, 1); pageLayout.Controls.Add(workspaceLayout, 0, 2);
        pageLayout.Dock = DockStyle.Fill; pageLayout.Padding = new Padding(28); pageLayout.RowCount = 3;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 6); titleLabel.Text = "Комиссии";
        subtitleLabel.AutoSize = true; subtitleLabel.Font = new Font("Segoe UI", 11F); subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 22); subtitleLabel.Text = "Управление комиссиями и их составом.";
        workspaceLayout.ColumnCount = 2; workspaceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
        workspaceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F)); workspaceLayout.Controls.Add(commissionsPanel, 0, 0);
        workspaceLayout.Controls.Add(membersPanel, 1, 0); workspaceLayout.Dock = DockStyle.Fill; workspaceLayout.RowCount = 1;
        workspaceLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        ConfigureSection(commissionsPanel, commissionsTitleLabel, new Padding(0, 0, 8, 0), 4);
        commissionsTitleLabel.Text = "Комиссии"; commissionsPanel.Controls.Add(filterLayout, 0, 1);
        commissionsPanel.Controls.Add(commissionsButtonsPanel, 0, 2); commissionsPanel.Controls.Add(commissionsGrid, 0, 3);
        commissionsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); commissionsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        commissionsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F)); commissionsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        filterLayout.ColumnCount = 3; filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F)); filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165F));
        filterLayout.Controls.Add(searchTextBox, 0, 0); filterLayout.Controls.Add(statusFilterComboBox, 2, 0); filterLayout.Dock = DockStyle.Fill;
        filterLayout.Margin = new Padding(0, 0, 0, 4); searchTextBox.BorderStyle = BorderStyle.FixedSingle; searchTextBox.Dock = DockStyle.Fill;
        searchTextBox.Font = new Font("Segoe UI", 10F); searchTextBox.Margin = new Padding(0, 3, 0, 5); searchTextBox.PlaceholderText = "Поиск комиссий";
        searchTextBox.TextChanged += SearchTextBox_TextChanged; statusFilterComboBox.Dock = DockStyle.Fill;
        statusFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList; statusFilterComboBox.Font = new Font("Segoe UI", 10F);
        statusFilterComboBox.Items.AddRange(new object[] { "Все", "Активные", "Архивированные" }); statusFilterComboBox.Margin = new Padding(0, 2, 0, 5);
        statusFilterComboBox.SelectedIndex = 1; statusFilterComboBox.SelectedIndexChanged += StatusFilterComboBox_SelectedIndexChanged;
        commissionsButtonsPanel.AutoScroll = true; commissionsButtonsPanel.Controls.Add(addButton); commissionsButtonsPanel.Controls.Add(editButton); commissionsButtonsPanel.Controls.Add(archiveButton);
        commissionsButtonsPanel.Dock = DockStyle.Fill; commissionsButtonsPanel.Margin = new Padding(0, 3, 0, 4); commissionsButtonsPanel.WrapContents = true;
        ConfigureSectionButton(addButton, "Добавить", 110); addButton.Click += AddButton_Click;
        ConfigureSectionButton(editButton, "Изменить", 110); editButton.Click += EditButton_Click;
        ConfigureSectionButton(archiveButton, "Архивировать", 135); archiveButton.Click += ArchiveButton_Click;
        ConfigureGrid(commissionsGrid); commissionsGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Название", Name = "nameColumn", FillWeight = 120F, MinimumWidth = 180 },
            new DataGridViewTextBoxColumn { HeaderText = "Описание", Name = "descriptionColumn", FillWeight = 140F, MinimumWidth = 220 },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", Name = "statusColumn", FillWeight = 70F, MinimumWidth = 120 });
        commissionsGrid.SelectionChanged += CommissionsGrid_SelectionChanged; commissionsGrid.CellDoubleClick += CommissionsGrid_CellDoubleClick;
        ConfigureSection(membersPanel, membersTitleLabel, new Padding(8, 0, 0, 0), 3); membersTitleLabel.Text = "Состав комиссии";
        membersPanel.Controls.Add(membersButtonsPanel, 0, 1); membersPanel.Controls.Add(membersGrid, 0, 2);
        membersPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); membersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        membersPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); membersButtonsPanel.Controls.Add(editMembersButton);
        membersButtonsPanel.Controls.Add(manageMembersButton); membersButtonsPanel.AutoScroll = true; membersButtonsPanel.Dock = DockStyle.Fill;
        membersButtonsPanel.Margin = new Padding(0, 3, 0, 4); membersButtonsPanel.WrapContents = true;
        ConfigureSectionButton(editMembersButton, "Изменить состав", 155); editMembersButton.Click += EditMembersButton_Click;
        ConfigureSectionButton(manageMembersButton, "Члены комиссии", 155); manageMembersButton.Click += ManageMembersButton_Click;
        ConfigureGrid(membersGrid); membersGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "ФИО", Name = "fullNameColumn", FillWeight = 140F, MinimumWidth = 220 },
            new DataGridViewTextBoxColumn { HeaderText = "Роль", Name = "roleColumn", FillWeight = 100F, MinimumWidth = 170 },
            new DataGridViewTextBoxColumn { HeaderText = "Порядок", Name = "sortOrderColumn", FillWeight = 50F, MinimumWidth = 90 });
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F); Name = "CommissionsControl"; Size = new Size(1340, 900);
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); workspaceLayout.ResumeLayout(false); commissionsPanel.ResumeLayout(false);
        commissionsPanel.PerformLayout(); filterLayout.ResumeLayout(false); filterLayout.PerformLayout(); commissionsButtonsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)commissionsGrid).EndInit(); membersPanel.ResumeLayout(false); membersPanel.PerformLayout();
        membersButtonsPanel.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)membersGrid).EndInit(); ResumeLayout(false);
    }

    private static void ConfigureSection(TableLayoutPanel panel, Label title, Padding margin, int rowCount)
    {
        panel.BackColor = AppColors.Surface; panel.BorderStyle = BorderStyle.FixedSingle; panel.ColumnCount = 1;
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); panel.Controls.Add(title, 0, 0); panel.Dock = DockStyle.Fill;
        panel.Margin = margin; panel.Padding = new Padding(16); panel.RowCount = rowCount; title.AutoSize = true;
        title.Font = new Font("Segoe UI Semibold", 15F); title.ForeColor = AppColors.TextPrimary; title.Margin = new Padding(0, 0, 0, 10);
    }

    private static void ConfigureSectionButton(Button button, string text, int width)
    {
        button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0, 0, 8, 4);
        button.Size = new Size(width, 40); button.Padding = new Padding(10, 0, 10, 0); button.Text = text; button.UseVisualStyleBackColor = false;
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false; grid.AllowUserToResizeRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.Dock = DockStyle.Fill; grid.MultiSelect = false;
        grid.ReadOnly = true; grid.RowHeadersVisible = false; grid.ScrollBars = ScrollBars.Both; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }
}
