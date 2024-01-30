using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Gui
{
    public class CapaErrorSummary
    {
        public int PackageID { get; set; }
        public int TotalUnits { get; set; }
        public int StatusInstalledCount { get; set; }
        public int StatusFailedCount { get; set; }
        public int OtherStatusCount { get; set; }
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public int TotalErrorCount { get; set; }
        public int TotalCancelledCount { get; set; }
    }
}
