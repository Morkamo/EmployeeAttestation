using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class EmployeeEditForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel formLayout = null!;
    private Label lastNameLabel = null!;
    private TextBox lastNameTextBox = null!;
    private Label firstNameLabel = null!;
    private TextBox firstNameTextBox = null!;
    private Label middleNameLabel = null!;
    private TextBox middleNameTextBox = null!;
    private Label departmentLabel = null!;
    private ComboBox departmentComboBox = null!;
    private Label positionLabel = null!;
    private ComboBox positionComboBox = null!;
    private CheckBox managerCheckBox = null!;
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
        lastNameLabel = new Label();
        lastNameTextBox = new TextBox();
        firstNameLabel = new Label();
        firstNameTextBox = new TextBox();
        middleNameLabel = new Label();
        middleNameTextBox = new TextBox();
        departmentLabel = new Label();
        departmentComboBox = new ComboBox();
        positionLabel = new Label();
        positionComboBox = new ComboBox();
        managerCheckBox = new CheckBox();
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
        formLayout.Controls.Add(lastNameLabel, 0, 0);
        formLayout.Controls.Add(lastNameTextBox, 0, 1);
        formLayout.Controls.Add(firstNameLabel, 0, 2);
        formLayout.Controls.Add(firstNameTextBox, 0, 3);
        formLayout.Controls.Add(middleNameLabel, 0, 4);
        formLayout.Controls.Add(middleNameTextBox, 0, 5);
        formLayout.Controls.Add(departmentLabel, 0, 6);
        formLayout.Controls.Add(departmentComboBox, 0, 7);
        formLayout.Controls.Add(positionLabel, 0, 8);
        formLayout.Controls.Add(positionComboBox, 0, 9);
        formLayout.Controls.Add(managerCheckBox, 0, 10);
        formLayout.Controls.Add(buttonsPanel, 0, 12);
        formLayout.Dock = DockStyle.Fill;
        formLayout.Padding = new Padding(34, 28, 34, 24);
        formLayout.RowCount = 13;
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        // 
        // labels and fields
        // 
        ConfigureFieldLabel(lastNameLabel, "Фамилия");
        ConfigureTextBox(lastNameTextBox, 0);
        ConfigureFieldLabel(firstNameLabel, "Имя");
        ConfigureTextBox(firstNameTextBox, 1);
        ConfigureFieldLabel(middleNameLabel, "Отчество");
        ConfigureTextBox(middleNameTextBox, 2);
        ConfigureFieldLabel(departmentLabel, "Подразделение");
        ConfigureComboBox(departmentComboBox, 3);
        ConfigureFieldLabel(positionLabel, "Должность");
        ConfigureComboBox(positionComboBox, 4);
        positionComboBox.SelectedIndexChanged += PositionComboBox_SelectedIndexChanged;
        // 
        // managerCheckBox
        // 
        managerCheckBox.AutoSize = true;
        managerCheckBox.Font = new Font("Segoe UI", 10F);
        managerCheckBox.ForeColor = AppColors.TextPrimary;
        managerCheckBox.Margin = new Padding(0, 5, 0, 0);
        managerCheckBox.TabIndex = 5;
        managerCheckBox.Text = "Оценивать как руководителя";
        managerCheckBox.UseVisualStyleBackColor = true;
        // 
        // buttonsPanel
        // 
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.AutoSize = true;
        buttonsPanel.Dock = DockStyle.Top;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Margin = new Padding(0);
        buttonsPanel.WrapContents = true;
        ConfigureDialogButton(saveButton, "Сохранить", 6);
        saveButton.Click += SaveButton_Click;
        ConfigureDialogButton(cancelButton, "Отмена", 7);
        cancelButton.DialogResult = DialogResult.Cancel;
        // 
        // EmployeeEditForm
        // 
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Surface;
        CancelButton = cancelButton;
        ClientSize = new Size(720, 620);
        Controls.Add(formLayout);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "EmployeeEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Добавить сотрудника";
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
        textBox.Margin = new Padding(0, 0, 0, 16);
        textBox.TabIndex = tabIndex;
    }

    private static void ConfigureComboBox(ComboBox comboBox, int tabIndex)
    {
        comboBox.Dock = DockStyle.Top;
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.Font = new Font("Segoe UI", 11F);
        comboBox.IntegralHeight = false;
        comboBox.Margin = new Padding(0, 0, 0, 16);
        comboBox.TabIndex = tabIndex;
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
