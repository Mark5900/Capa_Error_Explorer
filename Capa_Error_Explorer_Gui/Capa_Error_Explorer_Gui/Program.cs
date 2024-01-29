using System.Diagnostics;

namespace Capa_Error_Explorer_Gui
{
    internal static class Program
    {

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FileLogging _fileLogging = new FileLogging();

            GlobalSettings globalSettings = new GlobalSettings();
            _fileLogging.WriteLine($"CapaSQLServer: {globalSettings.SQLServer}");
            _fileLogging.WriteLine($"CapaSQLDB: {globalSettings.CapaSQLDB}");
            _fileLogging.WriteLine($"ErrorExplorerSQLDB: {globalSettings.ErrorExplorerSQLDB}");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}