namespace EmployeeAttestation.Styles;

public static class AppControlStyles
{
    public static void ApplyGrid(DataGridView grid)
    {
        grid.BackgroundColor = AppColors.Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersHeight = 46;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.EnableHeadersVisualStyles = false;
        grid.GridColor = AppColors.Border;
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        grid.ColumnHeadersDefaultCellStyle.BackColor = AppColors.HeaderBackground;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = AppColors.TextPrimary;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = AppColors.HeaderBackground;
        grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        grid.DefaultCellStyle.BackColor = AppColors.Surface;
        grid.DefaultCellStyle.ForeColor = AppColors.TextPrimary;
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
        grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        grid.DefaultCellStyle.SelectionBackColor = AppColors.ActiveBackground;
        grid.DefaultCellStyle.SelectionForeColor = AppColors.TextPrimary;
        grid.RowTemplate.Height = 46;
    }

    public static void ApplyReadableGrid(DataGridView grid)
    {
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        grid.ScrollBars = ScrollBars.Both;
    }

    public static void ApplyPrimaryButton(Button button)
    {
        button.BackColor = AppColors.Primary;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = AppColors.Primary;
        button.FlatAppearance.BorderSize = 0;
        button.Font = new Font("Segoe UI Semibold", 10F);
        button.UseVisualStyleBackColor = false;
    }

    public static void ApplySecondaryButton(Button button)
    {
        button.BackColor = AppColors.Surface;
        button.ForeColor = AppColors.Primary;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = AppColors.Primary;
        button.FlatAppearance.BorderSize = 1;
        button.Font = new Font("Segoe UI Semibold", 10F);
        button.UseVisualStyleBackColor = false;
    }

    public static void ApplyAutoSizeButton(Button button, int minimumWidth = 0)
    {
        button.AutoSize = true;
        button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button.MinimumSize = new Size(minimumWidth, 42);
        button.Padding = new Padding(16, 0, 16, 0);
    }
}
