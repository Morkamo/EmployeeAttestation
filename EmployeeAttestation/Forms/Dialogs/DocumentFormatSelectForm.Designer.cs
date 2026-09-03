using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

partial class DocumentFormatSelectForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel rootLayout = null!;
    private Label titleLabel = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button docxButton = null!;
    private Button pdfButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        rootLayout = new TableLayoutPanel();
        titleLabel = new Label();
        buttonsPanel = new FlowLayoutPanel();
        docxButton = new Button();
        pdfButton = new Button();
        cancelButton = new Button();
        rootLayout.SuspendLayout();
        buttonsPanel.SuspendLayout();
        SuspendLayout();

        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(buttonsPanel, 0, 1);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(24);
        rootLayout.RowCount = 2;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        titleLabel.AutoSize = true;
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI Semibold", 14F);
        titleLabel.ForeColor = AppColors.TextPrimary;
        titleLabel.Margin = new Padding(0, 0, 0, 20);
        titleLabel.Text = "Выберите формат документа";

        buttonsPanel.AutoSize = true;
        buttonsPanel.Controls.Add(docxButton);
        buttonsPanel.Controls.Add(pdfButton);
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Dock = DockStyle.Top;
        buttonsPanel.WrapContents = false;

        ConfigureButton(docxButton, "DOCX", 105);
        docxButton.Click += DocxButton_Click;
        ConfigureButton(pdfButton, "PDF", 105);
        pdfButton.Click += PdfButton_Click;
        ConfigureButton(cancelButton, "Отмена", 105);
        cancelButton.DialogResult = DialogResult.Cancel;

        AcceptButton = docxButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = AppColors.Background;
        CancelButton = cancelButton;
        ClientSize = new Size(405, 155);
        Controls.Add(rootLayout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "DocumentFormatSelectForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Сохранение документа";

        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        buttonsPanel.ResumeLayout(false);
        buttonsPanel.PerformLayout();
        ResumeLayout(false);
    }

    private static void ConfigureButton(Button button, string text, int width)
    {
        button.AutoSize = true;
        button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button.FlatStyle = FlatStyle.Flat;
        button.Margin = new Padding(0, 0, 10, 0);
        button.MinimumSize = new Size(width, 42);
        button.Padding = new Padding(14, 0, 14, 0);
        button.Text = text;
    }
}
