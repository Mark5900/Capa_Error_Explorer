using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    internal class Capa
    {
        internal FileLogging FileLogging = new FileLogging();
        internal string sConnectionString = "Persist Security Info=False;Trusted_Connection=True;database=AdventureWorks;server=(local);Encrypt=True;";

        public void SetConnectionString(string Server, string Database)
        {
            this.sConnectionString = $"Trusted_Connection=True; database={Database}; server={Server};TrustServerCertificate=True;";
        }
    }
}
