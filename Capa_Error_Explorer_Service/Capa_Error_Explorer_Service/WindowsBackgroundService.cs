namespace Capa_Error_Explorer_Service
{
    public class WindowsBackgroundService : BackgroundService
    {
        private readonly ILogger<WindowsBackgroundService> _logger;
        private readonly FileLogging _fileLogging = new FileLogging();

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                GlobalSettings globalSettings = new GlobalSettings();
                _fileLogging.WriteLine($"CapaSQLServer: {globalSettings.CapaSQLServer}");
                _fileLogging.WriteLine($"CapaSQLDB: {globalSettings.CapaSQLDB}");
                _fileLogging.WriteLine($"ErrorExplorerSQLServer: {globalSettings.ErrorExplorerSQLServer}");
                _fileLogging.WriteLine($"ErrorExplorerSQLDB: {globalSettings.ErrorExplorerSQLDB}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    Capa oCapa = new Capa();
                    bool bStatus = true;

                    bStatus = oCapa.SetDatabaseSettings(globalSettings.CapaSQLServer, globalSettings.CapaSQLDB);
                    if (bStatus)
                    {
                        _fileLogging.WriteLine($"SetDatabaseSettings: {bStatus}");
                    }
                    else
                    {
                        _fileLogging.WriteLine($"SetDatabaseSettings: {bStatus}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _fileLogging.WriteLine($"Exception: {ex.Message}");
                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}