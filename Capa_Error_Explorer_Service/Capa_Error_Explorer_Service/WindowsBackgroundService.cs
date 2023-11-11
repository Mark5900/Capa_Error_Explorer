namespace Capa_Error_Explorer_Service
{
    public class WindowsBackgroundService : BackgroundService
    {
        bool bDebug = true;

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

                CapaInstallerDB capaInstallerDB = new CapaInstallerDB();
                List<CapaPackage> capaPackages;
                List<CapaUnitJob> capaUnitJobs;
                CapaUnit capaUnit;

                while (!stoppingToken.IsCancellationRequested)
                {

                    capaInstallerDB.SetConnectionString(globalSettings.CapaSQLServer, globalSettings.CapaSQLDB);


                    {
                        capaPackages = capaInstallerDB.GetPackages();

                        if (capaPackages == null)
                        {
                            _fileLogging.WriteErrorLine("Got null from GetPackages");
                            _fileLogging.WriteErrorLine("Wating 10 seconds and trying again");
                            await Task.Delay(10000, stoppingToken);
                            continue;
                        }
                        else
                        {

                            _fileLogging.WriteLine($"GetPackages: {capaPackages.Count}");

                            foreach (CapaPackage capaPackage in capaPackages)
                            {
                                if (bDebug)
                                {
                                    _fileLogging.WriteLine($"Package: {capaPackage.Name} {capaPackage.Version} ID: {capaPackage.ID}");
                                }

                                capaUnitJobs = capaInstallerDB.GetUnitJob(capaPackage.ID, bDebug);

                                if (capaUnitJobs == null)
                                {
                                    _fileLogging.WriteLine($"Got null from GetUnitJobs for package: {capaPackage.Name} {capaPackage.Version}");
                                    continue;
                                }
                                else
                                {
                                    _fileLogging.WriteLine($"GetUnitJobs: {capaUnitJobs.Count}");

                                    foreach (CapaUnitJob capaUnitJob in capaUnitJobs)
                                    {
                                        if (bDebug)
                                        {
                                            _fileLogging.WriteLine($"UnitJob: {capaUnitJob.UnitID} {capaUnitJob.JobID} {capaUnitJob.Status} {capaUnitJob.LastRunDate}");
                                        }

                                        capaUnit = capaInstallerDB.GetUnit(capaUnitJob.UnitID, bDebug);

                                        if (capaUnit == null)
                                        {
                                            _fileLogging.WriteLine($"Got null from GetUnit for UnitID: {capaUnitJob.UnitID}");
                                            continue;
                                        }
                                        else
                                        {
                                            if (bDebug)
                                            {
                                                _fileLogging.WriteLine($"Unit: {capaUnit.Name} {capaUnit.UUID}");
                                            }
                                        }

                                    }
                                }
                            }

                        }
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