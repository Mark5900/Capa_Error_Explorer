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

                FileLogging.WriteLine($"ErrorDB.GetCapaErrorSummary: {capaErrorSummary.Count} rows");

                return capaErrorSummary;
            } catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaErrorSummary: {ex.Message}");
                return null;
            }
        }

        public List<CapaErrorTypeSummary> GetCapaErrorTypeSummary(string PackageName, string PackageVersion)
        {
            string query = $"SELECT CurrentErrorType,\r\n\t[Status],\r\n\tCOUNT(*) AS TotalUnits,\r\n\tSUM([RunCount]) AS TotalRunCount,\r\n\tSUM([ErrorCount]) AS TotalErrorCount,\r\n\tSUM([CancelledCount]) AS TotalCancelledCount,\r\n\tMAX([PackageRecurrence]) AS PackageRecurrence\r\nFROM Capa_Errors\r\nWHERE PackageName = '{PackageName}'\r\n\tAND PackageVersion = '{PackageVersion}'\r\n\tAND [Status] in ('Failed', 'Cancel', 'NotCompliant', 'PostFailed', 'UninstallFailed')\r\nGROUP BY CurrentErrorType, [Status]";
            List<CapaErrorTypeSummary> capaErrorTypeSummary = new List<CapaErrorTypeSummary>();
            CapaErrorTypeSummary capaErrorTypeSummaryItem;

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
                                capaErrorTypeSummaryItem = new CapaErrorTypeSummary();

                                capaErrorTypeSummaryItem.CurrentErrorType = reader.GetString(0);
                                capaErrorTypeSummaryItem.Status = reader.GetString(1);
                                capaErrorTypeSummaryItem.TotalUnits = reader.GetInt32(2);
                                capaErrorTypeSummaryItem.TotalRunCount = reader.GetInt32(3);
                                capaErrorTypeSummaryItem.TotalErrorCount = reader.GetInt32(4);
                                capaErrorTypeSummaryItem.TotalCancelledCount = reader.GetInt32(5);
                                capaErrorTypeSummaryItem.PackageRecurrence = reader.GetString(6);

                                capaErrorTypeSummary.Add(capaErrorTypeSummaryItem);
                            }
                        }
                    }
                }

                FileLogging.WriteLine($"ErrorDB.GetCapaErrorTypeSummary: {PackageName} {PackageVersion} - {capaErrorTypeSummary.Count} rows");

                return capaErrorTypeSummary;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaErrorTypeSummary: {ex.Message}");
                return null;
            }
        }

        public List<CapaError> GetCapaErrors(string packageName, string packageVersion, string currentErrorType)
        {
            string query = $"SELECT [UnitID]\r\n      ,[PackageID]\r\n      ,[Status]\r\n      ,[LastRunDate]\r\n      ,[RunCount]\r\n      ,[CurrentErrorType]\r\n      ,[UnitUUID]\r\n      ,[PackageGUID]\r\n      ,[UnitName]\r\n      ,[PackageName]\r\n      ,[PackageVersion]\r\n      ,[CMPID]\r\n      ,[TYPE]\r\n      ,[ErrorCount]\r\n      ,[LastErrorType]\r\n      ,[CancelledCount]\r\n      ,[PackageRecurrence]\r\n  FROM [Capa_Errors]\r\n WHERE [PackageName] = '{packageName}'\r\n   AND [PackageVersion] = '{packageVersion}'\r\n   AND [CurrentErrorType] = '{currentErrorType}'\r\n   AND [Status] in ('Failed', 'Cancel', 'NotCompliant', 'PostFailed', 'UninstallFailed')\r\n ORDER BY [LastRunDate] DESC";
            List<CapaError> capaError = new List<CapaError>();
            CapaError capaErrorItem;

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
                                capaErrorItem = new CapaError();

                                capaErrorItem.UnitID = reader.GetInt32(0);
                                capaErrorItem.PackageID = reader.GetInt32(1);
                                capaErrorItem.Status = reader.GetString(2);
                                capaErrorItem.LastRunDate = reader.GetInt32(3);
                                capaErrorItem.RunCount = reader.GetInt32(4);
                                capaErrorItem.CurrentErrorType = reader.GetString(5);
                                capaErrorItem.UnitUUID = reader.GetGuid(6);
                                capaErrorItem.PackageGUID = reader.GetGuid(7);
                                capaErrorItem.UnitName = reader.GetString(8);
                                capaErrorItem.PackageName = reader.GetString(9);
                                capaErrorItem.PackageVersion = reader.GetString(10);
                                capaErrorItem.CMPID = reader.GetInt32(11);
                                capaErrorItem.Type = reader.GetInt16(12);
                                capaErrorItem.ErrorCount = reader.GetInt32(13);
                                capaErrorItem.CancelledCount = reader.GetInt32(15);
                                capaErrorItem.PackageRecurrence = reader.GetString(16);

                                // If it is NULL, set it to empty string
                                if (reader.IsDBNull(14))
                                {
                                    capaErrorItem.LastErrorType = "";
                                }
                                else
                                { 
                                    capaErrorItem.LastErrorType = reader.GetString(14);
                                 }

                                capaError.Add(capaErrorItem);
                            }
                        }
                    }
                }

                FileLogging.WriteLine($"ErrorDB.GetCapaError: {packageName} {packageVersion} - {currentErrorType} - {capaError.Count} rows");

                return capaError;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaError: {ex.Message}");
                // Line number with error
                StackTrace st = new StackTrace(new StackFrame(true));
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaError: {st.GetFrame(0).GetFileLineNumber()}");
                return null;
            }
        }

        //public List<CapaError> GetCapaeErrorUnits(string packageName, string packageVersion, string errorType)
        //{
        //    string query = $""
        //}
    }
}
