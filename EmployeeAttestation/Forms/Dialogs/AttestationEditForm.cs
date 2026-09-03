using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class AttestationEditForm : Form
{
    private readonly AttestationService? attestationService;
    private readonly EmployeeService? employeeService;
    private readonly CommissionService? commissionService;
    private readonly EvaluationCriterionService? criterionService;
    private readonly Attestation? attestation;
    private readonly bool readOnlyMode;
    private bool loadingValues;
    private HashSet<int> selectedCriterionIds = [];
    private IReadOnlyCollection<AttestationCriterion> existingCriteria = [];

    public AttestationEditForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(scheduleButton);
        AppControlStyles.ApplySecondaryButton(saveDraftButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        AppControlStyles.ApplySecondaryButton(selectCriteriaButton);
        LoadWindowIcon();
    }

    public AttestationEditForm(
        AttestationService attestationService,
        EmployeeService employeeService,
        CommissionService commissionService,
        EvaluationCriterionService criterionService,
        Attestation? attestation = null,
        bool readOnly = false)
        : this()
    {
        this.attestationService = attestationService ?? throw new ArgumentNullException(nameof(attestationService));
        this.employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        this.commissionService = commissionService ?? throw new ArgumentNullException(nameof(commissionService));
        this.criterionService = criterionService ?? throw new ArgumentNullException(nameof(criterionService));
        this.attestation = attestation;
        readOnlyMode = readOnly || (attestation is not null && !AttestationStatusHelper.CanEdit(attestation.Status));
        ConfigureMode();
        LoadValues();
    }

    private void EmployeeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!loadingValues && employeeComboBox.SelectedItem is EmployeeListItem employee)
        {
            evaluateManagerialCheckBox.Checked = employee.IsManager;
            if (attestation is null) LoadDefaultCriteria();
        }
    }

    private void SaveDraftButton_Click(object? sender, EventArgs e) => SaveAttestation(schedule: false);

    private void ScheduleButton_Click(object? sender, EventArgs e) => SaveAttestation(schedule: true);

    private void ConfigureMode()
    {
        string status = attestation?.Status ?? AttestationStatusHelper.Draft;
        Text = attestation is null
            ? "Создать аттестацию"
            : readOnlyMode
                ? "Просмотр аттестации"
                : "Изменить аттестацию";
        statusValueLabel.Text = AttestationStatusHelper.GetDisplayName(status);

        if (readOnlyMode)
        {
            employeeComboBox.Enabled = false;
            commissionComboBox.Enabled = false;
            attestationDatePicker.Enabled = false;
            evaluateManagerialCheckBox.Enabled = false;
            selectCriteriaButton.Visible = false;
            saveDraftButton.Visible = false;
            scheduleButton.Visible = false;
            cancelButton.Text = "Закрыть";
            AcceptButton = cancelButton;
            return;
        }

        if (attestation?.Status == AttestationStatusHelper.Scheduled)
        {
            saveDraftButton.Text = "Сохранить";
            scheduleButton.Visible = false;
            AcceptButton = saveDraftButton;
        }
    }

    private void LoadValues()
    {
        if (employeeService is null || commissionService is null) return;

        loadingValues = true;
        try
        {
            List<EmployeeListItem> employees = employeeService.GetAll(false);
            List<Commission> commissions = commissionService.GetAll(false);

            if (attestation is not null)
            {
                AddCurrentEmployeeIfMissing(employees, attestation.EmployeeId);
                AddCurrentCommissionIfMissing(commissions, attestation.CommissionId);
            }

            employeeComboBox.DisplayMember = nameof(EmployeeListItem.FullName);
            employeeComboBox.ValueMember = nameof(EmployeeListItem.Id);
            employeeComboBox.DataSource = employees;
            commissionComboBox.DisplayMember = nameof(Commission.Name);
            commissionComboBox.ValueMember = nameof(Commission.Id);
            commissionComboBox.DataSource = commissions;

            if (attestation is null)
            {
                attestationDatePicker.Value = DateTime.Today;
                attestationDatePicker.Checked = true;
                if (employeeComboBox.SelectedItem is EmployeeListItem selectedEmployee)
                {
                    evaluateManagerialCheckBox.Checked = selectedEmployee.IsManager;
                }
                LoadDefaultCriteria();
                return;
            }

            employeeComboBox.SelectedValue = attestation.EmployeeId;
            commissionComboBox.SelectedValue = attestation.CommissionId;
            attestationDatePicker.Checked = attestation.AttestationDate.HasValue;
            if (attestation.AttestationDate.HasValue)
            {
                attestationDatePicker.Value = attestation.AttestationDate.Value;
            }
            evaluateManagerialCheckBox.Checked = attestation.EvaluateManagerial;
            existingCriteria = attestationService?.GetCriteria(attestation.Id) ?? [];
            selectedCriterionIds = existingCriteria
                .Where(item => item.CriterionId.HasValue)
                .Select(item => item.CriterionId!.Value)
                .ToHashSet();
            UpdateCriteriaCount();
        }
        catch (Exception exception) when (exception is EmployeeServiceException
            or CommissionServiceException
            or EvaluationCriterionServiceException
            or AttestationServiceException)
        {
            saveDraftButton.Enabled = false;
            scheduleButton.Enabled = false;
            MessageBox.Show(
                this,
                "Не удалось загрузить список сотрудников или комиссий.",
                "Аттестации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        finally
        {
            loadingValues = false;
        }
    }

    private void AddCurrentEmployeeIfMissing(List<EmployeeListItem> employees, int employeeId)
    {
        if (employeeService is null || employees.Any(item => item.Id == employeeId)) return;
        Employee? employee = employeeService.GetById(employeeId);
        if (employee is null) return;
        employees.Add(new EmployeeListItem
        {
            Id = employee.Id,
            FullName = employee.FullName,
            IsManager = employee.IsManager,
            IsArchived = employee.IsArchived
        });
    }

    private void AddCurrentCommissionIfMissing(List<Commission> commissions, int commissionId)
    {
        if (commissionService is null || commissions.Any(item => item.Id == commissionId)) return;
        Commission? commission = commissionService.GetById(commissionId);
        if (commission is not null) commissions.Add(commission);
    }

    private void SaveAttestation(bool schedule)
    {
        if (attestationService is null || readOnlyMode) return;
        if (employeeComboBox.SelectedItem is not EmployeeListItem employee)
        {
            ShowValidationMessage("Выберите сотрудника.", employeeComboBox);
            return;
        }
        if (commissionComboBox.SelectedItem is not Commission commission)
        {
            ShowValidationMessage("Выберите комиссию.", commissionComboBox);
            return;
        }
        if (schedule && !attestationDatePicker.Checked)
        {
            ShowValidationMessage("Выберите дату аттестации.", attestationDatePicker);
            return;
        }

        Attestation value = new()
        {
            Id = attestation?.Id ?? 0,
            EmployeeId = employee.Id,
            CommissionId = commission.Id,
            AttestationDate = attestationDatePicker.Checked ? attestationDatePicker.Value.Date : null,
            Status = attestation?.Status ?? AttestationStatusHelper.Draft,
            EvaluateManagerial = evaluateManagerialCheckBox.Checked,
            CreatedAt = attestation?.CreatedAt ?? DateTime.Now
        };

        try
        {
            if (attestation?.Status == AttestationStatusHelper.Scheduled)
            {
                attestationService.UpdateScheduled(value, selectedCriterionIds);
            }
            else if (schedule)
            {
                attestationService.SaveScheduled(value, selectedCriterionIds);
            }
            else
            {
                attestationService.SaveDraft(value, selectedCriterionIds);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) when (exception is AttestationServiceException or ArgumentException)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Аттестации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void EvaluateManagerialCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        if (loadingValues || criterionService is null) return;
        try
        {
            if (!evaluateManagerialCheckBox.Checked)
            {
                HashSet<int> managerialIds = criterionService.GetActive(true)
                    .Where(item => item.ManagersOnly)
                    .Select(item => item.Id)
                    .ToHashSet();
                selectedCriterionIds.RemoveWhere(managerialIds.Contains);
                UpdateCriteriaCount();
            }
        }
        catch (EvaluationCriterionServiceException exception)
        {
            MessageBox.Show(this, exception.Message, "Аттестации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void SelectCriteriaButton_Click(object? sender, EventArgs e)
    {
        if (criterionService is null || readOnlyMode) return;
        using AttestationCriteriaSelectForm form = new(
            criterionService,
            selectedCriterionIds,
            evaluateManagerialCheckBox.Checked,
            existingCriteria);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        selectedCriterionIds = form.SelectedCriterionIds.ToHashSet();
        UpdateCriteriaCount();
    }

    private void LoadDefaultCriteria()
    {
        if (criterionService is null) return;
        selectedCriterionIds = criterionService.GetActive(evaluateManagerialCheckBox.Checked)
            .Select(item => item.Id)
            .ToHashSet();
        UpdateCriteriaCount();
    }

    private void UpdateCriteriaCount() => criteriaCountLabel.Text = $"Выбрано: {selectedCriterionIds.Count}";

    private void ShowValidationMessage(string message, Control control)
    {
        MessageBox.Show(this, message, "Аттестации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
    }

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico");
        if (File.Exists(iconPath)) Icon = new Icon(iconPath);
    }
}
