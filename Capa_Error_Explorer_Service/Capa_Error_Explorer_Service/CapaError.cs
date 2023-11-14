using System;
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
        public int Type { get; set; }
        public int ErrorCount { get; set; }
        public string LastErrorType { get; set; }
        // Resets to 0 when status changes to Installed, but not if job runs allways
        public int CancelledCount { get; set; }
        public string PackageRecurrence { get; set; }

        public void ResetObj()
        {
            this.UnitID = 0;
            this.PackageID = 0;
            this.Status = "";
            this.LastRunDate = 0;
            this.RunCount = 0;
            this.CurrentErrorType = "";
            this.UnitUUID = Guid.Empty;
            this.PackageGUID = Guid.Empty;
            this.UnitName = "";
            this.PackageName = "";
            this.PackageVersion = "";
            this.CMPID = 0;
            this.Type = 0;
            this.ErrorCount = 0;
            this.LastErrorType = "";
            this.CancelledCount = 0;
            this.PackageRecurrence = "";
        }

        public void AssignValuesFromCI(CapaPackage Package, CapaUnit Unit, CapaUnitJob UnitJob)
        {
            this.UnitID = Unit.UnitID;
            this.PackageID = Package.ID;
            this.Status = UnitJob.Status;
            this.LastRunDate = UnitJob.LastRunDate;
            this.UnitUUID = Unit.UUID;
            this.PackageGUID = Package.GUID;
            this.UnitName = Unit.Name;
            this.PackageName = Package.Name;
            this.PackageVersion = Package.Version;
            this.CMPID = Package.CMPID;
            this.Type = Package.Type;
            this.PackageRecurrence = Package.Recurrence;

            if (UnitJob.LastRunDate != null)
            {
                this.RunCount = 1;
            }
            if (UnitJob.Status == "Cancelled")
            {
                this.CancelledCount = 1;
            }
            if (UnitJob.Status == "Failed" || UnitJob.Status == "PostFailed")
            {
                this.ErrorCount = 1;
            }
        }

        public string GetErrorType(string PackageLog)
        {
            return "Unknown";
        }

    }
}
