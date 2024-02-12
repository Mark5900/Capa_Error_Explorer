using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer
{
    internal class CapaErrorsExcludedPackages
    {
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public bool IsExcluded { get; set; }
        public int PackageID { get; set; }
    }
}
