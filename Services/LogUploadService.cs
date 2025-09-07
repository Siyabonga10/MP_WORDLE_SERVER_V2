using Supabase;

public class LogUploadService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<LogUploadService> _logger;

    public LogUploadService(IConfiguration config, ILogger<LogUploadService> logger)
    {
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UploadCompletedLogs();
            await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);
        }
    }

    private async Task UploadCompletedLogs()
{
    try
    {
        var logFiles = Directory.GetFiles("logs", "*.log")
            .Where(f => !IsCurrentLogFile(f)).ToArray();

        _logger.LogWarning("Found {Count} completed log files to upload", logFiles.Length);

        if (!logFiles.Any())
        {
            _logger.LogWarning("No completed log files found");
            return;
        }

        var supabase = new Client(_config["Supabase:Url"]!, _config["Supabase:Key"]);

        foreach (var file in logFiles)
        {
            _logger.LogWarning("Uploading file: {FileName}", Path.GetFileName(file));
            
            await supabase.Storage.From("app-logs").Upload(file, Path.GetFileName(file));
            
            _logger.LogWarning("Successfully uploaded: {FileName}", Path.GetFileName(file));
            
            File.Delete(file);
            
            _logger.LogWarning("Deleted local file: {FileName}", Path.GetFileName(file));
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error uploading logs");
    }
}

    private bool IsCurrentLogFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var currentHour = DateTime.Now.ToString("yyyyMMdd-HH");
        return fileName.Contains(currentHour);
    }
}