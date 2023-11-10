using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    public class GlobalSettings
    {
        public string CapaSQLServer;
        public string CapaSQLDB;
        public string ErrorExplorerSQLServer;
        public string ErrorExplorerSQLDB;
        public string ErrorMsg;

        public GlobalSettings()
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            CapaSQLServer = builder.Build().GetSection("SQLSettings").GetSection("CapaSQLServer").Value;
            CapaSQLDB = builder.Build().GetSection("SQLSettings").GetSection("CapaSQLDB").Value;
            ErrorExplorerSQLServer = builder.Build().GetSection("SQLSettings").GetSection("ErrorExplorerSQLServer").Value;
            ErrorExplorerSQLDB = builder.Build().GetSection("SQLSettings").GetSection("ErrorExplorerSQLDB").Value;
        }
    }
}
