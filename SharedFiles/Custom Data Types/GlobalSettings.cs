using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    public class GlobalSettings
    {
        public string SQLServer { get; set; }
        public string CapaSQLDB { get; set; }
        public string ErrorExplorerSQLDB { get; set; }
        public bool bDebug { get; set; }

        internal FileLogging FileLogging = new FileLogging();

        public GlobalSettings()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                                   .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                FileLogging.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
                FileLogging.WriteLine($"Json contents: {builder.Build().ToString()}");

                SQLServer = builder.Build().GetSection("SQLSettings").GetSection("SQLServer").Value;
                CapaSQLDB = builder.Build().GetSection("SQLSettings").GetSection("CapaSQLDB").Value;
                ErrorExplorerSQLDB = builder.Build().GetSection("SQLSettings").GetSection("ErrorExplorerSQLDB").Value;
                bDebug = Convert.ToBoolean(builder.Build().GetSection("Logging").GetSection("Debug").Value);
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"GlobalSettings: {ex.Message}");
            }
        }
    }
}
