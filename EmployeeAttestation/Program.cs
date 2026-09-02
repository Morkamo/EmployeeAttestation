namespace EmployeeAttestation
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Extra.AppConfigManager configManager = new();
            Data.DatabaseManager databaseManager = new(configManager);
            databaseManager.Initialize();
            Application.Run(new Forms.MainForm(databaseManager));
        }
    }
}
