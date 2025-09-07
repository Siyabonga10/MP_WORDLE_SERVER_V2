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
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task UploadCompletedLogs()
{
    try
    {
        var logFiles = Directory.GetFiles("logs", "*.log")
            .Where(f => !IsCurrentLogFile(f)).ToArray();


        if (!logFiles.Any())
        {
            return;
        }

        var supabase = new Client(_config["Supabase:Url"]!, _config["Supabase:Key"]);

        foreach (var file in logFiles)
        {
            
            await supabase.Storage.From("app-logs").Upload(file, Path.GetFileName(file));
            File.Delete(file);
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