using EmployeeAttestation.Events;

namespace EmployeeAttestation.Controls;

public partial class SidebarControl : UserControl
{
    private Image? placeholderIcon;

    public SidebarControl()
    {
        InitializeComponent();
        LoadPlaceholderIcons();
        SetSelectedPage(PageType.Home);
    }

    public event Action<PageType>? PageSelected;

    public void SetSelectedPage(PageType pageType)
    {
        sectionTitleLabel.Text = GetPageTitle(pageType);

        foreach (Control control in navigationPanel.Controls)
        {
            if (control is Button button)
            {
                bool selected = button.Tag is PageType type && type == pageType;
                button.BackColor = selected ? Styles.AppColors.ActiveBackground : Styles.AppColors.Surface;
                button.ForeColor = selected ? Styles.AppColors.Primary : Styles.AppColors.TextPrimary;
                button.FlatAppearance.BorderSize = 0;
            }
        }
    }

    private static string GetPageTitle(PageType pageType) => pageType switch
    {
        PageType.Home => "Главная",
        PageType.Employees => "Сотрудники",
        PageType.Attestations => "Аттестации",
        PageType.Commissions => "Комиссии",
        PageType.Departments => "Подразделения",
        PageType.Positions => "Должности",
        PageType.Settings => "Настройки",
        _ => throw new ArgumentOutOfRangeException(nameof(pageType))
    };

    private void LoadPlaceholderIcons()
    {
        string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Images", "null-icon.png");
        if (!File.Exists(imagePath))
        {
            return;
        }

        using Image sourceImage = Image.FromFile(imagePath);
        placeholderIcon = new Bitmap(sourceImage, new Size(24, 24));
        foreach (Control control in navigationPanel.Controls)
        {
            if (control is Button button)
            {
                button.Image = placeholderIcon;
            }
        }
    }
}
