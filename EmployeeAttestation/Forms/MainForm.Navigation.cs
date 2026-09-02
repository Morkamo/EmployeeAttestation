using EmployeeAttestation.Controls.Pages;
using EmployeeAttestation.Events;

namespace EmployeeAttestation.Forms;

public partial class MainForm
{
    private void OpenPage(PageType pageType)
    {
        UserControl page = pageType switch
        {
            PageType.Home => new HomeControl(),
            PageType.Employees => databaseManager is null
                ? new EmployeesControl()
                : new EmployeesControl(databaseManager),
            PageType.Attestations => new AttestationsControl(),
            PageType.Commissions => databaseManager is null
                ? new CommissionsControl()
                : new CommissionsControl(databaseManager),
            PageType.Departments => databaseManager is null
                ? new DepartmentsControl()
                : new DepartmentsControl(databaseManager),
            PageType.Positions => databaseManager is null
                ? new PositionsControl()
                : new PositionsControl(databaseManager),
            PageType.Settings => databaseManager is null
                ? new SettingsControl()
                : new SettingsControl(databaseManager),
            _ => throw new ArgumentOutOfRangeException(nameof(pageType))
        };

        page.Dock = DockStyle.Fill;
        contentPanel.SuspendLayout();
        while (contentPanel.Controls.Count > 0)
        {
            Control control = contentPanel.Controls[0];
            contentPanel.Controls.RemoveAt(0);
            control.Dispose();
        }
        contentPanel.Controls.Add(page);
        contentPanel.ResumeLayout();
        sidebarControl.SetSelectedPage(pageType);
    }
}
