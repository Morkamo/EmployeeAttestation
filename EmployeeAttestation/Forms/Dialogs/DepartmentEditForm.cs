using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class DepartmentEditForm : Form
{
    private readonly DepartmentService? departmentService;
    private readonly int departmentId;

    public DepartmentEditForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(saveButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        LoadWindowIcon();
    }

    public DepartmentEditForm(DepartmentService departmentService, Department? department = null)
        : this()
    {
        this.departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        departmentId = department?.Id ?? 0;

        if (department is null)
        {
            Text = "Добавить подразделение";
            return;
        }

        Text = "Изменить подразделение";
        codeTextBox.Text = department.Code;
        nameTextBox.Text = department.Name;
        documentNameTextBox.Text = department.DocumentName ?? string.Empty;
    }

    private void SaveButton_Click(object? sender, EventArgs e) => SaveDepartment();

    private void SaveDepartment()
    {
        if (departmentService is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(codeTextBox.Text))
        {
            ShowValidationMessage("Введите код подразделения.", codeTextBox);
            return;
        }

        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            ShowValidationMessage("Введите наименование подразделения.", nameTextBox);
            return;
        }

        Department department = new()
        {
            Id = departmentId,
            Code = codeTextBox.Text,
            Name = nameTextBox.Text,
            DocumentName = documentNameTextBox.Text
        };

        try
        {
            if (departmentId == 0)
            {
                departmentService.Create(department);
            }
            else
            {
                departmentService.Update(department);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) when (exception is DepartmentServiceException or ArgumentException)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Подразделения",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ShowValidationMessage(string message, TextBox textBox)
    {
        MessageBox.Show(
            this,
            message,
            "Подразделения",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        textBox.Focus();
    }

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico");
        if (File.Exists(iconPath))
        {
            Icon = new Icon(iconPath);
        }
    }
}
