using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class EmployeeAttestationHistoryForm : Form
{
    private readonly AttestationService? attestationService;
    private readonly AttestationProcessService? processService;
    private readonly Employee? employee;

    public EmployeeAttestationHistoryForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(historyGrid);
        AppControlStyles.ApplyPrimaryButton(openButton);
        AppControlStyles.ApplySecondaryButton(closeButton);
        LoadWindowIcon();
    }

    public EmployeeAttestationHistoryForm(
        AttestationService attestationService,
        AttestationProcessService processService,
        Employee employee)
        : this()
    {
        this.attestationService = attestationService ?? throw new ArgumentNullException(nameof(attestationService));
        this.processService = processService ?? throw new ArgumentNullException(nameof(processService));
        this.employee = employee ?? throw new ArgumentNullException(nameof(employee));
        employeeLabel.Text = employee.FullName;
    }

    protected override void OnLoad(EventArgs e) { base.OnLoad(e); RefreshHistory(); }
    private void OpenButton_Click(object? sender, EventArgs e) => OpenSelected();
    private void HistoryGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) OpenSelected(); }
    private void HistoryGrid_SelectionChanged(object? sender, EventArgs e) => openButton.Enabled = GetSelected() is not null;

    private void RefreshHistory()
    {
        if (attestationService is null || employee is null) return;
        try
        {
            historyGrid.Rows.Clear();
            foreach (AttestationListItem item in attestationService.GetEmployeeHistory(employee.Id))
            {
                int row = historyGrid.Rows.Add(item.AttestationDate?.ToString("dd.MM.yyyy") ?? "—", item.CommissionName,
                    item.OverallAverage?.ToString("0.00") ?? "—", item.Decision ?? string.Empty,
                    AttestationStatusHelper.GetDisplayName(item.Status));
                historyGrid.Rows[row].Tag = item;
            }
            historyGrid.ClearSelection();
            openButton.Enabled = false;
        }
        catch (AttestationServiceException exception) { ShowError(exception.Message); }
    }

    private void OpenSelected()
    {
        if (attestationService is null || processService is null) return;
        AttestationListItem? item = GetSelected();
        if (item is null) return;
        if (item.Status is not (AttestationStatusHelper.InProgress or AttestationStatusHelper.Decision or AttestationStatusHelper.Completed))
        {
            MessageBox.Show(this, "Черновики и запланированные аттестации открываются из раздела «Аттестации».",
                "История аттестаций", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using AttestationProcessForm form = new(attestationService, processService, item.Id);
        form.ShowDialog(this);
        RefreshHistory();
    }

    private AttestationListItem? GetSelected() => historyGrid.SelectedRows.Count == 1
        ? historyGrid.SelectedRows[0].Tag as AttestationListItem : null;
    private void ShowError(string message) => MessageBox.Show(this, message, "История аттестаций", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    private void LoadWindowIcon() { string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "null-icon.ico"); if (File.Exists(path)) Icon = new Icon(path); }
}
