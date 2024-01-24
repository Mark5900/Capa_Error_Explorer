using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer
{
    internal class CapaUnitJob
    {
        public int UnitID { get; set; }
        public int JobID { get; set; }
        public string Status { get; set; }
        public int LastRunDate { get; set; }
    }
}
