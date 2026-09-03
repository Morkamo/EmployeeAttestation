using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class DepartmentEditForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel formLayout = null!;
    private Label codeLabel = null!;
    private TextBox codeTextBox = null!;
    private Label nameLabel = null!;
    private TextBox nameTextBox = null!;
    private Label documentNameLabel = null!;
    private TextBox documentNameTextBox = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        formLayout = new TableLayoutPanel();
        codeLabel = new Label();
        codeTextBox = new TextBox();
        nameLabel = new Label();
        nameTextBox = new TextBox();
        documentNameLabel = new Label();
        documentNameTextBox = new TextBox();
        buttonsPanel = new FlowLayoutPanel();
        saveButton = new Button();
        cancelButton = new Button();
        formLayout.SuspendLayout();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // formLayout
        // 
        formLayout.ColumnCount = 1;
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        formLayout.Controls.Add(codeLabel, 0, 0);
        formLayout.Controls.Add(codeTextBox, 0, 1);
        formLayout.Controls.Add(nameLabel, 0, 2);
        formLayout.Controls.Add(nameTextBox, 0, 3);
        formLayout.Controls.Add(documentNameLabel, 0, 4);
        formLayout.Controls.Add(documentNameTextBox, 0, 5);
        formLayout.Controls.Add(buttonsPanel, 0, 7);
        formLayout.Dock = DockStyle.Fill;
        formLayout.Location = new Point(0, 0);
        formLayout.Name = "formLayout";
        formLayout.Padding = new Padding(30, 26, 30, 24);
        formLayout.RowCount = 8;
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.Size = new Size(600, 400);
        formLayout.TabIndex = 0;
        // 
        // codeLabel
        // 
        ConfigureFieldLabel(codeLabel, "Код");
        // 
        // codeTextBox
        // 
        ConfigureTextBox(codeTextBox, 0);
        // 
        // nameLabel
        // 
        ConfigureFieldLabel(nameLabel, "Наименование");
        // 
        // nameTextBox
        // 
        ConfigureTextBox(nameTextBox, 1);
        // 
        // documentNameLabel
        // 
        ConfigureFieldLabel(documentNameLabel, "Наименование в документах");
        // 
        // documentNameTextBox
        // 
        ConfigureTextBox(documentNameTextBox, 2);
        // 
        // buttonsPanel
        // 
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.AutoSize = true;
        buttonsPanel.Dock = DockStyle.Top;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Location = new Point(30, 328);
        buttonsPanel.Margin = new Padding(0);
        buttonsPanel.Name = "buttonsPanel";
        buttonsPanel.Size = new Size(540, 48);
        buttonsPanel.TabIndex = 3;
        buttonsPanel.WrapContents = true;
        // 
        // saveButton
        // 
        ConfigureDialogButton(saveButton, "Сохранить", 4);
        saveButton.Click += SaveButton_Click;
        // 
        // cancelButton
        // 
        ConfigureDialogButton(cancelButton, "Отмена", 5);
        cancelButton.DialogResult = DialogResult.Cancel;
        // 
        // DepartmentEditForm
        // 
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Surface;
        CancelButton = cancelButton;
        ClientSize = new Size(600, 400);
        Controls.Add(formLayout);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "DepartmentEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Добавить подразделение";
        formLayout.ResumeLayout(false);
        formLayout.PerformLayout();
        buttonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private static void ConfigureFieldLabel(Label label, string text)
    {
        label.AutoSize = true;
        label.Font = new Font("Segoe UI Semibold", 10F);
        label.ForeColor = AppColors.TextPrimary;
        label.Margin = new Padding(0, 0, 0, 7);
        label.Text = text;
    }

    private static void ConfigureTextBox(TextBox textBox, int tabIndex)
    {
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Dock = DockStyle.Top;
        textBox.Font = new Font("Segoe UI", 11F);
        textBox.Margin = new Padding(0, 0, 0, 18);
        textBox.Size = new Size(540, 32);
        textBox.TabIndex = tabIndex;
    }

    private static void ConfigureDialogButton(Button button, string text, int tabIndex)
    {
        button.AutoSize = true;
        button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button.Cursor = Cursors.Hand;
        button.FlatStyle = FlatStyle.Flat;
        button.Margin = new Padding(10, 0, 0, 0);
        button.MinimumSize = new Size(130, 44);
        button.Padding = new Padding(16, 0, 16, 0);
        button.TabIndex = tabIndex;
        button.Text = text;
        button.UseVisualStyleBackColor = false;
    }
}
