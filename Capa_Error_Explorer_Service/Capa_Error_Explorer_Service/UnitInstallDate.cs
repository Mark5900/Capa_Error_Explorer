using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    internal class UnitInstallDate
    {
        public int UnitID { get; set; }
        public string VALUE { get; set; }

        public void ResetObj()
        {
            this.UnitID = 0;
            this.VALUE = "";
        }
    }
}
