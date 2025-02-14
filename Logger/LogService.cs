namespace LoggerLib;

public sealed class LogService<T> : ILogService
{
    
    private readonly string _type;
    private readonly string _logFilePath= "shared_logs.txt";
    private static readonly object _lock = new object();

    private static readonly Lazy<LogService<T>> _instance = new(() => new LogService<T>());

    public static LogService<T> Instance => _instance.Value;
    private LogService()
    {
        _type = typeof(T).Name;
        EnsureLogFileExists();
    }
    private void EnsureLogFileExists()
    {
        if (!File.Exists(_logFilePath))
        {
            File.Create(_logFilePath).Dispose();
        }
    }

    private void WriteLog(string logLevel, string message, Exception ex = null)
    {
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message} in class {_type}";
        if (ex != null)
        {
            logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }

        lock (_lock)
        {
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }

    public void LogInformation(string message) => WriteLog("INFO", message);
    public void LogWarning(string message) => WriteLog("WARNING", message);
    public void LogError(string message, Exception ex = null) => WriteLog("ERROR", message, ex);
    public void LogDebug(string message) => WriteLog("DEBUG", message);
    public void LogTrace(string message) => WriteLog("TRACE", message);
}
