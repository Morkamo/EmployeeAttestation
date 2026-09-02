using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Controls.Pages;

public partial class AttestationsControl : UserControl
{
    public AttestationsControl()
    {
        InitializeComponent();
        AppControlStyles.ApplyGrid(attestationsGrid);
        AppControlStyles.ApplyPrimaryButton(createButton);
        AppControlStyles.ApplySecondaryButton(openButton);
        AppControlStyles.ApplySecondaryButton(editButton);
    }
}
