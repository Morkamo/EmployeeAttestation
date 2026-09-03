using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class EmployeeEditForm : Form
{
    private readonly EmployeeService? employeeService;
    private readonly DepartmentService? departmentService;
    private readonly PositionService? positionService;
    private readonly int employeeId;
    private readonly bool isArchived;
    private bool loadingValues;

    public EmployeeEditForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(saveButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        LoadWindowIcon();
    }

    public EmployeeEditForm(
        EmployeeService employeeService,
        DepartmentService departmentService,
        PositionService positionService,
        Employee? employee = null)
        : this()
    {
        this.employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        this.departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        this.positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
        employeeId = employee?.Id ?? 0;
        isArchived = employee?.IsArchived ?? false;
        Text = employee is null ? "Добавить сотрудника" : "Изменить сотрудника";
        LoadValues(employee);
    }

    private void PositionComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!loadingValues && positionComboBox.SelectedItem is Position position)
        {
            managerCheckBox.Checked = position.IsManagerial;
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e) => SaveEmployee();

    private void LoadValues(Employee? employee)
    {
        if (departmentService is null || positionService is null)
        {
            return;
        }

        loadingValues = true;
        try
        {
            departmentComboBox.DisplayMember = nameof(Department.Name);
            departmentComboBox.ValueMember = nameof(Department.Id);
            departmentComboBox.DataSource = departmentService.GetAll();
            positionComboBox.DisplayMember = nameof(Position.Name);
            positionComboBox.ValueMember = nameof(Position.Id);
            positionComboBox.DataSource = positionService.GetAll();

            if (employee is null)
            {
                loadingValues = false;
                if (positionComboBox.SelectedItem is Position selectedPosition)
                {
                    managerCheckBox.Checked = selectedPosition.IsManagerial;
                }
                return;
            }

            lastNameTextBox.Text = employee.LastName;
            firstNameTextBox.Text = employee.FirstName;
            middleNameTextBox.Text = employee.MiddleName ?? string.Empty;
            departmentComboBox.SelectedValue = employee.DepartmentId;
            positionComboBox.SelectedValue = employee.PositionId;
            managerCheckBox.Checked = employee.IsManager;
        }
        catch (Exception exception) when (exception is DepartmentServiceException or PositionServiceException)
        {
            saveButton.Enabled = false;
            MessageBox.Show(
                this,
                "Не удалось загрузить список подразделений или должностей.",
                "Сотрудники",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        finally
        {
            loadingValues = false;
        }
    }

    private void SaveEmployee()
    {
        if (employeeService is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
        {
            ShowValidationMessage("Введите фамилию сотрудника.", lastNameTextBox);
            return;
        }
        if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
        {
            ShowValidationMessage("Введите имя сотрудника.", firstNameTextBox);
            return;
        }
        if (departmentComboBox.SelectedItem is not Department department)
        {
            ShowValidationMessage("Выберите подразделение.", departmentComboBox);
            return;
        }
        if (positionComboBox.SelectedItem is not Position position)
        {
            ShowValidationMessage("Выберите должность.", positionComboBox);
            return;
        }

        Employee employee = new()
        {
            Id = employeeId,
            LastName = lastNameTextBox.Text,
            FirstName = firstNameTextBox.Text,
            MiddleName = middleNameTextBox.Text,
            DepartmentId = department.Id,
            PositionId = position.Id,
            IsManager = managerCheckBox.Checked,
            IsArchived = isArchived
        };

        try
        {
            if (employeeId == 0)
            {
                employeeService.Create(employee);
            }
            else
            {
                employeeService.Update(employee);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) when (exception is EmployeeServiceException or ArgumentException)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Сотрудники",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ShowValidationMessage(string message, Control control)
    {
        MessageBox.Show(
            this,
            message,
            "Сотрудники",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        control.Focus();
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
