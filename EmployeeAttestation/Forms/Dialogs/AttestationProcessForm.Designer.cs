using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class AttestationProcessForm
{
    private System.ComponentModel.IContainer components = null!; private TableLayoutPanel rootLayout = null!; private TabControl tabs = null!;
    private TabPage basicTab = null!; private TabPage evaluationTab = null!; private TabPage decisionTab = null!;
    private Label employeeValueLabel = null!; private Label departmentValueLabel = null!; private Label positionValueLabel = null!;
    private Label dateValueLabel = null!; private Label commissionValueLabel = null!; private Label statusValueLabel = null!; private Label managerialValueLabel = null!;
    private CheckedListBox presenceList = null!; private Panel scoresHostPanel = null!; private FlowLayoutPanel averagesPanel = null!;
    private Label professionalAverageLabel = null!; private Label personalAverageLabel = null!; private Label managerialAverageLabel = null!; private Label overallAverageLabel = null!;
    private FlowLayoutPanel evaluationButtons = null!; private Button saveScoresButton = null!; private Button transitionButton = null!;
    private TextBox decisionTextBox = null!; private TextBox recommendationsTextBox = null!; private DataGridView votesGrid = null!;
    private Label voteCountsLabel = null!; private FlowLayoutPanel decisionButtons = null!; private Button saveDecisionButton = null!; private Button completeButton = null!;
    private FlowLayoutPanel footer = null!; private Button closeButton = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        rootLayout = new TableLayoutPanel(); tabs = new TabControl(); basicTab = new TabPage(); evaluationTab = new TabPage(); decisionTab = new TabPage();
        employeeValueLabel = new Label(); departmentValueLabel = new Label(); positionValueLabel = new Label(); dateValueLabel = new Label();
        commissionValueLabel = new Label(); statusValueLabel = new Label(); managerialValueLabel = new Label(); presenceList = new CheckedListBox();
        scoresHostPanel = new Panel(); averagesPanel = new FlowLayoutPanel(); professionalAverageLabel = new Label(); personalAverageLabel = new Label();
        managerialAverageLabel = new Label(); overallAverageLabel = new Label(); evaluationButtons = new FlowLayoutPanel(); saveScoresButton = new Button();
        transitionButton = new Button(); decisionTextBox = new TextBox(); recommendationsTextBox = new TextBox(); votesGrid = new DataGridView();
        voteCountsLabel = new Label(); decisionButtons = new FlowLayoutPanel(); saveDecisionButton = new Button(); completeButton = new Button();
        footer = new FlowLayoutPanel(); closeButton = new Button(); SuspendLayout();
        rootLayout.ColumnCount = 1; rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(18); rootLayout.RowCount = 2; rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F)); tabs.Dock = DockStyle.Fill; tabs.Font = new Font("Segoe UI", 10F);
        tabs.TabPages.AddRange(basicTab, evaluationTab, decisionTab); rootLayout.Controls.Add(tabs, 0, 0);
        ConfigureBasicTab(); ConfigureEvaluationTab(); ConfigureDecisionTab();
        footer.Dock = DockStyle.Fill; footer.FlowDirection = FlowDirection.RightToLeft; footer.Controls.Add(closeButton);
        ConfigureButton(closeButton, "Закрыть", 120); closeButton.DialogResult = DialogResult.Cancel; rootLayout.Controls.Add(footer, 0, 1);
        CancelButton = closeButton; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = AppColors.Background; ClientSize = new Size(1260, 820);
        Controls.Add(rootLayout); MinimumSize = new Size(760, 500); Name = "AttestationProcessForm"; StartPosition = FormStartPosition.CenterParent;
        Text = "Проведение аттестации"; ResumeLayout(false);
    }

    private void ConfigureBasicTab()
    {
        basicTab.Text = "Основные данные"; basicTab.AutoScroll = true; basicTab.BackColor = AppColors.Surface; basicTab.Padding = new Padding(28);
        TableLayoutPanel panel = new() { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(12) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F)); panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        string[] labels = ["Сотрудник", "Подразделение", "Должность", "Дата", "Комиссия", "Статус", "Оценивать как руководителя"];
        Label[] values = [employeeValueLabel, departmentValueLabel, positionValueLabel, dateValueLabel, commissionValueLabel, statusValueLabel, managerialValueLabel];
        for (int i = 0; i < labels.Length; i++)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Label name = new() { Dock = DockStyle.Fill, Font = new Font("Segoe UI Semibold", 10F), ForeColor = AppColors.TextSecondary,
                AutoSize = true, Margin = new Padding(0, 12, 16, 12), Text = labels[i], TextAlign = ContentAlignment.MiddleLeft };
            values[i].Dock = DockStyle.Fill; values[i].Font = new Font("Segoe UI", 11F); values[i].ForeColor = AppColors.TextPrimary;
            values[i].AutoSize = true; values[i].Margin = new Padding(0, 10, 0, 10); values[i].TextAlign = ContentAlignment.MiddleLeft; panel.Controls.Add(name, 0, i); panel.Controls.Add(values[i], 1, i);
        }
        basicTab.Controls.Add(panel);
    }

    private void ConfigureEvaluationTab()
    {
        evaluationTab.Text = "Оценивание"; evaluationTab.AutoScroll = true; evaluationTab.BackColor = AppColors.Surface; evaluationTab.Padding = new Padding(18);
        TableLayoutPanel panel = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, MinimumSize = new Size(720, 500) };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 126F));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F)); panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Label presenceTitle = SectionLabel("Присутствующие члены комиссии"); panel.Controls.Add(presenceTitle, 0, 0);
        presenceList.Dock = DockStyle.Fill; presenceList.CheckOnClick = true; presenceList.Font = new Font("Segoe UI", 10F);
        presenceList.ItemCheck += PresenceList_ItemCheck; panel.Controls.Add(presenceList, 0, 1);
        Label scoresTitle = SectionLabel("Оценки"); scoresTitle.Margin = new Padding(0, 8, 0, 0); panel.Controls.Add(scoresTitle, 0, 2);
        scoresHostPanel.Dock = DockStyle.Fill; scoresHostPanel.AutoScroll = true; scoresHostPanel.BorderStyle = BorderStyle.FixedSingle; panel.Controls.Add(scoresHostPanel, 0, 3);
        averagesPanel.AutoScroll = true; averagesPanel.Dock = DockStyle.Fill; averagesPanel.WrapContents = true;
        Label[] averages = [professionalAverageLabel, personalAverageLabel, managerialAverageLabel, overallAverageLabel];
        foreach (Label label in averages) { label.AutoSize = true; label.Font = new Font("Segoe UI Semibold", 9.5F); label.ForeColor = AppColors.TextPrimary; label.Margin = new Padding(0, 14, 24, 0); averagesPanel.Controls.Add(label); }
        panel.Controls.Add(averagesPanel, 0, 4);
        evaluationButtons.AutoSize = true; evaluationButtons.Dock = DockStyle.Fill; evaluationButtons.FlowDirection = FlowDirection.RightToLeft;
        ConfigureButton(transitionButton, "Перейти к решению", 180); transitionButton.Click += TransitionButton_Click;
        ConfigureButton(saveScoresButton, "Сохранить", 130); saveScoresButton.Click += SaveScoresButton_Click;
        evaluationButtons.Controls.Add(transitionButton); evaluationButtons.Controls.Add(saveScoresButton); panel.Controls.Add(evaluationButtons, 0, 5);
        evaluationTab.Controls.Add(panel);
    }

    private void ConfigureDecisionTab()
    {
        decisionTab.Text = "Решение"; decisionTab.AutoScroll = true; decisionTab.BackColor = AppColors.Surface; decisionTab.Padding = new Padding(18);
        TableLayoutPanel panel = new() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, MinimumSize = new Size(720, 520) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(SectionLabel("Решение комиссии"), 0, 0); panel.Controls.Add(SectionLabel("Рекомендации"), 1, 0);
        ConfigureMultiline(decisionTextBox); ConfigureMultiline(recommendationsTextBox); panel.Controls.Add(decisionTextBox, 0, 1); panel.Controls.Add(recommendationsTextBox, 1, 1);
        Label votingTitle = SectionLabel("Голосование присутствующих членов комиссии"); panel.Controls.Add(votingTitle, 0, 2); panel.SetColumnSpan(votingTitle, 2);
        votesGrid.AllowUserToAddRows = false; votesGrid.AllowUserToDeleteRows = false; votesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        votesGrid.Dock = DockStyle.Fill; votesGrid.RowHeadersVisible = false; votesGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "member", HeaderText = "ФИО", FillWeight = 150F, MinimumWidth = 240, ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "role", HeaderText = "Роль", FillWeight = 100F, MinimumWidth = 180, ReadOnly = true },
            new DataGridViewComboBoxColumn { Name = "vote", HeaderText = "Голос", FillWeight = 75F, MinimumWidth = 150, FlatStyle = FlatStyle.Flat });
        votesGrid.ScrollBars = ScrollBars.Both;
        votesGrid.CellValueChanged += VotesGrid_CellValueChanged; panel.Controls.Add(votesGrid, 0, 3); panel.SetColumnSpan(votesGrid, 2);
        voteCountsLabel.AutoSize = true; voteCountsLabel.Font = new Font("Segoe UI Semibold", 10F); voteCountsLabel.ForeColor = AppColors.TextPrimary;
        voteCountsLabel.Margin = new Padding(0, 8, 0, 0); panel.Controls.Add(voteCountsLabel, 0, 4); panel.SetColumnSpan(voteCountsLabel, 2);
        decisionButtons.AutoSize = true; decisionButtons.Dock = DockStyle.Bottom; decisionButtons.FlowDirection = FlowDirection.RightToLeft; decisionButtons.WrapContents = true;
        ConfigureButton(completeButton, "Завершить аттестацию", 190); completeButton.Click += CompleteButton_Click;
        ConfigureButton(saveDecisionButton, "Сохранить", 130); saveDecisionButton.Click += SaveDecisionButton_Click;
        decisionButtons.Controls.Add(completeButton); decisionButtons.Controls.Add(saveDecisionButton); panel.Controls.Add(decisionButtons, 0, 6); panel.SetColumnSpan(decisionButtons, 2);
        decisionTab.Controls.Add(panel);
    }

    private static Label SectionLabel(string text) => new() { AutoSize = true, Font = new Font("Segoe UI Semibold", 10F), ForeColor = AppColors.TextPrimary, Text = text };
    private static void ConfigureMultiline(TextBox box) { box.Dock = DockStyle.Fill; box.Multiline = true; box.ScrollBars = ScrollBars.Vertical; box.Font = new Font("Segoe UI", 10F); box.Margin = new Padding(0, 0, 12, 8); }
    private static void ConfigureButton(Button button, string text, int width) { button.AutoSize = true; button.AutoSizeMode = AutoSizeMode.GrowAndShrink; button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(10, 6, 0, 4); button.MinimumSize = new Size(width, 42); button.Padding = new Padding(16, 0, 16, 0); button.Text = text; }
}
