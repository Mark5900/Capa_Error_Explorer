using CapaInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    internal class Capa
    {
        private FileLogging FileLogging = new FileLogging();
        private SDK oSDK = new SDK();

        public bool SetDatabaseSettings (string Server, string Database)
        {
            bool bReturn = false;

            try
            {
                bReturn= oSDK.SetDatabaseSettings(Server, Database, false);
            }
            catch (Exception ex)
            {
                 FileLogging.WriteLine($"Exception: {ex.Message}");
            }

            return bReturn;
        }
    }
}
