using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class CommissionEditForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel formLayout = null!;
    private Label nameLabel = null!;
    private TextBox nameTextBox = null!;
    private Label descriptionLabel = null!;
    private TextBox descriptionTextBox = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        formLayout = new TableLayoutPanel(); nameLabel = new Label(); nameTextBox = new TextBox(); descriptionLabel = new Label();
        descriptionTextBox = new TextBox(); buttonsPanel = new FlowLayoutPanel(); saveButton = new Button(); cancelButton = new Button();
        formLayout.SuspendLayout(); buttonsPanel.SuspendLayout(); SuspendLayout();
        formLayout.ColumnCount = 1; formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        formLayout.Controls.Add(nameLabel, 0, 0); formLayout.Controls.Add(nameTextBox, 0, 1);
        formLayout.Controls.Add(descriptionLabel, 0, 2); formLayout.Controls.Add(descriptionTextBox, 0, 3);
        formLayout.Controls.Add(buttonsPanel, 0, 5); formLayout.Dock = DockStyle.Fill; formLayout.Padding = new Padding(30, 26, 30, 24);
        formLayout.RowCount = 6; formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145F));
        formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        ConfigureLabel(nameLabel, "Наименование"); ConfigureLabel(descriptionLabel, "Описание");
        nameTextBox.BorderStyle = BorderStyle.FixedSingle; nameTextBox.Dock = DockStyle.Top; nameTextBox.Font = new Font("Segoe UI", 11F);
        nameTextBox.Margin = new Padding(0, 0, 0, 18); nameTextBox.TabIndex = 0;
        descriptionTextBox.BorderStyle = BorderStyle.FixedSingle; descriptionTextBox.Dock = DockStyle.Fill;
        descriptionTextBox.Font = new Font("Segoe UI", 10F); descriptionTextBox.Margin = new Padding(0, 0, 0, 18);
        descriptionTextBox.Multiline = true; descriptionTextBox.ScrollBars = ScrollBars.Vertical; descriptionTextBox.TabIndex = 1;
        buttonsPanel.Controls.Add(cancelButton); buttonsPanel.Controls.Add(saveButton); buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft; buttonsPanel.Margin = new Padding(0); buttonsPanel.WrapContents = false;
        ConfigureButton(saveButton, "Сохранить", 2); saveButton.Click += SaveButton_Click;
        ConfigureButton(cancelButton, "Отмена", 3); cancelButton.DialogResult = DialogResult.Cancel;
        AcceptButton = saveButton; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Surface; CancelButton = cancelButton; ClientSize = new Size(650, 420); Controls.Add(formLayout);
        Font = new Font("Segoe UI", 9F); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        Name = "CommissionEditForm"; ShowInTaskbar = false; StartPosition = FormStartPosition.CenterParent; Text = "Добавить комиссию";
        formLayout.ResumeLayout(false); formLayout.PerformLayout(); buttonsPanel.ResumeLayout(false); ResumeLayout(false);
    }

    private static void ConfigureLabel(Label label, string text)
    {
        label.AutoSize = true; label.Font = new Font("Segoe UI Semibold", 10F); label.ForeColor = AppColors.TextPrimary;
        label.Margin = new Padding(0, 0, 0, 7); label.Text = text;
    }

    private static void ConfigureButton(Button button, string text, int tabIndex)
    {
        button.Cursor = Cursors.Hand; button.FlatStyle = FlatStyle.Flat; button.Margin = new Padding(10, 0, 0, 0);
        button.Size = new Size(130, 44); button.TabIndex = tabIndex; button.Text = text; button.UseVisualStyleBackColor = false;
    }
}
