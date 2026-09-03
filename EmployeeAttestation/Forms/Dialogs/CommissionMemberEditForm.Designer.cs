using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class CommissionMemberEditForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel formLayout = null!;
    private Label lastNameLabel = null!;
    private TextBox lastNameTextBox = null!;
    private Label firstNameLabel = null!;
    private TextBox firstNameTextBox = null!;
    private Label middleNameLabel = null!;
    private TextBox middleNameTextBox = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        formLayout = new TableLayoutPanel(); lastNameLabel = new Label(); lastNameTextBox = new TextBox();
        firstNameLabel = new Label(); firstNameTextBox = new TextBox(); middleNameLabel = new Label();
        middleNameTextBox = new TextBox(); buttonsPanel = new FlowLayoutPanel(); saveButton = new Button(); cancelButton = new Button();
        formLayout.SuspendLayout(); buttonsPanel.SuspendLayout(); SuspendLayout();
        formLayout.ColumnCount = 1; formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        formLayout.Controls.Add(lastNameLabel, 0, 0); formLayout.Controls.Add(lastNameTextBox, 0, 1);
        formLayout.Controls.Add(firstNameLabel, 0, 2); formLayout.Controls.Add(firstNameTextBox, 0, 3);
        formLayout.Controls.Add(middleNameLabel, 0, 4); formLayout.Controls.Add(middleNameTextBox, 0, 5);
        formLayout.Controls.Add(buttonsPanel, 0, 7); formLayout.Dock = DockStyle.Fill; formLayout.Padding = new Padding(30, 26, 30, 24);
        formLayout.RowCount = 8; formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        ConfigureField(lastNameLabel, lastNameTextBox, "Фамилия", 0); ConfigureField(firstNameLabel, firstNameTextBox, "Имя", 1);
        ConfigureField(middleNameLabel, middleNameTextBox, "Отчество", 2);
        buttonsPanel.Controls.Add(cancelButton); buttonsPanel.Controls.Add(saveButton); buttonsPanel.AutoSize = true; buttonsPanel.Dock = DockStyle.Top;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft; buttonsPanel.Margin = new Padding(0); buttonsPanel.WrapContents = true;
        ConfigureButton(saveButton, "Сохранить", 3); saveButton.Click += SaveButton_Click;
        ConfigureButton(cancelButton, "Отмена", 4); cancelButton.DialogResult = DialogResult.Cancel;
        AcceptButton = saveButton; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Surface; CancelButton = cancelButton; ClientSize = new Size(600, 400); Controls.Add(formLayout);
        Font = new Font("Segoe UI", 9F); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        Name = "CommissionMemberEditForm"; ShowInTaskbar = false; StartPosition = FormStartPosition.CenterParent; Text = "Добавить члена комиссии";
        formLayout.ResumeLayout(false); formLayout.PerformLayout(); buttonsPanel.ResumeLayout(false); ResumeLayout(false);
    }

    private static void ConfigureField(Label label, TextBox textBox, string text, int tabIndex)
    {
        label.AutoSize = true; label.Font = new Font("Segoe UI Semibold", 10F); label.ForeColor = AppColors.TextPrimary;
        label.Margin = new Padding(0, 0, 0, 7); label.Text = text; textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Dock = DockStyle.Top; textBox.Font = new Font("Segoe UI", 11F); textBox.Margin = new Padding(0, 0, 0, 18); textBox.TabIndex = tabIndex;
    }

    private static void ConfigureButton(Button button, string text, int tabIndex)
    {
        button.AutoSize = true; button.AutoSizeMode = AutoSizeMode.GrowAndShrink; button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(10, 0, 0, 0);
        button.MinimumSize = new Size(130, 44); button.Padding = new Padding(16, 0, 16, 0); button.TabIndex = tabIndex; button.Text = text; button.UseVisualStyleBackColor = false;
    }
}
