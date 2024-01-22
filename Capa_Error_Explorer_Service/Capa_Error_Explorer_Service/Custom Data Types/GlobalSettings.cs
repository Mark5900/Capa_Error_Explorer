using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    public class GlobalSettings
    {
        public string SQLServer;
        public string CapaSQLDB;
        public string ErrorExplorerSQLDB;
        public bool bDebug;

        public GlobalSettings()
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            SQLServer = builder.Build().GetSection("SQLSettings").GetSection("SQLServer").Value;
            CapaSQLDB = builder.Build().GetSection("SQLSettings").GetSection("CapaSQLDB").Value;
            ErrorExplorerSQLDB = builder.Build().GetSection("SQLSettings").GetSection("ErrorExplorerSQLDB").Value;
            bDebug = Convert.ToBoolean(builder.Build().GetSection("Logging").GetSection("Debug").Value);
        }
    }
}
