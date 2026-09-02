using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class PositionsControl : UserControl
{
    public PositionsControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(positionsGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(deleteButton);
    }
}
