using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class CommissionEditForm : Form
{
    private readonly CommissionService? commissionService;
    private readonly int commissionId;
    private readonly bool isArchived;

    public CommissionEditForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(saveButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        LoadWindowIcon();
    }

    public CommissionEditForm(CommissionService commissionService, Commission? commission = null)
        : this()
    {
        this.commissionService = commissionService ?? throw new ArgumentNullException(nameof(commissionService));
        commissionId = commission?.Id ?? 0;
        isArchived = commission?.IsArchived ?? false;
        Text = commission is null ? "Добавить комиссию" : "Изменить комиссию";
        if (commission is null) return;
        nameTextBox.Text = commission.Name;
        descriptionTextBox.Text = commission.Description ?? string.Empty;
    }

    private void SaveButton_Click(object? sender, EventArgs e) => SaveCommission();

    private void SaveCommission()
    {
        if (commissionService is null) return;
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show(this, "Введите наименование комиссии.", "Комиссии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            nameTextBox.Focus();
            return;
        }
        Commission commission = new()
        {
            Id = commissionId,
            Name = nameTextBox.Text,
            Description = descriptionTextBox.Text,
            IsArchived = isArchived
        };
        try
        {
            if (commissionId == 0) commissionService.Create(commission);
            else commissionService.Update(commission);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) when (exception is CommissionServiceException or ArgumentException)
        {
            MessageBox.Show(this, exception.Message, "Комиссии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico");
        if (File.Exists(iconPath)) Icon = new Icon(iconPath);
    }
}
