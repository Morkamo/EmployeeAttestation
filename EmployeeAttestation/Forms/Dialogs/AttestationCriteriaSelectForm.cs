using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class AttestationCriteriaSelectForm : Form
{
    private readonly EvaluationCriterionService? criterionService;
    private readonly HashSet<int> initiallySelectedIds = [];
    private readonly IReadOnlyCollection<AttestationCriterion> existingCriteria = [];
    private readonly bool allowManagerial;

    public AttestationCriteriaSelectForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(criteriaGrid);
        AppControlStyles.ApplyPrimaryButton(saveButton);
        AppControlStyles.ApplySecondaryButton(selectAllButton);
        AppControlStyles.ApplySecondaryButton(clearAllButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        LoadWindowIcon();
    }

    public AttestationCriteriaSelectForm(
        EvaluationCriterionService criterionService,
        IEnumerable<int> selectedCriterionIds,
        bool allowManagerial,
        IReadOnlyCollection<AttestationCriterion>? existingCriteria = null)
        : this()
    {
        this.criterionService = criterionService ?? throw new ArgumentNullException(nameof(criterionService));
        initiallySelectedIds = selectedCriterionIds?.Where(id => id > 0).ToHashSet() ?? [];
        this.allowManagerial = allowManagerial;
        this.existingCriteria = existingCriteria ?? [];
    }

    public IReadOnlyCollection<int> SelectedCriterionIds { get; private set; } = [];

    protected override void OnLoad(EventArgs e) { base.OnLoad(e); LoadCriteria(); }
    private void SelectAllButton_Click(object? sender, EventArgs e) => SetAll(true);
    private void ClearAllButton_Click(object? sender, EventArgs e) => SetAll(false);
    private void SaveButton_Click(object? sender, EventArgs e) => SaveSelection();

    private void LoadCriteria()
    {
        if (criterionService is null) return;
        try
        {
            List<EvaluationCriterion> criteria = criterionService.GetActive(allowManagerial);
            foreach (AttestationCriterion snapshot in existingCriteria)
            {
                if (!snapshot.CriterionId.HasValue
                    || criteria.Any(item => item.Id == snapshot.CriterionId.Value)
                    || (!allowManagerial && snapshot.ManagersOnly)) continue;
                criteria.Add(new EvaluationCriterion
                {
                    Id = snapshot.CriterionId.Value,
                    Code = snapshot.CriterionCode,
                    Name = snapshot.CriterionName,
                    Category = snapshot.Category,
                    MinimumScore = snapshot.MinimumScore,
                    MaximumScore = snapshot.MaximumScore,
                    ManagersOnly = snapshot.ManagersOnly,
                    SortOrder = snapshot.SortOrder,
                    IsActive = false
                });
            }
            criteria = criteria.OrderBy(item => CategoryOrder(item.Category)).ThenBy(item => item.SortOrder).ThenBy(item => item.Name).ToList();
            criteriaGrid.Rows.Clear();
            foreach (EvaluationCriterion criterion in criteria)
            {
                int row = criteriaGrid.Rows.Add(
                    initiallySelectedIds.Contains(criterion.Id),
                    criterion.Name,
                    EvaluationCategoryHelper.GetDisplayName(criterion.Category),
                    $"{criterion.MinimumScore}–{criterion.MaximumScore}",
                    criterion.IsActive ? "" : "Неактивен");
                criteriaGrid.Rows[row].Tag = criterion;
            }
        }
        catch (EvaluationCriterionServiceException exception)
        {
            saveButton.Enabled = false;
            MessageBox.Show(this, exception.Message, "Выбор критериев", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void SetAll(bool selected)
    {
        foreach (DataGridViewRow row in criteriaGrid.Rows)
            row.Cells[0].Value = selected;
    }

    private void SaveSelection()
    {
        criteriaGrid.EndEdit();
        SelectedCriterionIds = criteriaGrid.Rows.Cast<DataGridViewRow>()
            .Where(row => Convert.ToBoolean(row.Cells[0].Value))
            .Select(row => ((EvaluationCriterion)row.Tag!).Id)
            .ToArray();
        DialogResult = DialogResult.OK;
        Close();
    }

    private static int CategoryOrder(string category) => category switch
    {
        EvaluationCategoryHelper.Professional => 1,
        EvaluationCategoryHelper.Personal => 2,
        EvaluationCategoryHelper.Managerial => 3,
        _ => 4
    };

    private void LoadWindowIcon() { string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "null-icon.ico"); if (File.Exists(path)) Icon = new Icon(path); }
}
