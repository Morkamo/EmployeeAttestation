using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class EvaluationCriteriaForm : Form
{
    private readonly EvaluationCriterionService? criterionService;

    public EvaluationCriteriaForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(criteriaGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(deleteButton);
        InitializeFilters();
        LoadWindowIcon();
    }

    public EvaluationCriteriaForm(EvaluationCriterionService criterionService) : this()
    {
        this.criterionService = criterionService ?? throw new ArgumentNullException(nameof(criterionService));
    }

    protected override void OnLoad(EventArgs e) { base.OnLoad(e); RefreshCriteria(); }
    private void FilterChanged(object? sender, EventArgs e) => RefreshCriteria();
    private void AddButton_Click(object? sender, EventArgs e) => AddCriterion();
    private void EditButton_Click(object? sender, EventArgs e) => EditCriterion();
    private void DeleteButton_Click(object? sender, EventArgs e) => DeleteOrRestoreCriterion();
    private void CriteriaGrid_SelectionChanged(object? sender, EventArgs e) => UpdateButtons();
    private void CriteriaGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) EditCriterion(); }

    private void InitializeFilters()
    {
        categoryFilterComboBox.Items.Add(new FilterItem("Все категории", null));
        foreach (EvaluationCategoryOption category in EvaluationCategoryHelper.AvailableCategories)
            categoryFilterComboBox.Items.Add(new FilterItem(category.DisplayName, category.Value));
        categoryFilterComboBox.SelectedIndex = 0;
        activityFilterComboBox.Items.Add(new ActivityFilterItem("Все", null));
        activityFilterComboBox.Items.Add(new ActivityFilterItem("Активные", true));
        activityFilterComboBox.Items.Add(new ActivityFilterItem("Неактивные", false));
        activityFilterComboBox.SelectedIndex = 1;
    }

    private void RefreshCriteria()
    {
        if (criterionService is null || IsDisposed || Disposing) return;
        try
        {
            string? category = (categoryFilterComboBox.SelectedItem as FilterItem)?.Value;
            bool? isActive = (activityFilterComboBox.SelectedItem as ActivityFilterItem)?.Value;
            List<EvaluationCriterion> criteria = string.IsNullOrWhiteSpace(searchTextBox.Text)
                ? criterionService.GetAll(isActive, category)
                : criterionService.Search(searchTextBox.Text, isActive, category);
            criteriaGrid.Rows.Clear();
            foreach (EvaluationCriterion criterion in criteria)
            {
                int row = criteriaGrid.Rows.Add(
                    criterion.Name,
                    EvaluationCategoryHelper.GetDisplayName(criterion.Category),
                    $"{criterion.MinimumScore}–{criterion.MaximumScore}",
                    criterion.ManagersOnly ? "Да" : "Нет",
                    criterion.IsActive ? "Активен" : "Неактивен",
                    criterion.SortOrder);
                criteriaGrid.Rows[row].Tag = criterion;
            }
            criteriaGrid.ClearSelection();
            UpdateButtons();
        }
        catch (EvaluationCriterionServiceException exception) { ShowError(exception.Message); }
    }

    private void AddCriterion()
    {
        if (criterionService is null) return;
        using EvaluationCriterionEditForm form = new(criterionService);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshCriteria();
    }

    private void EditCriterion()
    {
        if (criterionService is null) return;
        EvaluationCriterion? selected = GetSelected();
        if (selected is null) { ShowSelectionRequired(); return; }
        try
        {
            EvaluationCriterion? current = criterionService.GetById(selected.Id);
            if (current is null) { ShowError("Критерий больше не существует."); RefreshCriteria(); return; }
            using EvaluationCriterionEditForm form = new(criterionService, current);
            if (form.ShowDialog(this) == DialogResult.OK) RefreshCriteria();
        }
        catch (EvaluationCriterionServiceException exception) { ShowError(exception.Message); }
    }

    private void DeleteOrRestoreCriterion()
    {
        if (criterionService is null) return;
        EvaluationCriterion? selected = GetSelected();
        if (selected is null) { ShowSelectionRequired(); return; }
        try
        {
            if (!selected.IsActive)
            {
                criterionService.Activate(selected.Id);
            }
            else
            {
                if (MessageBox.Show(this,
                        "Удалить выбранный критерий? Если он уже использовался, критерий станет неактивным и останется в истории.",
                        "Критерии", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
                bool deleted = criterionService.DeleteOrDeactivate(selected.Id);
                if (!deleted)
                {
                    MessageBox.Show(this, "Критерий использовался в аттестациях и был переведен в неактивное состояние.",
                        "Критерии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            RefreshCriteria();
        }
        catch (EvaluationCriterionServiceException exception) { ShowError(exception.Message); }
    }

    private EvaluationCriterion? GetSelected() => criteriaGrid.SelectedRows.Count == 1
        ? criteriaGrid.SelectedRows[0].Tag as EvaluationCriterion : null;
    private void UpdateButtons()
    {
        EvaluationCriterion? selected = GetSelected();
        editButton.Enabled = selected is not null;
        deleteButton.Enabled = selected is not null;
        deleteButton.Text = selected?.IsActive == false ? "Восстановить" : "Удалить";
    }
    private void ShowSelectionRequired() => MessageBox.Show(this, "Выберите критерий.", "Критерии", MessageBoxButtons.OK, MessageBoxIcon.Information);
    private void ShowError(string message) => MessageBox.Show(this, message, "Критерии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    private void LoadWindowIcon() { string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "null-icon.ico"); if (File.Exists(path)) Icon = new Icon(path); }
    private sealed record FilterItem(string Text, string? Value) { public override string ToString() => Text; }
    private sealed record ActivityFilterItem(string Text, bool? Value) { public override string ToString() => Text; }
}
