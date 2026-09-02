using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class EmployeesControl : UserControl
{
    public EmployeesControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(employeesGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(archiveButton);
    }
}
