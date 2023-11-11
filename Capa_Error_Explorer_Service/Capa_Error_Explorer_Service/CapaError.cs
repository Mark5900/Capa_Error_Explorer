﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    internal class CapaError
    {
        public int UnitID { get; set; }
        public int PackageID { get; set; }
        public string Status { get; set; }
        public int LastRunDate { get; set; }
        public int RunCount { get; set; }
        public string CurrentErrorType { get; set; }
        public Guid UnitUUID { get; set; }
        public Guid PackageGUID { get; set; }
        public string UnitName { get; set; }
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public int CMPID { get; set; }
        public Int16 Type { get; set; }
        public int ErrorCount { get; set; }
        public string LastErrorType { get; set; }
        // Resets to 0 when status changes to Installed, but not if job runs allways
        public int CancelledCount { get; set; }
        public string PackageRecurrence { get; set; }
    }
}