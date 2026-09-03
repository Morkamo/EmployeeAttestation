using EmployeeAttestation.Events;

namespace EmployeeAttestation.Controls;

public partial class SidebarControl : UserControl
{
    private readonly List<Image> navigationIcons = [];

    public SidebarControl()
    {
        InitializeComponent();
        LoadNavigationIcons();
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

    private void LoadNavigationIcons()
    {
        foreach (Control control in navigationPanel.Controls)
        {
            if (control is Button button && button.Tag is PageType pageType)
            {
                button.Image = LoadIcon(pageType);
            }
        }
    }

    private Image? LoadIcon(PageType pageType)
    {
        string fileName = pageType switch
        {
            PageType.Home => "home-icon.png",
            PageType.Employees => "employee-icon.png",
            PageType.Attestations => "attestation-icon.png",
            PageType.Commissions => "squad_testers-icon.png",
            PageType.Departments => "departments-icon.png",
            PageType.Positions => "rank-icon.png",
            PageType.Settings => "settings-icon.png",
            _ => "null-icon.png"
        };

        string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Images", fileName);
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", Path.ChangeExtension(fileName, ".ico"));
        string fallbackPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Images", "null-icon.png");

        if (File.Exists(imagePath))
        {
            using Image sourceImage = Image.FromFile(imagePath);
            Bitmap imageIcon = new(sourceImage, new Size(40, 40));
            navigationIcons.Add(imageIcon);
            return imageIcon;
        }

        if (File.Exists(iconPath))
        {
            using Icon sourceIcon = new(iconPath, new Size(40, 40));
            using Bitmap sourceBitmap = sourceIcon.ToBitmap();
            Bitmap icoIcon = new(sourceBitmap, new Size(40, 40));
            navigationIcons.Add(icoIcon);
            return icoIcon;
        }

        if (!File.Exists(fallbackPath))
        {
            return null;
        }

        using Image fallbackImage = Image.FromFile(fallbackPath);
        Bitmap fallbackIcon = new(fallbackImage, new Size(40, 40));
        navigationIcons.Add(fallbackIcon);
        return fallbackIcon;
    }
}
