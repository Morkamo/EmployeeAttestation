using EmployeeAttestation.Data;
using EmployeeAttestation.Forms.Dialogs;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class DepartmentsControl : UserControl
{
    private readonly DepartmentService? departmentService;

    public DepartmentsControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(departmentsGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(deleteButton);
    }

    public DepartmentsControl(DatabaseManager databaseManager)
        : this()
    {
        departmentService = new DepartmentService(
            databaseManager ?? throw new ArgumentNullException(nameof(databaseManager)));
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshDepartments();
    }

    private void SearchTextBox_TextChanged(object? sender, EventArgs e) => RefreshDepartments();

    private void AddButton_Click(object? sender, EventArgs e) => AddDepartment();

    private void EditButton_Click(object? sender, EventArgs e) => EditSelectedDepartment();

    private void DeleteButton_Click(object? sender, EventArgs e) => DeleteSelectedDepartment();

    private void DepartmentsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            EditSelectedDepartment();
        }
    }

    private void RefreshDepartments()
    {
        if (departmentService is null || IsDisposed || Disposing)
        {
            return;
        }

        try
        {
            List<Department> departments = string.IsNullOrWhiteSpace(searchTextBox.Text)
                ? departmentService.GetAll()
                : departmentService.Search(searchTextBox.Text);

            departmentsGrid.Rows.Clear();
            foreach (Department department in departments)
            {
                int rowIndex = departmentsGrid.Rows.Add(
                    department.Code,
                    department.Name,
                    department.DocumentName ?? string.Empty);
                departmentsGrid.Rows[rowIndex].Tag = department;
            }

            departmentsGrid.ClearSelection();
        }
        catch (DepartmentServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void AddDepartment()
    {
        if (departmentService is null)
        {
            return;
        }

        using DepartmentEditForm form = new(departmentService);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            RefreshDepartments();
        }
    }

    private void EditSelectedDepartment()
    {
        if (departmentService is null)
        {
            return;
        }

        Department? department = GetSelectedDepartment();
        if (department is null)
        {
            ShowSelectionRequired();
            return;
        }

        using DepartmentEditForm form = new(departmentService, department);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            RefreshDepartments();
        }
    }

    private void DeleteSelectedDepartment()
    {
        if (departmentService is null)
        {
            return;
        }

        Department? department = GetSelectedDepartment();
        if (department is null)
        {
            ShowSelectionRequired();
            return;
        }

        DialogResult confirmation = MessageBox.Show(
            this,
            "Удалить выбранное подразделение?",
            "Подразделения",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);
        if (confirmation != DialogResult.Yes)
        {
            return;
        }

        try
        {
            departmentService.Delete(department.Id);
            RefreshDepartments();
        }
        catch (DepartmentServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private Department? GetSelectedDepartment() => departmentsGrid.CurrentRow?.Tag as Department;

    private void ShowSelectionRequired()
    {
        MessageBox.Show(
            this,
            "Выберите подразделение.",
            "Подразделения",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ShowServiceError(string message)
    {
        MessageBox.Show(
            this,
            message,
            "Подразделения",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
