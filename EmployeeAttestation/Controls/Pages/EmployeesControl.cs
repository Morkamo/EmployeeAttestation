using EmployeeAttestation.Data;
using EmployeeAttestation.Forms.Dialogs;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class EmployeesControl : UserControl
{
    private readonly EmployeeService? employeeService;
    private readonly DepartmentService? departmentService;
    private readonly PositionService? positionService;
    private readonly AttestationService? attestationService;
    private readonly AttestationProcessService? processService;
    private readonly AttestationDocumentService? documentService;

    public EmployeesControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(employeesGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(archiveButton);
        AppControlStyles.ApplySecondaryButton(historyButton);
    }

    public EmployeesControl(DatabaseManager databaseManager)
        : this()
    {
        ArgumentNullException.ThrowIfNull(databaseManager);
        employeeService = new EmployeeService(databaseManager);
        departmentService = new DepartmentService(databaseManager);
        positionService = new PositionService(databaseManager);
        attestationService = new AttestationService(databaseManager);
        processService = new AttestationProcessService(databaseManager);
        documentService = new AttestationDocumentService(databaseManager);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshEmployees();
    }

    private void SearchTextBox_TextChanged(object? sender, EventArgs e) => RefreshEmployees();

    private void StatusFilterComboBox_SelectedIndexChanged(object? sender, EventArgs e) => RefreshEmployees();

    private void AddButton_Click(object? sender, EventArgs e) => AddEmployee();

    private void EditButton_Click(object? sender, EventArgs e) => EditSelectedEmployee();

    private void ArchiveButton_Click(object? sender, EventArgs e) => ArchiveOrRestoreSelectedEmployee();
    private void HistoryButton_Click(object? sender, EventArgs e) => OpenHistory();

    private void EmployeesGrid_SelectionChanged(object? sender, EventArgs e) => UpdateArchiveButtonText();

    private void EmployeesGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) EditSelectedEmployee();
    }

    private void RefreshEmployees()
    {
        if (employeeService is null || IsDisposed || Disposing) return;
        try
        {
            bool? archiveFilter = GetArchiveFilter();
            List<EmployeeListItem> employees = string.IsNullOrWhiteSpace(searchTextBox.Text)
                ? employeeService.GetAll(archiveFilter)
                : employeeService.Search(searchTextBox.Text, archiveFilter);
            employeesGrid.Rows.Clear();
            foreach (EmployeeListItem employee in employees)
            {
                int rowIndex = employeesGrid.Rows.Add(
                    employee.FullName,
                    employee.DepartmentName,
                    employee.PositionName,
                    employee.IsManager ? "Да" : "Нет",
                    employee.IsArchived ? "Архивирован" : "Активен");
                employeesGrid.Rows[rowIndex].Tag = employee;
            }
            employeesGrid.ClearSelection();
            UpdateArchiveButtonText();
        }
        catch (EmployeeServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void AddEmployee()
    {
        if (employeeService is null || departmentService is null || positionService is null) return;
        using EmployeeEditForm form = new(employeeService, departmentService, positionService);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshEmployees();
    }

    private void EditSelectedEmployee()
    {
        if (employeeService is null || departmentService is null || positionService is null) return;
        EmployeeListItem? selectedItem = GetSelectedEmployee();
        if (selectedItem is null)
        {
            ShowSelectionRequired();
            return;
        }
        try
        {
            Employee? employee = employeeService.GetById(selectedItem.Id);
            if (employee is null)
            {
                ShowServiceError("Сотрудник не найден.");
                RefreshEmployees();
                return;
            }
            using EmployeeEditForm form = new(employeeService, departmentService, positionService, employee);
            if (form.ShowDialog(this) == DialogResult.OK) RefreshEmployees();
        }
        catch (EmployeeServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void ArchiveOrRestoreSelectedEmployee()
    {
        if (employeeService is null) return;
        EmployeeListItem? employee = GetSelectedEmployee();
        if (employee is null)
        {
            ShowSelectionRequired();
            return;
        }

        if (!employee.IsArchived
            && MessageBox.Show(
                this,
                "Архивировать выбранного сотрудника?",
                "Сотрудники",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            if (employee.IsArchived) employeeService.Restore(employee.Id);
            else employeeService.Archive(employee.Id);
            RefreshEmployees();
        }
        catch (EmployeeServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void OpenHistory()
    {
        if (employeeService is null || attestationService is null || processService is null || documentService is null) return;
        EmployeeListItem? selected = GetSelectedEmployee();
        if (selected is null) { ShowSelectionRequired(); return; }
        try
        {
            Employee? employee = employeeService.GetById(selected.Id);
            if (employee is null) { ShowServiceError("Сотрудник не найден."); RefreshEmployees(); return; }
            using EmployeeAttestationHistoryForm form = new(attestationService, processService, documentService, employee);
            form.ShowDialog(this);
        }
        catch (EmployeeServiceException exception) { ShowServiceError(exception.Message); }
    }

    private bool? GetArchiveFilter() => statusFilterComboBox.SelectedIndex switch
    {
        0 => null,
        2 => true,
        _ => false
    };

    private EmployeeListItem? GetSelectedEmployee() => employeesGrid.SelectedRows.Count == 1
        ? employeesGrid.SelectedRows[0].Tag as EmployeeListItem
        : null;

    private void UpdateArchiveButtonText()
    {
        archiveButton.Text = GetSelectedEmployee()?.IsArchived == true ? "Восстановить" : "Архивировать";
    }

    private void ShowSelectionRequired()
    {
        MessageBox.Show(this, "Выберите сотрудника.", "Сотрудники", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowServiceError(string message)
    {
        MessageBox.Show(this, message, "Сотрудники", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
