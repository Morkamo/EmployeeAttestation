namespace EmployeeAttestation.Forms;

public partial class MainForm
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        sidebarControl.PageSelected += SidebarControl_PageSelected;
    }

    private void SidebarControl_PageSelected(Events.PageType pageType)
    {
        OpenPage(pageType);
    }
}
