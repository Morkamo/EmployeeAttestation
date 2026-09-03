using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class SettingsControl
{
    private System.ComponentModel.IContainer components = null!;
    private Panel scrollPanel = null!;
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
        scrollPanel = new Panel();
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
        scrollPanel.SuspendLayout();
        pageLayout.SuspendLayout();
        workspacePanel.SuspendLayout();
        databaseSettingsLayout.SuspendLayout();
        databaseActionsLayout.SuspendLayout();
        SuspendLayout();
        // 
        // scrollPanel
        // 
        scrollPanel.AutoScroll = true;
        scrollPanel.Controls.Add(pageLayout);
        scrollPanel.Dock = DockStyle.Fill;
        scrollPanel.Name = "scrollPanel";
        // 
        // pageLayout
        // 
        pageLayout.AutoSize = true;
        pageLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        pageLayout.ColumnCount = 1;
        pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(titleLabel, 0, 0);
        pageLayout.Controls.Add(subtitleLabel, 0, 1);
        pageLayout.Controls.Add(workspacePanel, 0, 2);
        pageLayout.Dock = DockStyle.Top;
        pageLayout.MinimumSize = new Size(680, 0);
        pageLayout.Name = "pageLayout";
        pageLayout.Padding = new Padding(36);
        pageLayout.RowCount = 3;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        // 
        // titleLabel
        // 
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold);
        titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 6);
        titleLabel.Text = "Настройки";
        // 
        // subtitleLabel
        // 
        subtitleLabel.AutoSize = true;
        subtitleLabel.Font = new Font("Segoe UI", 11F);
        subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 30);
        subtitleLabel.Text = "Параметры приложения.";
        // 
        // workspacePanel
        // 
        workspacePanel.BackColor = AppColors.Surface;
        workspacePanel.BorderStyle = BorderStyle.FixedSingle;
        workspacePanel.Controls.Add(databaseSettingsLayout);
        workspacePanel.Dock = DockStyle.Top;
        workspacePanel.Margin = new Padding(0);
        workspacePanel.MinimumSize = new Size(0, 180);
        workspacePanel.Name = "workspacePanel";
        workspacePanel.Padding = new Padding(22);
        workspacePanel.Size = new Size(900, 210);
        // 
        // databaseSettingsLayout
        // 
        databaseSettingsLayout.AutoSize = true;
        databaseSettingsLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        databaseSettingsLayout.ColumnCount = 1;
        databaseSettingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        databaseSettingsLayout.Controls.Add(databaseTitleLabel, 0, 0);
        databaseSettingsLayout.Controls.Add(databaseDescriptionLabel, 0, 1);
        databaseSettingsLayout.Controls.Add(databaseActionsLayout, 0, 2);
        databaseSettingsLayout.Dock = DockStyle.Top;
        databaseSettingsLayout.Name = "databaseSettingsLayout";
        databaseSettingsLayout.RowCount = 3;
        databaseSettingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        databaseSettingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        databaseSettingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        // 
        // databaseTitleLabel
        // 
        databaseTitleLabel.AutoSize = true;
        databaseTitleLabel.Font = new Font("Segoe UI Semibold", 15F);
        databaseTitleLabel.ForeColor = AppColors.TextPrimary;
        databaseTitleLabel.Margin = new Padding(0, 0, 0, 8);
        databaseTitleLabel.Text = "Рабочая база данных";
        // 
        // databaseDescriptionLabel
        // 
        databaseDescriptionLabel.AutoSize = true;
        databaseDescriptionLabel.Font = new Font("Segoe UI", 10F);
        databaseDescriptionLabel.ForeColor = AppColors.TextSecondary;
        databaseDescriptionLabel.Margin = new Padding(0, 0, 0, 14);
        databaseDescriptionLabel.Text = "Выберите совместимую базу из локальной папки databases.";
        // 
        // databaseActionsLayout
        // 
        databaseActionsLayout.AutoScroll = true;
        databaseActionsLayout.AutoSize = false;
        databaseActionsLayout.ColumnCount = 5;
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
        databaseActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        databaseActionsLayout.Controls.Add(databaseComboBox, 0, 0);
        databaseActionsLayout.Controls.Add(refreshButton, 2, 0);
        databaseActionsLayout.Controls.Add(openFolderButton, 4, 0);
        databaseActionsLayout.Dock = DockStyle.Top;
        databaseActionsLayout.Height = 48;
        databaseActionsLayout.MinimumSize = new Size(520, 48);
        databaseActionsLayout.Name = "databaseActionsLayout";
        databaseActionsLayout.RowCount = 1;
        databaseActionsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        databaseActionsLayout.Size = new Size(856, 48);
        databaseActionsLayout.TabIndex = 2;
        // 
        // databaseComboBox
        // 
        databaseComboBox.Dock = DockStyle.Fill;
        databaseComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        databaseComboBox.Font = new Font("Segoe UI", 11F);
        databaseComboBox.IntegralHeight = false;
        databaseComboBox.Margin = new Padding(0, 5, 0, 6);
        databaseComboBox.Name = "databaseComboBox";
        databaseComboBox.TabIndex = 0;
        databaseComboBox.SelectedIndexChanged += DatabaseComboBox_SelectedIndexChanged;
        // 
        // refreshButton
        // 
        refreshButton.Name = "refreshButton";
        refreshButton.TabIndex = 1;
        refreshButton.Click += RefreshButton_Click;
        // 
        // openFolderButton
        // 
        openFolderButton.Name = "openFolderButton";
        openFolderButton.TabIndex = 2;
        openFolderButton.Click += OpenFolderButton_Click;
        ConfigureActionButton(refreshButton, "Обновить");
        ConfigureActionButton(openFolderButton, "Открыть папку");
        // 
        // SettingsControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Background;
        Controls.Add(scrollPanel);
        Font = new Font("Segoe UI", 9F);
        Name = "SettingsControl";
        Size = new Size(1340, 900);
        scrollPanel.ResumeLayout(false);
        scrollPanel.PerformLayout();
        pageLayout.ResumeLayout(false);
        pageLayout.PerformLayout();
        workspacePanel.ResumeLayout(false);
        workspacePanel.PerformLayout();
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
        button.MinimumSize = new Size(100, 40);
        button.Padding = new Padding(8, 0, 8, 0);
        button.Text = text;
        button.UseVisualStyleBackColor = false;
        button.FlatAppearance.BorderColor = AppColors.Primary;
        button.FlatAppearance.BorderSize = 1;
    }
}
