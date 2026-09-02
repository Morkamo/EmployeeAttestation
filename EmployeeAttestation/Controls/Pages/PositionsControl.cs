using EmployeeAttestation.Data;
using EmployeeAttestation.Forms.Dialogs;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class PositionsControl : UserControl
{
    private readonly PositionService? positionService;

    public PositionsControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(positionsGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(deleteButton);
    }

    public PositionsControl(DatabaseManager databaseManager)
        : this()
    {
        positionService = new PositionService(
            databaseManager ?? throw new ArgumentNullException(nameof(databaseManager)));
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshPositions();
    }

    private void SearchTextBox_TextChanged(object? sender, EventArgs e) => RefreshPositions();

    private void AddButton_Click(object? sender, EventArgs e) => AddPosition();

    private void EditButton_Click(object? sender, EventArgs e) => EditSelectedPosition();

    private void DeleteButton_Click(object? sender, EventArgs e) => DeleteSelectedPosition();

    private void PositionsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) EditSelectedPosition();
    }

    private void RefreshPositions()
    {
        if (positionService is null || IsDisposed || Disposing) return;
        try
        {
            List<Position> positions = string.IsNullOrWhiteSpace(searchTextBox.Text)
                ? positionService.GetAll()
                : positionService.Search(searchTextBox.Text);
            positionsGrid.Rows.Clear();
            foreach (Position position in positions)
            {
                int rowIndex = positionsGrid.Rows.Add(position.Name, position.IsManagerial ? "Да" : "Нет");
                positionsGrid.Rows[rowIndex].Tag = position;
            }
            positionsGrid.ClearSelection();
        }
        catch (PositionServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private void AddPosition()
    {
        if (positionService is null) return;
        using PositionEditForm form = new(positionService);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshPositions();
    }

    private void EditSelectedPosition()
    {
        if (positionService is null) return;
        Position? position = GetSelectedPosition();
        if (position is null)
        {
            ShowSelectionRequired();
            return;
        }
        using PositionEditForm form = new(positionService, position);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshPositions();
    }

    private void DeleteSelectedPosition()
    {
        if (positionService is null) return;
        Position? position = GetSelectedPosition();
        if (position is null)
        {
            ShowSelectionRequired();
            return;
        }
        if (MessageBox.Show(
                this,
                "Удалить выбранную должность?",
                "Должности",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }
        try
        {
            positionService.Delete(position.Id);
            RefreshPositions();
        }
        catch (PositionServiceException exception)
        {
            ShowServiceError(exception.Message);
        }
    }

    private Position? GetSelectedPosition() => positionsGrid.SelectedRows.Count == 1
        ? positionsGrid.SelectedRows[0].Tag as Position
        : null;

    private void ShowSelectionRequired()
    {
        MessageBox.Show(this, "Выберите должность.", "Должности", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowServiceError(string message)
    {
        MessageBox.Show(this, message, "Должности", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
