using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Error_Explorer;

namespace Capa_Error_Explorer_Gui
{
    internal class ErrorDB : SQL
    {
        public List<CapaErrorSummary> GetCapaErrorSummary()
        {
            //TODO: Do not include excluded packages
            string query = "Select \r\n\tPackageID\r\n\t,COUNT(*) AS TotalUnits\r\n\t,SUM(CASE WHEN [Status] = 'Installed' THEN 1 ELSE 0 END) AS StatusInstalledCount\r\n\t,SUM(CASE WHEN [Status] = 'Failed' THEN 1 ELSE 0 END) AS StatusFailedCount\r\n\t,SUM(CASE WHEN [Status] != 'Installed' AND [Status] != 'Failed' THEN 1 ELSE 0 END) AS OtherStatusCount\r\n\t,MAX(PackageName) AS PackageName\r\n\t,MAX(PackageVersion) AS PackageVersion\r\n\t,SUM([ErrorCount]) AS TotalErrorCount\r\n\t,SUM([CancelledCount]) AS TotalCancelledCount\r\nFrom Capa_Errors\r\nGROUP BY PackageID";
            List<CapaErrorSummary> capaErrorSummary = new List<CapaErrorSummary>();
            CapaErrorSummary capaErrorSummaryItem;

            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                capaErrorSummaryItem = new CapaErrorSummary();

                                capaErrorSummaryItem.PackageID = reader.GetInt32(0);
                                capaErrorSummaryItem.TotalUnits = reader.GetInt32(1);
                                capaErrorSummaryItem.StatusInstalledCount = reader.GetInt32(2);
                                capaErrorSummaryItem.StatusFailedCount = reader.GetInt32(3);
                                capaErrorSummaryItem.OtherStatusCount = reader.GetInt32(4);
                                capaErrorSummaryItem.PackageName = reader.GetString(5);
                                capaErrorSummaryItem.PackageVersion = reader.GetString(6);
                                capaErrorSummaryItem.TotalErrorCount = reader.GetInt32(7);
                                capaErrorSummaryItem.TotalCancelledCount = reader.GetInt32(8);

                                capaErrorSummary.Add(capaErrorSummaryItem);
                            }
                        }
                    }
                }

                return capaErrorSummary;
            } catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaErrorSummary: {ex.Message}");
                return null;
            }
        }
    }
}
