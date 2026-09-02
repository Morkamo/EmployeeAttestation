using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class CommissionsControl : UserControl
{
    public CommissionsControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(commissionsGrid);
        AppControlStyles.ApplyGrid(membersGrid);
        AppControlStyles.ApplyPrimaryButton(addButton);
        AppControlStyles.ApplySecondaryButton(editButton);
        AppControlStyles.ApplySecondaryButton(editMembersButton);
    }
}
