using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class SettingsControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private Panel workspacePanel = null!;
    private TableLayoutPanel databaseSettingsLayout = null!;
    private Label databaseTitleLabel = null!;
    private Label databaseDescriptionLabel = null!;
    private TableLayoutPanel databaseActionsLayout = null!;
    private ComboBox databaseComboBox = null!;
    private Button refreshButton = null!;
    private Button openFolderButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeDatabaseWatcher();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel();
        titleLabel = new Label();
        subtitleLabel = new Label();
        workspacePanel = new Panel();
        databaseSettingsLayout = new TableLayoutPanel();
        databaseTitleLabel = new Label();
        databaseDescriptionLabel = new Label();
        databaseActionsLayout = new TableLayoutPanel();
        databaseComboBox = new ComboBox();
        refreshButton = new Button();
        openFolderButton = new Button();
        pageLayout.SuspendLayout();
        workspacePanel.SuspendLayout();
        databaseSettingsLayout.SuspendLayout();
        databaseActionsLayout.SuspendLayout();
        SuspendLayout();
        // 
        // pageLayout
        // 
        pageLayout.ColumnCount = 1;
        pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(titleLabel, 0, 0);
        pageLayout.Controls.Add(subtitleLabel, 0, 1);
        pageLayout.Controls.Add(workspacePanel, 0, 2);
        pageLayout.Dock = DockStyle.Fill;
        pageLayout.Location = new Point(0, 0);
        pageLayout.Margin = new Padding(6);
        pageLayout.Name = "pageLayout";
        pageLayout.Padding = new Padding(67, 77, 67, 77);
        pageLayout.RowCount = 3;
        pageLayout.RowStyles.Add(new RowStyle());
        pageLayout.RowStyles.Add(new RowStyle());
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        pageLayout.Size = new Size(2489, 1920);
        pageLayout.TabIndex = 0;
        // 
        // titleLabel
        // 
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold);
        titleLabel.ForeColor = Color.FromArgb(31, 41, 55);
        titleLabel.Location = new Point(67, 77);
        titleLabel.Margin = new Padding(0, 0, 0, 13);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(396, 93);
        titleLabel.TabIndex = 0;
        titleLabel.Text = "Настройки";
        // 
        // subtitleLabel
        // 
        subtitleLabel.AutoSize = true;
        subtitleLabel.Font = new Font("Segoe UI", 11F);
        subtitleLabel.ForeColor = Color.FromArgb(107, 114, 128);
        subtitleLabel.Location = new Point(71, 183);
        subtitleLabel.Margin = new Padding(4, 0, 0, 64);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Size = new Size(363, 41);
        subtitleLabel.TabIndex = 1;
        subtitleLabel.Text = "Параметры приложения.";
        // 
        // workspacePanel
        // 
        workspacePanel.BackColor = Color.White;
        workspacePanel.BorderStyle = BorderStyle.FixedSingle;
        workspacePanel.Controls.Add(databaseSettingsLayout);
        workspacePanel.Dock = DockStyle.Fill;
        workspacePanel.Location = new Point(73, 294);
        workspacePanel.Margin = new Padding(6);
        workspacePanel.Name = "workspacePanel";
        workspacePanel.Padding = new Padding(52, 60, 52, 60);
        workspacePanel.Size = new Size(2343, 1543);
        workspacePanel.TabIndex = 2;
        // 
        // databaseSettingsLayout
        // 
        databaseSettingsLayout.ColumnCount = 1;
        databaseSettingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        databaseSettingsLayout.Controls.Add(databaseTitleLabel, 0, 0);
        databaseSettingsLayout.Controls.Add(databaseDescriptionLabel, 0, 1);
        databaseSettingsLayout.Controls.Add(databaseActionsLayout, 0, 2);
        databaseSettingsLayout.Dock = DockStyle.Top;
        databaseSettingsLayout.Location = new Point(52, 60);
        databaseSettingsLayout.Margin = new Padding(6);
        databaseSettingsLayout.Name = "databaseSettingsLayout";
        databaseSettingsLayout.RowCount = 3;
        databaseSettingsLayout.RowStyles.Add(new RowStyle());
        databaseSettingsLayout.RowStyles.Add(new RowStyle());
        databaseSettingsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
        databaseSettingsLayout.Size = new Size(2237, 252);
        databaseSettingsLayout.TabIndex = 0;
        // 
        // databaseTitleLabel
        // 
        databaseTitleLabel.AutoSize = true;
        databaseTitleLabel.Font = new Font("Segoe UI Semibold", 15F);
        databaseTitleLabel.ForeColor = Color.FromArgb(31, 41, 55);
        databaseTitleLabel.Location = new Point(0, 0);
        databaseTitleLabel.Margin = new Padding(0, 0, 0, 17);
        databaseTitleLabel.Name = "databaseTitleLabel";
        databaseTitleLabel.Size = new Size(427, 54);
        databaseTitleLabel.TabIndex = 0;
        databaseTitleLabel.Text = "Рабочая база данных";
        // 
        // databaseDescriptionLabel
        // 
        databaseDescriptionLabel.AutoSize = true;
        databaseDescriptionLabel.Font = new Font("Segoe UI", 10F);
        databaseDescriptionLabel.ForeColor = Color.FromArgb(107, 114, 128);
        databaseDescriptionLabel.Location = new Point(0, 71);
        databaseDescriptionLabel.Margin = new Padding(0, 0, 0, 38);
        databaseDescriptionLabel.Name = "databaseDescriptionLabel";
        databaseDescriptionLabel.Size = new Size(764, 37);
        databaseDescriptionLabel.TabIndex = 1;
        databaseDescriptionLabel.Text = "Выберите совместимую базу из локальной папки databases.";
        // 
        // databaseActionsLayout
        // 
        databaseActionsLayout.ColumnCount = 5;
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 176F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        databaseActionsLayout.Controls.Add(databaseComboBox, 0, 0);
        databaseActionsLayout.Controls.Add(refreshButton, 2, 0);
        databaseActionsLayout.Controls.Add(openFolderButton, 4, 0);
        databaseActionsLayout.Location = new Point(6, 152);
        databaseActionsLayout.Margin = new Padding(6);
        databaseActionsLayout.Name = "databaseActionsLayout";
        databaseActionsLayout.RowCount = 1;
        databaseActionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        databaseActionsLayout.Size = new Size(2225, 64);
        databaseActionsLayout.TabIndex = 2;
        // 
        // databaseComboBox
        // 
        databaseComboBox.Dock = DockStyle.Fill;
        databaseComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        databaseComboBox.Font = new Font("Segoe UI", 11F);
        databaseComboBox.IntegralHeight = false;
        databaseComboBox.Location = new Point(0, 8);
        databaseComboBox.Margin = new Padding(0, 8, 0, 8);
        databaseComboBox.Name = "databaseComboBox";
        databaseComboBox.Size = new Size(1793, 48);
        databaseComboBox.TabIndex = 0;
        databaseComboBox.SelectedIndexChanged += DatabaseComboBox_SelectedIndexChanged;
        // 
        // refreshButton
        // 
        refreshButton.Location = new Point(1811, 6);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(176, 52);
        refreshButton.TabIndex = 1;
        refreshButton.Click += RefreshButton_Click;
        // 
        // openFolderButton
        // 
        openFolderButton.Location = new Point(2005, 6);
        openFolderButton.Name = "openFolderButton";
        openFolderButton.Size = new Size(220, 52);
        openFolderButton.TabIndex = 2;
        openFolderButton.Click += OpenFolderButton_Click;
        ConfigureActionButton(refreshButton, "Обновить");
        ConfigureActionButton(openFolderButton, "Открыть папку");
        // 
        // SettingsControl
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(247, 249, 252);
        Controls.Add(pageLayout);
        Font = new Font("Segoe UI", 9F);
        Margin = new Padding(6);
        Name = "SettingsControl";
        Size = new Size(2489, 1920);
        pageLayout.ResumeLayout(false);
        pageLayout.PerformLayout();
        workspacePanel.ResumeLayout(false);
        databaseSettingsLayout.ResumeLayout(false);
        databaseSettingsLayout.PerformLayout();
        databaseActionsLayout.ResumeLayout(false);
        ResumeLayout(false);
    }

    private static void ConfigureActionButton(Button button, string text)
    {
        button.BackColor = AppColors.Surface;
        button.Cursor = Cursors.Hand;
        button.Dock = DockStyle.Fill;
        button.FlatStyle = FlatStyle.Flat;
        button.Font = new Font("Segoe UI Semibold", 10F);
        button.ForeColor = AppColors.Primary;
        button.Margin = new Padding(0);
        button.Text = text;
        button.UseVisualStyleBackColor = false;
        button.FlatAppearance.BorderColor = AppColors.Primary;
        button.FlatAppearance.BorderSize = 1;
    }
}
