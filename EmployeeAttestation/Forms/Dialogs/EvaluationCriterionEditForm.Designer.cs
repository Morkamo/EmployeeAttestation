using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class EvaluationCriterionEditForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel formLayout = null!;
    private TextBox nameTextBox = null!;
    private ComboBox categoryComboBox = null!;
    private NumericUpDown minimumScoreNumeric = null!;
    private NumericUpDown maximumScoreNumeric = null!;
    private CheckBox managersOnlyCheckBox = null!;
    private NumericUpDown sortOrderNumeric = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        formLayout = new TableLayoutPanel(); nameTextBox = new TextBox(); categoryComboBox = new ComboBox();
        minimumScoreNumeric = new NumericUpDown(); maximumScoreNumeric = new NumericUpDown();
        managersOnlyCheckBox = new CheckBox(); sortOrderNumeric = new NumericUpDown(); buttonsPanel = new FlowLayoutPanel();
        saveButton = new Button(); cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)minimumScoreNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)maximumScoreNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sortOrderNumeric).BeginInit();
        SuspendLayout();
        formLayout.ColumnCount = 2;
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        formLayout.Dock = DockStyle.Fill;
        formLayout.Padding = new Padding(30, 24, 30, 22);
        formLayout.RowCount = 9;
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F)); formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        AddLabel(formLayout, "Наименование", 0, 0, 2);
        ConfigureTextBox(nameTextBox); formLayout.Controls.Add(nameTextBox, 0, 1); formLayout.SetColumnSpan(nameTextBox, 2);
        AddLabel(formLayout, "Категория", 0, 2, 1); AddLabel(formLayout, "Порядок", 1, 2, 1);
        ConfigureComboBox(categoryComboBox); formLayout.Controls.Add(categoryComboBox, 0, 3);
        ConfigureNumeric(sortOrderNumeric, 0, 10000); formLayout.Controls.Add(sortOrderNumeric, 1, 3);
        AddLabel(formLayout, "Минимальный балл", 0, 4, 1); AddLabel(formLayout, "Максимальный балл", 1, 4, 1);
        ConfigureNumeric(minimumScoreNumeric, 1, 5); minimumScoreNumeric.Value = 2; formLayout.Controls.Add(minimumScoreNumeric, 0, 5);
        ConfigureNumeric(maximumScoreNumeric, 1, 5); maximumScoreNumeric.Value = 5; formLayout.Controls.Add(maximumScoreNumeric, 1, 5);
        managersOnlyCheckBox.AutoSize = true; managersOnlyCheckBox.Font = new Font("Segoe UI", 10F);
        managersOnlyCheckBox.ForeColor = AppColors.TextPrimary; managersOnlyCheckBox.Margin = new Padding(0, 8, 0, 0);
        managersOnlyCheckBox.Text = "Только для руководителей"; formLayout.Controls.Add(managersOnlyCheckBox, 0, 6);
        formLayout.SetColumnSpan(managersOnlyCheckBox, 2);
        buttonsPanel.Dock = DockStyle.Fill; buttonsPanel.FlowDirection = FlowDirection.RightToLeft; buttonsPanel.WrapContents = false;
        buttonsPanel.Controls.Add(cancelButton); buttonsPanel.Controls.Add(saveButton); formLayout.Controls.Add(buttonsPanel, 0, 8);
        formLayout.SetColumnSpan(buttonsPanel, 2);
        ConfigureButton(saveButton, "Сохранить", 130); saveButton.Click += SaveButton_Click;
        ConfigureButton(cancelButton, "Отмена", 120); cancelButton.DialogResult = DialogResult.Cancel;
        AcceptButton = saveButton; CancelButton = cancelButton; AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Surface; ClientSize = new Size(660, 440); Controls.Add(formLayout);
        FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        Name = "EvaluationCriterionEditForm"; ShowInTaskbar = false; StartPosition = FormStartPosition.CenterParent;
        Text = "Добавить критерий";
        ((System.ComponentModel.ISupportInitialize)minimumScoreNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)maximumScoreNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)sortOrderNumeric).EndInit();
        ResumeLayout(false);
    }

    private static void AddLabel(TableLayoutPanel panel, string text, int column, int row, int span)
    {
        Label label = new() { AutoSize = true, Font = new Font("Segoe UI Semibold", 10F), ForeColor = AppColors.TextPrimary,
            Margin = new Padding(0, 0, 10, 7), Text = text };
        panel.Controls.Add(label, column, row); panel.SetColumnSpan(label, span);
    }
    private static void ConfigureTextBox(TextBox control) { control.BorderStyle = BorderStyle.FixedSingle; control.Dock = DockStyle.Top; control.Font = new Font("Segoe UI", 11F); control.Margin = new Padding(0, 0, 10, 16); }
    private static void ConfigureComboBox(ComboBox control) { control.Dock = DockStyle.Top; control.DropDownStyle = ComboBoxStyle.DropDownList; control.Font = new Font("Segoe UI", 11F); control.Margin = new Padding(0, 0, 10, 16); }
    private static void ConfigureNumeric(NumericUpDown control, int min, int max) { control.Dock = DockStyle.Top; control.Font = new Font("Segoe UI", 11F); control.Minimum = min; control.Maximum = max; control.Margin = new Padding(0, 0, 10, 16); }
    private static void ConfigureButton(Button button, string text, int width) { button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(10, 0, 0, 0); button.Size = new Size(width, 44); button.Text = text; }
}
