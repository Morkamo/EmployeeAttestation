using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class EvaluationCriterionEditForm : Form
{
    private readonly EvaluationCriterionService? criterionService;
    private readonly EvaluationCriterion? criterion;

    public EvaluationCriterionEditForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(saveButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        categoryComboBox.DisplayMember = nameof(EvaluationCategoryOption.DisplayName);
        categoryComboBox.ValueMember = nameof(EvaluationCategoryOption.Value);
        categoryComboBox.DataSource = EvaluationCategoryHelper.AvailableCategories.ToList();
        LoadWindowIcon();
    }

    public EvaluationCriterionEditForm(
        EvaluationCriterionService criterionService,
        EvaluationCriterion? criterion = null)
        : this()
    {
        this.criterionService = criterionService ?? throw new ArgumentNullException(nameof(criterionService));
        this.criterion = criterion;
        Text = criterion is null ? "Добавить критерий" : "Изменить критерий";
        if (criterion is null) return;
        nameTextBox.Text = criterion.Name;
        categoryComboBox.SelectedValue = criterion.Category;
        minimumScoreNumeric.Value = criterion.MinimumScore;
        maximumScoreNumeric.Value = criterion.MaximumScore;
        managersOnlyCheckBox.Checked = criterion.ManagersOnly;
        sortOrderNumeric.Value = criterion.SortOrder;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (criterionService is null) return;
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            ShowValidation("Введите наименование критерия.", nameTextBox);
            return;
        }
        if (categoryComboBox.SelectedItem is not EvaluationCategoryOption category)
        {
            ShowValidation("Выберите категорию критерия.", categoryComboBox);
            return;
        }
        if (minimumScoreNumeric.Value > maximumScoreNumeric.Value)
        {
            ShowValidation("Минимальный балл не может быть больше максимального.", minimumScoreNumeric);
            return;
        }

        EvaluationCriterion value = new()
        {
            Id = criterion?.Id ?? 0,
            Code = criterion?.Code ?? string.Empty,
            Name = nameTextBox.Text,
            Category = category.Value,
            MinimumScore = Decimal.ToInt32(minimumScoreNumeric.Value),
            MaximumScore = Decimal.ToInt32(maximumScoreNumeric.Value),
            ManagersOnly = managersOnlyCheckBox.Checked,
            SortOrder = Decimal.ToInt32(sortOrderNumeric.Value),
            IsActive = criterion?.IsActive ?? true
        };
        try
        {
            if (value.Id == 0) criterionService.Create(value);
            else criterionService.Update(value);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) when (exception is EvaluationCriterionServiceException or ArgumentException)
        {
            MessageBox.Show(this, exception.Message, "Критерии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ShowValidation(string message, Control control)
    {
        MessageBox.Show(this, message, "Критерии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
    }

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "null-icon.ico");
        if (File.Exists(iconPath)) Icon = new Icon(iconPath);
    }
}
