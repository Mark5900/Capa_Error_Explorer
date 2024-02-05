using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Gui
{
    internal class CapaErrorTypeSummary
    {
        public string CurrentErrorType { get; set; }
        public string Status { get; set; }
        public int TotalUnits { get; set; }
        public int TotalRunCount { get; set; }
        public int TotalErrorCount { get; set; }
        public int TotalCancelledCount { get; set; }
        public string PackageRecurrence { get; set; }
    }
}
