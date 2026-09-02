using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;
using EmployeeAttestation.Forms.Dialogs;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class CommissionsControl : UserControl
{
    private readonly CommissionService? commissionService;
    private readonly CommissionMemberService? memberService;
    private readonly CommissionCompositionService? compositionService;

    public CommissionsControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(commissionsGrid);
        AppControlStyles.ApplyGrid(membersGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(archiveButton);
        AppControlStyles.ApplySecondaryButton(manageMembersButton);
        AppControlStyles.ApplyPrimaryButton(editMembersButton);
    }

    public CommissionsControl(DatabaseManager databaseManager) : this()
    {
        ArgumentNullException.ThrowIfNull(databaseManager);
        commissionService = new CommissionService(databaseManager);
        memberService = new CommissionMemberService(databaseManager);
        compositionService = new CommissionCompositionService(databaseManager);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshCommissions();
    }

    private void SearchTextBox_TextChanged(object? sender, EventArgs e) => RefreshCommissions();
    private void StatusFilterComboBox_SelectedIndexChanged(object? sender, EventArgs e) => RefreshCommissions();
    private void AddButton_Click(object? sender, EventArgs e) => AddCommission();
    private void EditButton_Click(object? sender, EventArgs e) => EditSelectedCommission();
    private void ArchiveButton_Click(object? sender, EventArgs e) => ArchiveOrRestoreSelectedCommission();
    private void ManageMembersButton_Click(object? sender, EventArgs e) => OpenMembersDirectory();
    private void EditMembersButton_Click(object? sender, EventArgs e) => EditComposition();
    private void CommissionsGrid_SelectionChanged(object? sender, EventArgs e)
    {
        UpdateArchiveButtonText();
        RefreshComposition();
    }
    private void CommissionsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) EditSelectedCommission();
    }

    private void RefreshCommissions()
    {
        if (commissionService is null || IsDisposed || Disposing) return;
        try
        {
            bool? archiveFilter = GetArchiveFilter();
            List<Commission> commissions = string.IsNullOrWhiteSpace(searchTextBox.Text)
                ? commissionService.GetAll(archiveFilter)
                : commissionService.Search(searchTextBox.Text, archiveFilter);
            commissionsGrid.Rows.Clear();
            foreach (Commission commission in commissions)
            {
                int rowIndex = commissionsGrid.Rows.Add(
                    commission.Name,
                    commission.Description ?? string.Empty,
                    commission.IsArchived ? "Архивирована" : "Активна");
                commissionsGrid.Rows[rowIndex].Tag = commission;
            }
            commissionsGrid.ClearSelection();
            membersGrid.Rows.Clear();
            UpdateArchiveButtonText();
        }
        catch (CommissionServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void RefreshComposition()
    {
        membersGrid.Rows.Clear();
        if (compositionService is null || GetSelectedCommission() is not Commission commission) return;
        try
        {
            foreach (CommissionComposition item in compositionService.GetComposition(commission.Id))
            {
                int rowIndex = membersGrid.Rows.Add(
                    item.CommissionMemberName,
                    CommissionRoleHelper.GetDisplayName(item.Role),
                    item.SortOrder);
                membersGrid.Rows[rowIndex].Tag = item;
            }
            membersGrid.ClearSelection();
        }
        catch (CommissionCompositionServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void AddCommission()
    {
        if (commissionService is null) return;
        using CommissionEditForm form = new(commissionService);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshCommissions();
    }

    private void EditSelectedCommission()
    {
        if (commissionService is null) return;
        Commission? commission = GetSelectedCommission();
        if (commission is null)
        {
            ShowSelectionRequired();
            return;
        }
        using CommissionEditForm form = new(commissionService, commission);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshCommissions();
    }

    private void ArchiveOrRestoreSelectedCommission()
    {
        if (commissionService is null) return;
        Commission? commission = GetSelectedCommission();
        if (commission is null)
        {
            ShowSelectionRequired();
            return;
        }
        if (!commission.IsArchived
            && MessageBox.Show(this, "Архивировать выбранную комиссию?", "Комиссии",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }
        try
        {
            if (commission.IsArchived) commissionService.Restore(commission.Id);
            else commissionService.Archive(commission.Id);
            RefreshCommissions();
        }
        catch (CommissionServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void OpenMembersDirectory()
    {
        if (memberService is null) return;
        using CommissionMembersForm form = new(memberService);
        form.ShowDialog(this);
        RefreshComposition();
    }

    private void EditComposition()
    {
        if (compositionService is null || memberService is null) return;
        Commission? commission = GetSelectedCommission();
        if (commission is null)
        {
            ShowSelectionRequired();
            return;
        }
        using CommissionCompositionForm form = new(compositionService, memberService, commission);
        form.ShowDialog(this);
        RefreshComposition();
    }

    private bool? GetArchiveFilter() => statusFilterComboBox.SelectedIndex switch { 0 => null, 2 => true, _ => false };

    private Commission? GetSelectedCommission() => commissionsGrid.SelectedRows.Count == 1
        ? commissionsGrid.SelectedRows[0].Tag as Commission
        : null;

    private void UpdateArchiveButtonText() => archiveButton.Text = GetSelectedCommission()?.IsArchived == true
        ? "Восстановить"
        : "Архивировать";

    private void ShowSelectionRequired() => MessageBox.Show(
        this, "Выберите комиссию.", "Комиссии", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void ShowServiceError(string message) => MessageBox.Show(
        this, message, "Комиссии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
