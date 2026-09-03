using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class CommissionMembersForm : Form
{
    private readonly CommissionMemberService? memberService;

    public CommissionMembersForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(membersGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(archiveButton);
        AppControlStyles.ApplySecondaryButton(closeButton);
        LoadWindowIcon();
    }

    public CommissionMembersForm(CommissionMemberService memberService)
        : this()
    {
        this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshMembers();
    }

    private void SearchTextBox_TextChanged(object? sender, EventArgs e) => RefreshMembers();
    private void StatusFilterComboBox_SelectedIndexChanged(object? sender, EventArgs e) => RefreshMembers();
    private void AddButton_Click(object? sender, EventArgs e) => AddMember();
    private void EditButton_Click(object? sender, EventArgs e) => EditSelectedMember();
    private void ArchiveButton_Click(object? sender, EventArgs e) => ArchiveOrRestoreSelectedMember();
    private void MembersGrid_SelectionChanged(object? sender, EventArgs e) => UpdateArchiveButtonText();
    private void MembersGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) EditSelectedMember();
    }

    private void RefreshMembers()
    {
        if (memberService is null || IsDisposed || Disposing) return;
        try
        {
            bool? archiveFilter = GetArchiveFilter();
            List<CommissionMember> members = string.IsNullOrWhiteSpace(searchTextBox.Text)
                ? memberService.GetAll(archiveFilter)
                : memberService.Search(searchTextBox.Text, archiveFilter);
            membersGrid.Rows.Clear();
            foreach (CommissionMember member in members)
            {
                int rowIndex = membersGrid.Rows.Add(member.FullName, member.IsArchived ? "Архивирован" : "Активен");
                membersGrid.Rows[rowIndex].Tag = member;
            }
            membersGrid.ClearSelection();
            UpdateArchiveButtonText();
        }
        catch (CommissionMemberServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void AddMember()
    {
        if (memberService is null) return;
        using CommissionMemberEditForm form = new(memberService);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshMembers();
    }

    private void EditSelectedMember()
    {
        if (memberService is null) return;
        CommissionMember? member = GetSelectedMember();
        if (member is null)
        {
            ShowSelectionRequired();
            return;
        }
        using CommissionMemberEditForm form = new(memberService, member);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshMembers();
    }

    private void ArchiveOrRestoreSelectedMember()
    {
        if (memberService is null) return;
        CommissionMember? member = GetSelectedMember();
        if (member is null)
        {
            ShowSelectionRequired();
            return;
        }
        if (!member.IsArchived
            && MessageBox.Show(this, "Архивировать выбранного члена комиссии?", "Члены комиссии",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }
        try
        {
            if (member.IsArchived) memberService.Restore(member.Id);
            else memberService.Archive(member.Id);
            RefreshMembers();
        }
        catch (CommissionMemberServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private bool? GetArchiveFilter() => statusFilterComboBox.SelectedIndex switch { 0 => null, 2 => true, _ => false };

    private CommissionMember? GetSelectedMember() => membersGrid.SelectedRows.Count == 1
        ? membersGrid.SelectedRows[0].Tag as CommissionMember
        : null;

    private void UpdateArchiveButtonText() => archiveButton.Text = GetSelectedMember()?.IsArchived == true
        ? "Восстановить"
        : "Архивировать";

    private void ShowSelectionRequired() => MessageBox.Show(
        this, "Выберите члена комиссии.", "Члены комиссии", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void ShowServiceError(string message) => MessageBox.Show(
        this, message, "Члены комиссии", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico");
        if (File.Exists(iconPath)) Icon = new Icon(iconPath);
    }
}
