using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class PositionEditForm : Form
{
    private readonly PositionService? positionService;
    private readonly int positionId;

    public PositionEditForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(saveButton);
        AppControlStyles.ApplySecondaryButton(cancelButton);
        LoadWindowIcon();
    }

    public PositionEditForm(PositionService positionService, Position? position = null)
        : this()
    {
        this.positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
        positionId = position?.Id ?? 0;
        if (position is null)
        {
            Text = "Добавить должность";
            return;
        }

        Text = "Изменить должность";
        nameTextBox.Text = position.Name;
        managerialCheckBox.Checked = position.IsManagerial;
    }

    private void SaveButton_Click(object? sender, EventArgs e) => SavePosition();

    private void SavePosition()
    {
        if (positionService is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show(
                this,
                "Введите наименование должности.",
                "Должности",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            nameTextBox.Focus();
            return;
        }

        Position position = new()
        {
            Id = positionId,
            Name = nameTextBox.Text,
            IsManagerial = managerialCheckBox.Checked
        };

        try
        {
            if (positionId == 0)
            {
                positionService.Create(position);
            }
            else
            {
                positionService.Update(position);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) when (exception is PositionServiceException or ArgumentException)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Должности",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "null-icon.ico");
        if (File.Exists(iconPath))
        {
            Icon = new Icon(iconPath);
        }
    }
}
