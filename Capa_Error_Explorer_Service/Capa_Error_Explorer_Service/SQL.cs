using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    internal class SQL
    {
        private string sConnectionString = string.Empty;

        public void SetConnectionString(string Server, string Database)
        {
            this.sConnectionString = "Data Source=" + Server + ";Initial Catalog=" + Database + ";Integrated Security=SSPI";
        }


    }
}
