using EmployeeAttestation.Events;

namespace EmployeeAttestation.Controls;

public partial class SidebarControl
{
    private void navigationButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button { Tag: PageType pageType })
        {
            SetSelectedPage(pageType);
            PageSelected?.Invoke(pageType);
        }
    }
}
