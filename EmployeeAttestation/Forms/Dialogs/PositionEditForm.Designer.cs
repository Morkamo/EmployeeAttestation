using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class PositionEditForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel formLayout = null!;
    private Label nameLabel = null!;
    private TextBox nameTextBox = null!;
    private CheckBox managerialCheckBox = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        formLayout = new TableLayoutPanel();
        nameLabel = new Label();
        nameTextBox = new TextBox();
        managerialCheckBox = new CheckBox();
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
        formLayout.Controls.Add(nameLabel, 0, 0);
        formLayout.Controls.Add(nameTextBox, 0, 1);
        formLayout.Controls.Add(managerialCheckBox, 0, 2);
        formLayout.Controls.Add(buttonsPanel, 0, 4);
        formLayout.Dock = DockStyle.Fill;
        formLayout.Padding = new Padding(30, 28, 30, 24);
        formLayout.RowCount = 5;
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        // 
        // nameLabel
        // 
        nameLabel.AutoSize = true;
        nameLabel.Font = new Font("Segoe UI Semibold", 10F);
        nameLabel.ForeColor = AppColors.TextPrimary;
        nameLabel.Margin = new Padding(0, 0, 0, 7);
        nameLabel.Text = "Наименование";
        // 
        // nameTextBox
        // 
        nameTextBox.BorderStyle = BorderStyle.FixedSingle;
        nameTextBox.Dock = DockStyle.Top;
        nameTextBox.Font = new Font("Segoe UI", 11F);
        nameTextBox.Margin = new Padding(0, 0, 0, 18);
        nameTextBox.TabIndex = 0;
        // 
        // managerialCheckBox
        // 
        managerialCheckBox.AutoSize = true;
        managerialCheckBox.Font = new Font("Segoe UI", 10F);
        managerialCheckBox.ForeColor = AppColors.TextPrimary;
        managerialCheckBox.Margin = new Padding(0, 4, 0, 0);
        managerialCheckBox.TabIndex = 1;
        managerialCheckBox.Text = "Руководящая должность";
        managerialCheckBox.UseVisualStyleBackColor = true;
        // 
        // buttonsPanel
        // 
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Margin = new Padding(0);
        buttonsPanel.WrapContents = false;
        // 
        // saveButton
        // 
        ConfigureDialogButton(saveButton, "Сохранить", 2);
        saveButton.Click += SaveButton_Click;
        // 
        // cancelButton
        // 
        ConfigureDialogButton(cancelButton, "Отмена", 3);
        cancelButton.DialogResult = DialogResult.Cancel;
        // 
        // PositionEditForm
        // 
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Surface;
        CancelButton = cancelButton;
        ClientSize = new Size(550, 300);
        Controls.Add(formLayout);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PositionEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Добавить должность";
        formLayout.ResumeLayout(false);
        formLayout.PerformLayout();
        buttonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private static void ConfigureDialogButton(Button button, string text, int tabIndex)
    {
        button.Cursor = Cursors.Hand;
        button.FlatStyle = FlatStyle.Flat;
        button.Margin = new Padding(10, 0, 0, 0);
        button.Size = new Size(130, 44);
        button.TabIndex = tabIndex;
        button.Text = text;
        button.UseVisualStyleBackColor = false;
    }
}
