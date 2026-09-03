using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class CommissionCompositionForm : Form
{
    private readonly CommissionCompositionService? compositionService;
    private readonly CommissionMemberService? memberService;
    private readonly Commission? commission;

    public CommissionCompositionForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(compositionGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(updateButton);
        AppControlStyles.ApplySecondaryButton(removeButton);
        AppControlStyles.ApplySecondaryButton(closeButton);
        LoadWindowIcon();
    }

    public CommissionCompositionForm(
        CommissionCompositionService compositionService,
        CommissionMemberService memberService,
        Commission commission)
        : this()
    {
        this.compositionService = compositionService ?? throw new ArgumentNullException(nameof(compositionService));
        this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        this.commission = commission ?? throw new ArgumentNullException(nameof(commission));
        commissionNameLabel.Text = commission.Name;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadEditorValues();
        RefreshComposition();
    }

    private void AddButton_Click(object? sender, EventArgs e) => AddMember();
    private void UpdateButton_Click(object? sender, EventArgs e) => UpdateSelectedMember();
    private void RemoveButton_Click(object? sender, EventArgs e) => RemoveSelectedMember();
    private void CompositionGrid_SelectionChanged(object? sender, EventArgs e) => LoadSelectedComposition();

    private void LoadEditorValues()
    {
        if (memberService is null) return;
        try
        {
            memberComboBox.DisplayMember = nameof(CommissionMember.FullName);
            memberComboBox.ValueMember = nameof(CommissionMember.Id);
            memberComboBox.DataSource = memberService.GetAll(false);
            roleComboBox.DisplayMember = nameof(CommissionRoleOption.DisplayName);
            roleComboBox.ValueMember = nameof(CommissionRoleOption.Value);
            roleComboBox.DataSource = CommissionRoleHelper.AvailableRoles.ToList();
            roleComboBox.SelectedValue = "Member";
        }
        catch (CommissionMemberServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void RefreshComposition()
    {
        if (compositionService is null || commission is null || IsDisposed || Disposing) return;
        try
        {
            List<CommissionComposition> composition = compositionService.GetComposition(commission.Id);
            compositionGrid.Rows.Clear();
            foreach (CommissionComposition item in composition)
            {
                int rowIndex = compositionGrid.Rows.Add(
                    item.CommissionMemberName,
                    CommissionRoleHelper.GetDisplayName(item.Role),
                    item.SortOrder);
                compositionGrid.Rows[rowIndex].Tag = item;
            }
            compositionGrid.ClearSelection();
        }
        catch (CommissionCompositionServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void AddMember()
    {
        if (compositionService is null || commission is null) return;
        if (memberComboBox.SelectedItem is not CommissionMember member)
        {
            ShowSelectionMessage("Выберите члена комиссии.");
            return;
        }
        if (roleComboBox.SelectedItem is not CommissionRoleOption role)
        {
            ShowSelectionMessage("Выберите роль участника.");
            return;
        }
        try
        {
            compositionService.AddMember(commission.Id, member.Id, role.Value, decimal.ToInt32(sortOrderNumeric.Value));
            RefreshComposition();
        }
        catch (Exception exception) when (exception is CommissionCompositionServiceException or ArgumentException)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void UpdateSelectedMember()
    {
        if (compositionService is null) return;
        CommissionComposition? composition = GetSelectedComposition();
        if (composition is null)
        {
            ShowSelectionMessage("Выберите участника состава.");
            return;
        }
        if (roleComboBox.SelectedItem is not CommissionRoleOption role)
        {
            ShowSelectionMessage("Выберите роль участника.");
            return;
        }
        try
        {
            compositionService.UpdateMember(composition.Id, role.Value, decimal.ToInt32(sortOrderNumeric.Value));
            RefreshComposition();
        }
        catch (Exception exception) when (exception is CommissionCompositionServiceException or ArgumentException)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void RemoveSelectedMember()
    {
        if (compositionService is null) return;
        CommissionComposition? composition = GetSelectedComposition();
        if (composition is null)
        {
            ShowSelectionMessage("Выберите участника состава.");
            return;
        }
        if (MessageBox.Show(this, "Удалить выбранного человека из состава комиссии?", "Состав комиссии",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }
        try
        {
            compositionService.RemoveMember(composition.Id);
            RefreshComposition();
        }
        catch (CommissionCompositionServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void LoadSelectedComposition()
    {
        CommissionComposition? composition = GetSelectedComposition();
        if (composition is null) return;
        memberComboBox.SelectedValue = composition.CommissionMemberId;
        roleComboBox.SelectedValue = composition.Role;
        sortOrderNumeric.Value = Math.Clamp(composition.SortOrder, 0, decimal.ToInt32(sortOrderNumeric.Maximum));
    }

    private CommissionComposition? GetSelectedComposition() => compositionGrid.SelectedRows.Count == 1
        ? compositionGrid.SelectedRows[0].Tag as CommissionComposition
        : null;

    private void ShowSelectionMessage(string message) => MessageBox.Show(
        this, message, "Состав комиссии", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void ShowServiceError(string message) => MessageBox.Show(
        this, message, "Состав комиссии", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico");
        if (File.Exists(iconPath)) Icon = new Icon(iconPath);
    }
}
