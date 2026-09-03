using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;
using EmployeeAttestation.Forms.Dialogs;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class AttestationsControl : UserControl
{
    private readonly AttestationService? attestationService;
    private readonly EmployeeService? employeeService;
    private readonly CommissionService? commissionService;
    private readonly EvaluationCriterionService? criterionService;
    private readonly AttestationProcessService? processService;

    public AttestationsControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(attestationsGrid);
        AppControlStyles.ApplyPrimaryButton(createButton);
        AppControlStyles.ApplySecondaryButton(openButton);
        AppControlStyles.ApplySecondaryButton(startButton);
        AppControlStyles.ApplySecondaryButton(cancelAttestationButton);
        AppControlStyles.ApplySecondaryButton(criteriaButton);
        InitializeStatusFilter();
        UpdateButtonState();
    }

    public AttestationsControl(DatabaseManager databaseManager)
        : this()
    {
        ArgumentNullException.ThrowIfNull(databaseManager);
        attestationService = new AttestationService(databaseManager);
        employeeService = new EmployeeService(databaseManager);
        commissionService = new CommissionService(databaseManager);
        criterionService = new EvaluationCriterionService(databaseManager);
        processService = new AttestationProcessService(databaseManager);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshAttestations();
    }

    private void SearchTextBox_TextChanged(object? sender, EventArgs e) => RefreshAttestations();
    private void StatusFilterComboBox_SelectedIndexChanged(object? sender, EventArgs e) => RefreshAttestations();
    private void CreateButton_Click(object? sender, EventArgs e) => CreateAttestation();
    private void OpenButton_Click(object? sender, EventArgs e) => OpenSelectedAttestation();
    private void StartButton_Click(object? sender, EventArgs e) => StartSelectedAttestation();
    private void CancelAttestationButton_Click(object? sender, EventArgs e) => CancelSelectedAttestation();
    private void CriteriaButton_Click(object? sender, EventArgs e) => OpenCriteriaDirectory();
    private void AttestationsGrid_SelectionChanged(object? sender, EventArgs e) => UpdateButtonState();

    private void AttestationsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) OpenSelectedAttestation();
    }

    private void InitializeStatusFilter()
    {
        statusFilterComboBox.DisplayMember = nameof(StatusFilterItem.DisplayName);
        statusFilterComboBox.ValueMember = nameof(StatusFilterItem.Status);
        statusFilterComboBox.DataSource = new List<StatusFilterItem>
        {
            new("Все", null),
            new("Черновики", AttestationStatusHelper.Draft),
            new("Запланированные", AttestationStatusHelper.Scheduled),
            new("Проводятся", AttestationStatusHelper.InProgress),
            new("Решение комиссии", AttestationStatusHelper.Decision),
            new("Завершенные", AttestationStatusHelper.Completed),
            new("Отмененные", AttestationStatusHelper.Cancelled)
        };
    }

    private void RefreshAttestations()
    {
        if (attestationService is null || IsDisposed || Disposing) return;
        try
        {
            string? status = (statusFilterComboBox.SelectedItem as StatusFilterItem)?.Status;
            List<AttestationListItem> attestations = string.IsNullOrWhiteSpace(searchTextBox.Text)
                ? attestationService.GetAll(status)
                : attestationService.Search(searchTextBox.Text, status);
            attestationsGrid.Rows.Clear();
            foreach (AttestationListItem attestation in attestations)
            {
                int rowIndex = attestationsGrid.Rows.Add(
                    attestation.EmployeeFullName,
                    attestation.DepartmentName,
                    attestation.PositionName,
                    attestation.AttestationDate?.ToString("dd.MM.yyyy") ?? "—",
                    attestation.CommissionName,
                    AttestationStatusHelper.GetDisplayName(attestation.Status),
                    attestation.EvaluateManagerial ? "Да" : "Нет");
                attestationsGrid.Rows[rowIndex].Tag = attestation;
            }
            attestationsGrid.ClearSelection();
            UpdateButtonState();
        }
        catch (AttestationServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void CreateAttestation()
    {
        if (attestationService is null || employeeService is null || commissionService is null || criterionService is null) return;
        using AttestationEditForm form = new(attestationService, employeeService, commissionService, criterionService);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshAttestations();
    }

    private void OpenSelectedAttestation()
    {
        if (attestationService is null || employeeService is null || commissionService is null
            || criterionService is null || processService is null) return;
        AttestationListItem? selectedItem = GetSelectedAttestation();
        if (selectedItem is null)
        {
            ShowSelectionRequired();
            return;
        }

        try
        {
            Attestation? attestation = attestationService.GetById(selectedItem.Id);
            if (attestation is null)
            {
                ShowServiceError("Аттестация больше не существует.");
                RefreshAttestations();
                return;
            }

            if (attestation.Status is AttestationStatusHelper.InProgress
                or AttestationStatusHelper.Decision
                or AttestationStatusHelper.Completed)
            {
                using AttestationProcessForm processForm = new(attestationService, processService, attestation.Id);
                processForm.ShowDialog(this);
                RefreshAttestations();
                return;
            }

            bool readOnly = !AttestationStatusHelper.CanEdit(attestation.Status);
            using AttestationEditForm form = new(
                attestationService,
                employeeService,
                commissionService,
                criterionService,
                attestation,
                readOnly);
            if (form.ShowDialog(this) == DialogResult.OK) RefreshAttestations();
        }
        catch (AttestationServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void StartSelectedAttestation()
    {
        if (attestationService is null) return;
        AttestationListItem? attestation = GetSelectedAttestation();
        if (attestation is null)
        {
            ShowSelectionRequired();
            return;
        }
        if (attestation.Status != AttestationStatusHelper.Scheduled) return;
        if (MessageBox.Show(
                this,
                "Начать проведение выбранной аттестации?",
                "Аттестации",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            attestationService.Start(attestation.Id);
            RefreshAttestations();
            if (processService is not null)
            {
                using AttestationProcessForm form = new(attestationService, processService, attestation.Id);
                form.ShowDialog(this);
                RefreshAttestations();
            }
        }
        catch (AttestationServiceException exception)
        {
            ShowServiceError(exception.Message);
            RefreshAttestations();
        }
    }

    private void OpenCriteriaDirectory()
    {
        if (criterionService is null) return;
        using EvaluationCriteriaForm form = new(criterionService);
        form.ShowDialog(this);
    }

    private void CancelSelectedAttestation()
    {
        if (attestationService is null) return;
        AttestationListItem? attestation = GetSelectedAttestation();
        if (attestation is null)
        {
            ShowSelectionRequired();
            return;
        }
        if (!AttestationStatusHelper.CanCancel(attestation.Status)) return;
        if (MessageBox.Show(
                this,
                "Отменить выбранную аттестацию? Запись останется в базе данных.",
                "Аттестации",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            attestationService.Cancel(attestation.Id);
            RefreshAttestations();
        }
        catch (AttestationServiceException exception)
        {
            ShowServiceError(exception.Message);
            RefreshAttestations();
        }
    }

    private AttestationListItem? GetSelectedAttestation() => attestationsGrid.SelectedRows.Count == 1
        ? attestationsGrid.SelectedRows[0].Tag as AttestationListItem
        : null;

    private void UpdateButtonState()
    {
        AttestationListItem? attestation = GetSelectedAttestation();
        openButton.Enabled = attestation is not null;
        openButton.Text = AttestationStatusHelper.CanEdit(attestation?.Status) ? "Изменить" : "Открыть";
        startButton.Enabled = attestation?.Status == AttestationStatusHelper.Scheduled;
        cancelAttestationButton.Enabled = AttestationStatusHelper.CanCancel(attestation?.Status);
    }

    private void ShowSelectionRequired() => MessageBox.Show(
        this, "Выберите аттестацию.", "Аттестации", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void ShowServiceError(string message) => MessageBox.Show(
        this, message, "Аттестации", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private sealed record StatusFilterItem(string DisplayName, string? Status);
}
