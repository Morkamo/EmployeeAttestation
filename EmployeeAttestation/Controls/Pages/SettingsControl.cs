using System.ComponentModel;
using System.Diagnostics;
using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;

namespace EmployeeAttestation.Controls.Pages;

public partial class SettingsControl : UserControl
{
    private DatabaseManager? databaseManager;
    private DatabaseFolderWatcher? databaseFolderWatcher;
    private bool refreshingDatabaseList;

    public SettingsControl()
    {
        InitializeComponent();
    }

    public SettingsControl(DatabaseManager databaseManager)
        : this()
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        databaseFolderWatcher = new DatabaseFolderWatcher(databaseManager.DatabasesDirectory);
        databaseFolderWatcher.DatabaseFilesChanged += DatabaseFolderWatcher_DatabaseFilesChanged;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshDatabaseList();
    }

    private void RefreshDatabaseList()
    {
        if (databaseManager is null || IsDisposed || Disposing)
        {
            return;
        }

        string selectedDatabase = databaseManager.EnsureCurrentDatabase();
        IReadOnlyList<string> availableDatabases = databaseManager.GetAvailableDatabases();

        refreshingDatabaseList = true;
        databaseComboBox.BeginUpdate();
        try
        {
            databaseComboBox.Items.Clear();
            databaseComboBox.Items.AddRange(availableDatabases.Cast<object>().ToArray());
            databaseComboBox.SelectedItem = availableDatabases.FirstOrDefault(
                fileName => string.Equals(fileName, selectedDatabase, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            databaseComboBox.EndUpdate();
            refreshingDatabaseList = false;
        }
    }

    private void DatabaseComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (refreshingDatabaseList
            || databaseManager is null
            || databaseComboBox.SelectedItem is not string databaseFileName)
        {
            return;
        }

        if (!databaseManager.TrySelectDatabase(databaseFileName))
        {
            RefreshDatabaseList();
        }
    }

    private void RefreshButton_Click(object? sender, EventArgs e) => RefreshDatabaseList();

    private void OpenFolderButton_Click(object? sender, EventArgs e)
    {
        if (databaseManager is null)
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(databaseManager.DatabasesDirectory);
            Process.Start(new ProcessStartInfo
            {
                FileName = databaseManager.DatabasesDirectory,
                UseShellExecute = true
            });
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or Win32Exception
            or InvalidOperationException)
        {
            MessageBox.Show(
                this,
                "Не удалось открыть папку баз данных.",
                "Аттестация",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void DatabaseFolderWatcher_DatabaseFilesChanged()
    {
        if (IsDisposed || Disposing || !IsHandleCreated)
        {
            return;
        }

        try
        {
            BeginInvoke(RefreshDatabaseList);
        }
        catch (InvalidOperationException)
        {
            // The control can be disposed while a file-system notification is in flight.
        }
    }

    private void DisposeDatabaseWatcher()
    {
        if (databaseFolderWatcher is null)
        {
            return;
        }

        databaseFolderWatcher.DatabaseFilesChanged -= DatabaseFolderWatcher_DatabaseFilesChanged;
        databaseFolderWatcher.Dispose();
        databaseFolderWatcher = null;
        databaseManager = null;
    }
}
