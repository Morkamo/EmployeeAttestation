using EmployeeAttestation.Events;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls;

partial class SidebarControl
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel navigationPanel = null!;
    private Label sectionTitleLabel = null!;
    private Panel separatorPanel = null!;
    private Button homeButton = null!;
    private Button employeesButton = null!;
    private Button attestationsButton = null!;
    private Button commissionsButton = null!;
    private Button departmentsButton = null!;
    private Button positionsButton = null!;
    private Button settingsButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            placeholderIcon?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        navigationPanel = new TableLayoutPanel();
        separatorPanel = new Panel();
        sectionTitleLabel = new Label();
        homeButton = CreateNavigationButton(PageType.Home);
        employeesButton = CreateNavigationButton(PageType.Employees);
        attestationsButton = CreateNavigationButton(PageType.Attestations);
        commissionsButton = CreateNavigationButton(PageType.Commissions);
        departmentsButton = CreateNavigationButton(PageType.Departments);
        positionsButton = CreateNavigationButton(PageType.Positions);
        settingsButton = CreateNavigationButton(PageType.Settings);
        SuspendLayout();
        // 
        // navigationPanel
        // 
        navigationPanel.Dock = DockStyle.Fill;
        navigationPanel.ColumnCount = 1;
        navigationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        navigationPanel.Location = new Point(0, 76);
        navigationPanel.Name = "navigationPanel";
        navigationPanel.Padding = new Padding(12, 10, 12, 10);
        navigationPanel.RowCount = 8;
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        navigationPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        navigationPanel.Size = new Size(259, 824);
        navigationPanel.TabIndex = 1;
        navigationPanel.Controls.Add(homeButton, 0, 0);
        navigationPanel.Controls.Add(employeesButton, 0, 1);
        navigationPanel.Controls.Add(attestationsButton, 0, 2);
        navigationPanel.Controls.Add(commissionsButton, 0, 3);
        navigationPanel.Controls.Add(departmentsButton, 0, 4);
        navigationPanel.Controls.Add(positionsButton, 0, 5);
        navigationPanel.Controls.Add(settingsButton, 0, 6);
        // 
        // homeButton
        // 
        homeButton.Dock = DockStyle.Fill;
        // 
        // employeesButton
        // 
        employeesButton.Dock = DockStyle.Fill;
        // 
        // attestationsButton
        // 
        attestationsButton.Dock = DockStyle.Fill;
        // 
        // commissionsButton
        // 
        commissionsButton.Dock = DockStyle.Fill;
        // 
        // departmentsButton
        // 
        departmentsButton.Dock = DockStyle.Fill;
        // 
        // positionsButton
        // 
        positionsButton.Dock = DockStyle.Fill;
        // 
        // settingsButton
        // 
        settingsButton.Dock = DockStyle.Fill;
        // 
        // separatorPanel
        // 
        separatorPanel.Dock = DockStyle.Right;
        separatorPanel.BackColor = AppColors.Border;
        separatorPanel.Location = new Point(259, 0);
        separatorPanel.Name = "separatorPanel";
        separatorPanel.Size = new Size(1, 900);
        separatorPanel.TabIndex = 2;
        // 
        // sectionTitleLabel
        // 
        sectionTitleLabel.Dock = DockStyle.Top;
        sectionTitleLabel.Font = new Font("Segoe UI Semibold", 17F);
        sectionTitleLabel.Location = new Point(0, 0);
        sectionTitleLabel.Name = "sectionTitleLabel";
        sectionTitleLabel.Padding = new Padding(24, 0, 0, 0);
        sectionTitleLabel.Size = new Size(259, 76);
        sectionTitleLabel.TabIndex = 0;
        sectionTitleLabel.Text = "Главная";
        sectionTitleLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // SidebarControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(navigationPanel);
        Controls.Add(sectionTitleLabel);
        Controls.Add(separatorPanel);
        Font = new Font("Segoe UI", 10F);
        Name = "SidebarControl";
        Size = new Size(260, 900);
        ResumeLayout(false);
    }

    private Button CreateNavigationButton(PageType pageType)
    {
        Button button = new()
        {
            BackColor = AppColors.Surface,
            Cursor = Cursors.Hand,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10.5F),
            ForeColor = AppColors.TextPrimary,
            ImageAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 0, 0, 4),
            Padding = new Padding(16, 0, 12, 0),
            Size = new Size(235, 54),
            TabIndex = (int)pageType,
            Tag = pageType,
            Text = GetPageTitle(pageType),
            TextAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderColor = AppColors.Border;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseDownBackColor = AppColors.ActiveBackground;
        button.FlatAppearance.MouseOverBackColor = AppColors.ActiveBackground;
        button.Click += navigationButton_Click;
        return button;
    }
}
