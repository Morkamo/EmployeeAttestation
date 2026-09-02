using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

partial class HomeControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel pageLayout = null!; private Label titleLabel = null!; private Label subtitleLabel = null!;
    private TableLayoutPanel informationPanel = null!; private Label informationTitleLabel = null!; private Label informationTextLabel = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        pageLayout = new TableLayoutPanel(); titleLabel = new Label(); subtitleLabel = new Label(); informationPanel = new TableLayoutPanel();
        informationTitleLabel = new Label(); informationTextLabel = new Label(); pageLayout.SuspendLayout(); informationPanel.SuspendLayout(); SuspendLayout();
        pageLayout.ColumnCount = 1; pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); pageLayout.Controls.Add(titleLabel, 0, 0);
        pageLayout.Controls.Add(subtitleLabel, 0, 1); pageLayout.Controls.Add(informationPanel, 0, 2); pageLayout.Dock = DockStyle.Fill;
        pageLayout.Padding = new Padding(36); pageLayout.RowCount = 4; pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 190F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold); titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 6); titleLabel.Text = "Главная";
        subtitleLabel.AutoSize = true; subtitleLabel.Font = new Font("Segoe UI", 11F); subtitleLabel.ForeColor = AppColors.TextSecondary;
        subtitleLabel.Margin = new Padding(2, 0, 0, 30); subtitleLabel.Text = "Система аттестации сотрудников";
        informationPanel.BackColor = AppColors.Surface; informationPanel.BorderStyle = BorderStyle.FixedSingle;
        informationPanel.ColumnCount = 1; informationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        informationPanel.Controls.Add(informationTitleLabel, 0, 0); informationPanel.Controls.Add(informationTextLabel, 0, 1);
        informationPanel.Dock = DockStyle.Fill; informationPanel.Padding = new Padding(28); informationPanel.RowCount = 2;
        informationPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); informationPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        informationTitleLabel.AutoSize = true; informationTitleLabel.Font = new Font("Segoe UI Semibold", 15F); informationTitleLabel.ForeColor = AppColors.TextPrimary;
        informationTitleLabel.Margin = new Padding(0, 0, 0, 14); informationTitleLabel.Text = "Добро пожаловать";
        informationTextLabel.AutoSize = true; informationTextLabel.Dock = DockStyle.Top; informationTextLabel.Font = new Font("Segoe UI", 11F);
        informationTextLabel.ForeColor = AppColors.TextSecondary; informationTextLabel.MaximumSize = new Size(900, 0); informationTextLabel.Text = "Используйте разделы слева для работы с сотрудниками, аттестациями, комиссиями и справочниками организации.";
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background;
        Controls.Add(pageLayout); Font = new Font("Segoe UI", 9F); Name = "HomeControl"; Size = new Size(1340, 900);
        pageLayout.ResumeLayout(false); pageLayout.PerformLayout(); informationPanel.ResumeLayout(false); informationPanel.PerformLayout(); ResumeLayout(false);
    }
}
