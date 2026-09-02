namespace EmployeeAttestation.Extra;

public sealed class DatabaseFolderWatcher : IDisposable
{
    private readonly FileSystemWatcher watcher;
    private readonly System.Threading.Timer notificationTimer;
    private volatile bool disposed;

    public DatabaseFolderWatcher(string databaseDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseDirectory);
        Directory.CreateDirectory(databaseDirectory);

        notificationTimer = new System.Threading.Timer(NotifyDatabaseFilesChanged);
        watcher = new FileSystemWatcher(databaseDirectory, "*.db")
        {
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

        watcher.Created += DatabaseFileChanged;
        watcher.Deleted += DatabaseFileChanged;
        watcher.Renamed += DatabaseFileRenamed;
    }

    public event Action? DatabaseFilesChanged;

    private void DatabaseFileChanged(object sender, FileSystemEventArgs e) => ScheduleNotification();

    private void DatabaseFileRenamed(object sender, RenamedEventArgs e) => ScheduleNotification();

    private void ScheduleNotification()
    {
        if (!disposed)
        {
            try
            {
                notificationTimer.Change(350, Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                // Disposal can race with an in-flight FileSystemWatcher callback.
            }
        }
    }

    private void NotifyDatabaseFilesChanged(object? state)
    {
        if (!disposed)
        {
            DatabaseFilesChanged?.Invoke();
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        watcher.EnableRaisingEvents = false;
        watcher.Created -= DatabaseFileChanged;
        watcher.Deleted -= DatabaseFileChanged;
        watcher.Renamed -= DatabaseFileRenamed;
        watcher.Dispose();
        notificationTimer.Dispose();
    }
}
