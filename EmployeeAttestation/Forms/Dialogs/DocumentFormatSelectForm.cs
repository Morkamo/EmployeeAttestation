using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class DocumentFormatSelectForm : Form
{
    public DocumentFormatSelectForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(docxButton);
        AppControlStyles.ApplySecondaryButton(pdfButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        LoadWindowIcon();
    }

    public AttestationDocumentFormat SelectedFormat { get; private set; }

    private void DocxButton_Click(object? sender, EventArgs e) => SelectFormat(AttestationDocumentFormat.Docx);

    private void PdfButton_Click(object? sender, EventArgs e) => SelectFormat(AttestationDocumentFormat.Pdf);

    private void SelectFormat(AttestationDocumentFormat format)
    {
        SelectedFormat = format;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void LoadWindowIcon()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico");
        if (File.Exists(path)) Icon = new Icon(path);
    }
}

public enum AttestationDocumentFormat
{
    None,
    Docx,
    Pdf
}
