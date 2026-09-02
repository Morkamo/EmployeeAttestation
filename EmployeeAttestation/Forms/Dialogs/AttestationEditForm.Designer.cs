using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class AttestationEditForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel formLayout = null!;
    private Label employeeLabel = null!;
    private ComboBox employeeComboBox = null!;
    private Label commissionLabel = null!;
    private ComboBox commissionComboBox = null!;
    private Label dateLabel = null!;
    private DateTimePicker attestationDatePicker = null!;
    private CheckBox evaluateManagerialCheckBox = null!;
    private FlowLayoutPanel statusPanel = null!;
    private Label statusLabel = null!;
    private Label statusValueLabel = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button saveDraftButton = null!;
    private Button scheduleButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        formLayout = new TableLayoutPanel();
        employeeLabel = new Label();
        employeeComboBox = new ComboBox();
        commissionLabel = new Label();
        commissionComboBox = new ComboBox();
        dateLabel = new Label();
        attestationDatePicker = new DateTimePicker();
        evaluateManagerialCheckBox = new CheckBox();
        statusPanel = new FlowLayoutPanel();
        statusLabel = new Label();
        statusValueLabel = new Label();
        buttonsPanel = new FlowLayoutPanel();
        saveDraftButton = new Button();
        scheduleButton = new Button();
        cancelButton = new Button();
        formLayout.SuspendLayout();
        statusPanel.SuspendLayout();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        //
        // formLayout
        //
        formLayout.ColumnCount = 1;
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        formLayout.Controls.Add(employeeLabel, 0, 0);
        formLayout.Controls.Add(employeeComboBox, 0, 1);
        formLayout.Controls.Add(commissionLabel, 0, 2);
        formLayout.Controls.Add(commissionComboBox, 0, 3);
        formLayout.Controls.Add(dateLabel, 0, 4);
        formLayout.Controls.Add(attestationDatePicker, 0, 5);
        formLayout.Controls.Add(evaluateManagerialCheckBox, 0, 6);
        formLayout.Controls.Add(statusPanel, 0, 7);
        formLayout.Controls.Add(buttonsPanel, 0, 9);
        formLayout.Dock = DockStyle.Fill;
        formLayout.Padding = new Padding(34, 28, 34, 24);
        formLayout.RowCount = 10;
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        //
        // fields
        //
        ConfigureFieldLabel(employeeLabel, "Сотрудник");
        ConfigureComboBox(employeeComboBox, 0);
        employeeComboBox.SelectedIndexChanged += EmployeeComboBox_SelectedIndexChanged;
        ConfigureFieldLabel(commissionLabel, "Комиссия");
        ConfigureComboBox(commissionComboBox, 1);
        ConfigureFieldLabel(dateLabel, "Дата аттестации");
        attestationDatePicker.CalendarFont = new Font("Segoe UI", 10F);
        attestationDatePicker.Dock = DockStyle.Top;
        attestationDatePicker.Font = new Font("Segoe UI", 11F);
        attestationDatePicker.Format = DateTimePickerFormat.Custom;
        attestationDatePicker.CustomFormat = "dd.MM.yyyy";
        attestationDatePicker.Margin = new Padding(0, 0, 0, 16);
        attestationDatePicker.ShowCheckBox = true;
        attestationDatePicker.TabIndex = 2;
        evaluateManagerialCheckBox.AutoSize = true;
        evaluateManagerialCheckBox.Font = new Font("Segoe UI", 10F);
        evaluateManagerialCheckBox.ForeColor = AppColors.TextPrimary;
        evaluateManagerialCheckBox.Margin = new Padding(0, 5, 0, 0);
        evaluateManagerialCheckBox.TabIndex = 3;
        evaluateManagerialCheckBox.Text = "Оценивать как руководителя";
        evaluateManagerialCheckBox.UseVisualStyleBackColor = true;
        //
        // statusPanel
        //
        statusPanel.Controls.Add(statusLabel);
        statusPanel.Controls.Add(statusValueLabel);
        statusPanel.Dock = DockStyle.Fill;
        statusPanel.Margin = new Padding(0);
        statusPanel.WrapContents = false;
        statusLabel.AutoSize = true;
        statusLabel.Font = new Font("Segoe UI Semibold", 10F);
        statusLabel.ForeColor = AppColors.TextSecondary;
        statusLabel.Margin = new Padding(0, 8, 6, 0);
        statusLabel.Text = "Статус:";
        statusValueLabel.AutoSize = true;
        statusValueLabel.Font = new Font("Segoe UI Semibold", 10F);
        statusValueLabel.ForeColor = AppColors.Primary;
        statusValueLabel.Margin = new Padding(0, 8, 0, 0);
        statusValueLabel.Text = "Черновик";
        //
        // buttonsPanel
        //
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(scheduleButton);
        buttonsPanel.Controls.Add(saveDraftButton);
        buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Margin = new Padding(0);
        buttonsPanel.WrapContents = false;
        ConfigureDialogButton(saveDraftButton, "Сохранить черновик", 4, 178);
        saveDraftButton.Click += SaveDraftButton_Click;
        ConfigureDialogButton(scheduleButton, "Запланировать", 5, 150);
        scheduleButton.Click += ScheduleButton_Click;
        ConfigureDialogButton(cancelButton, "Отмена", 6, 120);
        cancelButton.DialogResult = DialogResult.Cancel;
        //
        // AttestationEditForm
        //
        AcceptButton = scheduleButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Surface;
        CancelButton = cancelButton;
        ClientSize = new Size(720, 510);
        Controls.Add(formLayout);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AttestationEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Создать аттестацию";
        formLayout.ResumeLayout(false);
        formLayout.PerformLayout();
        statusPanel.ResumeLayout(false);
        statusPanel.PerformLayout();
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

    private static void ConfigureComboBox(ComboBox comboBox, int tabIndex)
    {
        comboBox.Dock = DockStyle.Top;
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.Font = new Font("Segoe UI", 11F);
        comboBox.IntegralHeight = false;
        comboBox.Margin = new Padding(0, 0, 0, 16);
        comboBox.TabIndex = tabIndex;
    }

    private static void ConfigureDialogButton(Button button, string text, int tabIndex, int width)
    {
        button.Cursor = Cursors.Hand;
        button.FlatStyle = FlatStyle.Flat;
        button.Margin = new Padding(10, 0, 0, 0);
        button.Size = new Size(width, 44);
        button.TabIndex = tabIndex;
        button.Text = text;
        button.UseVisualStyleBackColor = false;
    }
}
