using EmployeeAttestation.Data;
using EmployeeAttestation.Events;
using EmployeeAttestation.Extra;

namespace EmployeeAttestation.Forms;

public partial class MainForm : Form
{
    private readonly DatabaseManager? databaseManager;

    public MainForm()
    {
        databaseManager = null;
        InitializeMainForm();
    }

    public MainForm(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        InitializeMainForm();
    }

    private void InitializeMainForm()
    {
        InitializeComponent();
        Text = GeneralProperties.WindowTitle;
        LoadWindowIcon();
        OpenPage(PageType.Home);
    }

    private void LoadWindowIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico");
        if (File.Exists(iconPath))
        {
            Icon = new Icon(iconPath);
        }
    }
}
