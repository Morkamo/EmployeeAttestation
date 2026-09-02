using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class CommissionsControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!; private Label titleLabel = null!; private Label subtitleLabel = null!;
    private TableLayoutPanel workspaceLayout = null!; private TableLayoutPanel commissionsPanel = null!; private TableLayoutPanel membersPanel = null!;
    private Label commissionsTitleLabel = null!; private Label membersTitleLabel = null!; private FlowLayoutPanel commissionsButtonsPanel = null!;
    private FlowLayoutPanel membersButtonsPanel = null!; private Button addButton = null!; private Button editButton = null!;
    private Button editMembersButton = null!; private DataGridView commissionsGrid = null!; private DataGridView membersGrid = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); subtitleLabel = new Label(); workspaceLayout = new TableLayoutPanel();
        commissionsPanel = new TableLayoutPanel(); membersPanel = new TableLayoutPanel(); commissionsTitleLabel = new Label(); membersTitleLabel = new Label();
        commissionsButtonsPanel = new FlowLayoutPanel(); membersButtonsPanel = new FlowLayoutPanel(); addButton = new Button(); editButton = new Button();
        editMembersButton = new Button(); commissionsGrid = new DataGridView(); membersGrid = new DataGridView(); pageLayout.SuspendLayout();
        workspaceLayout.SuspendLayout(); commissionsPanel.SuspendLayout(); membersPanel.SuspendLayout(); commissionsButtonsPanel.SuspendLayout();
        membersButtonsPanel.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)commissionsGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)membersGrid).BeginInit(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); pageLayout.Controls.Add(titleLabel, 0, 0);
        pageLayout.Controls.Add(subtitleLabel, 0, 1); pageLayout.Controls.Add(workspaceLayout, 0, 2); pageLayout.Dock = DockStyle.Fill;
        pageLayout.Padding = new Padding(36); pageLayout.RowCount = 3; pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 6); titleLabel.Text = "Комиссии";
        subtitleLabel.AutoSize = true; subtitleLabel.Font = new Font("Segoe UI", 11F); subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 30); subtitleLabel.Text = "Управление комиссиями и их составом.";
        workspaceLayout.ColumnCount = 2; workspaceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        workspaceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); workspaceLayout.Controls.Add(commissionsPanel, 0, 0);
        workspaceLayout.Controls.Add(membersPanel, 1, 0); workspaceLayout.Dock = DockStyle.Fill; workspaceLayout.RowCount = 1;
        workspaceLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        ConfigureSection(commissionsPanel, commissionsTitleLabel, commissionsButtonsPanel, commissionsGrid, new Padding(0, 0, 8, 0));
        ConfigureSection(membersPanel, membersTitleLabel, membersButtonsPanel, membersGrid, new Padding(8, 0, 0, 0));
        commissionsTitleLabel.Text = "Комиссии"; membersTitleLabel.Text = "Состав комиссии";
        commissionsButtonsPanel.Controls.Add(addButton); commissionsButtonsPanel.Controls.Add(editButton);
        membersButtonsPanel.Controls.Add(editMembersButton); ConfigureSectionButton(addButton, "Добавить", 120);
        ConfigureSectionButton(editButton, "Изменить", 120); ConfigureSectionButton(editMembersButton, "Изменить состав", 170);
        commissionsGrid.AllowUserToAddRows = false; commissionsGrid.AllowUserToDeleteRows = false; commissionsGrid.AllowUserToResizeRows = false;
        commissionsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        commissionsGrid.Columns.AddRange(new DataGridViewTextBoxColumn { HeaderText = "Название комиссии", Name = "nameColumn" },
            new DataGridViewTextBoxColumn { HeaderText = "Описание", Name = "descriptionColumn" });
        ConfigureGrid(commissionsGrid);
        membersGrid.AllowUserToAddRows = false; membersGrid.AllowUserToDeleteRows = false; membersGrid.AllowUserToResizeRows = false;
        membersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        membersGrid.Columns.AddRange(new DataGridViewTextBoxColumn { HeaderText = "ФИО", Name = "fullNameColumn", FillWeight = 140 },
            new DataGridViewTextBoxColumn { HeaderText = "Роль", Name = "roleColumn", FillWeight = 75 });
        ConfigureGrid(membersGrid);
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F); Name = "CommissionsControl"; Size = new Size(1340, 900);
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); workspaceLayout.ResumeLayout(false); commissionsPanel.ResumeLayout(false);
        commissionsPanel.PerformLayout(); membersPanel.ResumeLayout(false); membersPanel.PerformLayout(); commissionsButtonsPanel.ResumeLayout(false);
        membersButtonsPanel.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)commissionsGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)membersGrid).EndInit(); ResumeLayout(false);
    }

    private static void ConfigureSection(TableLayoutPanel panel, Label title, FlowLayoutPanel buttons, DataGridView grid, Padding margin)
    {
        panel.BackColor = AppColors.Surface; panel.BorderStyle = BorderStyle.FixedSingle; panel.ColumnCount = 1;
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); panel.Controls.Add(title, 0, 0); panel.Controls.Add(buttons, 0, 1);
        panel.Controls.Add(grid, 0, 2); panel.Dock = DockStyle.Fill; panel.Margin = margin; panel.Padding = new Padding(20);
        panel.RowCount = 3; panel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); title.AutoSize = true; title.Font = new Font("Segoe UI Semibold", 15F);
        title.ForeColor = AppColors.TextPrimary; title.Margin = new Padding(0, 0, 0, 12); buttons.Dock = DockStyle.Fill;
        buttons.Margin = new Padding(0, 0, 0, 16); buttons.WrapContents = false;
    }

    private static void ConfigureSectionButton(Button button, string text, int width)
    {
        button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(0, 0, 10, 0);
        button.Size = new Size(width, 44); button.Text = text; button.UseVisualStyleBackColor = false;
    }

    private static void ConfigureGrid(DataGridView grid)
    { grid.Dock = DockStyle.Fill; grid.MultiSelect = false; grid.ReadOnly = true; grid.RowHeadersVisible = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; }
}
