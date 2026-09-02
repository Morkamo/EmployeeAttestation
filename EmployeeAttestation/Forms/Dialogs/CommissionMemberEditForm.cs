using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class CommissionMemberEditForm : Form
{
    private readonly CommissionMemberService? memberService;
    private readonly int memberId;
    private readonly bool isArchived;

    public CommissionMemberEditForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(saveButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        LoadWindowIcon();
    }

    public CommissionMemberEditForm(CommissionMemberService memberService, CommissionMember? member = null)
        : this()
    {
        this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        memberId = member?.Id ?? 0;
        isArchived = member?.IsArchived ?? false;
        Text = member is null ? "Добавить члена комиссии" : "Изменить члена комиссии";
        if (member is null) return;
        lastNameTextBox.Text = member.LastName;
        firstNameTextBox.Text = member.FirstName;
        middleNameTextBox.Text = member.MiddleName ?? string.Empty;
    }

    private void SaveButton_Click(object? sender, EventArgs e) => SaveMember();

    private void SaveMember()
    {
        if (memberService is null) return;
        if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
        {
            ShowValidationMessage("Введите фамилию.", lastNameTextBox);
            return;
        }
        if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
        {
            ShowValidationMessage("Введите имя.", firstNameTextBox);
            return;
        }

        CommissionMember member = new()
        {
            Id = memberId,
            LastName = lastNameTextBox.Text,
            FirstName = firstNameTextBox.Text,
            MiddleName = middleNameTextBox.Text,
            IsArchived = isArchived
        };
        try
        {
            if (memberId == 0) memberService.Create(member);
            else memberService.Update(member);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) when (exception is CommissionMemberServiceException or ArgumentException)
        {
            MessageBox.Show(this, exception.Message, "Члены комиссии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ShowValidationMessage(string message, TextBox textBox)
    {
        MessageBox.Show(this, message, "Члены комиссии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        textBox.Focus();
    }

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "null-icon.ico");
        if (File.Exists(iconPath)) Icon = new Icon(iconPath);
    }
}
