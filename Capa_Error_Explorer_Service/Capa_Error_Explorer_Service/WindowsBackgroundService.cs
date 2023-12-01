using System.Diagnostics;

namespace Capa_Error_Explorer_Service
{
    public class WindowsBackgroundService : BackgroundService
    {
        bool bDebug = false;

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
                _fileLogging.WriteLine($"CapaSQLServer: {globalSettings.SQLServer}");
                _fileLogging.WriteLine($"CapaSQLDB: {globalSettings.CapaSQLDB}");
                _fileLogging.WriteLine($"ErrorExplorerSQLDB: {globalSettings.ErrorExplorerSQLDB}");
                bDebug = globalSettings.bDebug;

                CapaInstallerDB capaInstallerDB = new CapaInstallerDB();
                ErrorDB errorDB = new ErrorDB();
                List<CapaError> capaErrors;

                while (!stoppingToken.IsCancellationRequested)
                {

                    capaInstallerDB.SetConnectionString(globalSettings.SQLServer, globalSettings.CapaSQLDB);
                    errorDB.SetConnectionString(globalSettings.SQLServer, globalSettings.ErrorExplorerSQLDB);

                    #region Insert things that are not in Capa_Errors tabel
                    capaErrors = errorDB.Get_NotIn_Capa_Error();
                    if (capaErrors != null && capaErrors.Count > 0)
                    {
                        _fileLogging.WriteLine($"Inserting {capaErrors.Count} new rows into Capa_Error");
                        foreach (CapaError capaError in capaErrors)
                        {
                            try
                            {
                                errorDB.InsertError(capaError);
                                _fileLogging.WriteLine($"Inserted PackageID: {capaError.PackageID} UnitID: {capaError.UnitID}");
                            }
                            catch (Exception ex)
                            {
                                _fileLogging.WriteErrorLine($"Exception: {ex.Message}");
                                _logger.LogError(ex, "{Message}", ex.Message);
                            }
                        }
                    }

                    #endregion
                    #region Update Capa_Error table with new values from CapaInstaller
                    capaErrors = errorDB.Get_LastRunDate_Has_Changed();

                    if (capaErrors != null && capaErrors.Count > 0)
                    {
                        _fileLogging.WriteLine($"Updating {capaErrors.Count} rows in Capa_Error");
                        foreach (CapaError capaError in capaErrors)
                        {
                            try
                            {
                                errorDB.UpdateErrorStatus(capaError, bDebug);
                                _fileLogging.WriteLine($"Updated PackageID: {capaError.PackageID} UnitID: {capaError.UnitID}");
                            }
                            catch (Exception ex)
                            {
                                _fileLogging.WriteErrorLine($"Exception: {ex.Message}");
                                _logger.LogError(ex, "{Message}", ex.Message);
                            }
                        }
                    }
                    #endregion

                    /*
                     TODO: Clean up in ErrorDB for packages that are no longer in CapaInstaller and units that are no longer in CapaInstaller
                     But also in case of package is unliked from unit.
                    */

                    // TODO: Handle reinstallation of a unit remove it from the DB
                    // TODO Skip when package status is Installing, Uninstalling, Advertised and PostInstalling

                    _fileLogging.WriteLine("DONE");
                    _fileLogging.WriteLine("");
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