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
                ErrorDB errorDB = new ErrorDB();
                List<CapaPackage> capaPackages;
                List<CapaUnitJob> capaUnitJobs;
                CapaUnit capaUnit;
                CapaError capaErrorFromCIDB;
                CapaError capaErrorFromErrDB;

                while (!stoppingToken.IsCancellationRequested)
                {

                    capaInstallerDB.SetConnectionString(globalSettings.CapaSQLServer, globalSettings.CapaSQLDB);
                    errorDB.SetConnectionString(globalSettings.ErrorExplorerSQLServer, globalSettings.ErrorExplorerSQLDB);
                    {
                        capaPackages = new List<CapaPackage>();
                        capaPackages = capaInstallerDB.GetPackages(bDebug);

                        if (capaPackages == null)
                        {
                            _fileLogging.WriteErrorLine("Got null from GetPackages");
                            _fileLogging.WriteErrorLine("Wating 10 seconds and trying again");
                            await Task.Delay(10000, stoppingToken);
                            continue;
                        }

                        _fileLogging.WriteLine($"GetPackages: {capaPackages.Count}");

                        foreach (CapaPackage capaPackage in capaPackages)
                        {
                            if (bDebug)
                            {
                                _fileLogging.WriteLine($"Package: {capaPackage.Name} {capaPackage.Version} ID: {capaPackage.ID} Recurrence: {capaPackage.Recurrence}");
                            }

                            capaUnitJobs = new List<CapaUnitJob>();
                            capaUnitJobs = capaInstallerDB.GetUnitJob(capaPackage.ID, bDebug);

                            if (capaUnitJobs == null)
                            {
                                _fileLogging.WriteLine($"Got null from GetUnitJobs for package: {capaPackage.Name} {capaPackage.Version}");
                                continue;
                            }
                            _fileLogging.WriteLine($"GetUnitJobs: {capaUnitJobs.Count}");

                            foreach (CapaUnitJob capaUnitJob in capaUnitJobs)
                            {
                                if (bDebug)
                                {
                                    _fileLogging.WriteLine($"UnitJob: {capaUnitJob.UnitID} {capaUnitJob.JobID} {capaUnitJob.Status} {capaUnitJob.LastRunDate}");
                                }

                                capaUnit = new CapaUnit();
                                capaUnit = capaInstallerDB.GetUnit(capaUnitJob.UnitID, bDebug);

                                if (capaUnit == null)
                                {
                                    _fileLogging.WriteLine($"Got null from GetUnit for UnitID: {capaUnitJob.UnitID}");
                                    continue;
                                }
                                if (bDebug)
                                {
                                    _fileLogging.WriteLine($"Unit: {capaUnit.Name} {capaUnit.UUID}");
                                }

                                capaErrorFromCIDB = new CapaError();
                                capaErrorFromErrDB = new CapaError();

                                capaErrorFromCIDB.AssignValuesFromCI(capaPackage, capaUnit, capaUnitJob);

                                if (errorDB.DoesErrorExist(capaErrorFromCIDB))
                                {
                                    capaErrorFromErrDB = errorDB.GetError(capaErrorFromCIDB.UnitID, capaErrorFromCIDB.PackageID);

                                    if (capaErrorFromCIDB.LastRunDate != capaErrorFromErrDB.LastRunDate)
                                    {
                                        _fileLogging.WriteLine($"LastRunDate changed from {capaErrorFromErrDB.LastRunDate} to {capaErrorFromCIDB.LastRunDate}");
                                        _fileLogging.WriteLine($"Updating ErrorDB with new status");
                                        errorDB.UpdateErrorStatus(capaErrorFromCIDB, capaInstallerDB, capaErrorFromErrDB.LastErrorType);
                                    }
                                    else
                                    {
                                        _fileLogging.WriteLine($"Status did not change");
                                    }
                                }
                                else
                                {
                                    _fileLogging.WriteLine($"Error does not exist in ErrorDB");
                                    _fileLogging.WriteLine($"Inserting new error into ErrorDB");
                                    errorDB.InsertError(capaErrorFromCIDB, capaInstallerDB);
                                }

                            }

                        }
                    }

                    /*
                     TODO: Clean up in ErrorDB for packages that are no longer in CapaInstaller and units that are no longer in CapaInstaller
                     But also in case of package is unliked from unit.
                    */

                    // TODO: Handle reinstallation of a unit remove it from the DB
                    // TODO Skip when package status is Installing, Uninstalling, Advertised and PostInstalling
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