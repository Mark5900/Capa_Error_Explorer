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
        public List<CapaErrorSummary> GetCapaErrorSummary(string cmpId = "All")
        {
            //TODO: Do not include excluded packages
            //TODO: GROUP BY type (Computer/User)
            string query = "Select  PackageID ,COUNT(*) AS TotalUnits ,SUM(CASE WHEN [Status] = 'Installed' THEN 1 ELSE 0 END) AS StatusInstalledCount ,SUM(CASE WHEN [Status] = 'Failed' THEN 1 ELSE 0 END) AS StatusFailedCount ,SUM(CASE WHEN [Status] != 'Installed' AND [Status] != 'Failed' THEN 1 ELSE 0 END) AS OtherStatusCount ,MAX(PackageName) AS PackageName ,MAX(PackageVersion) AS PackageVersion ,SUM([ErrorCount]) AS TotalErrorCount ,SUM([CancelledCount]) AS TotalCancelledCount From Capa_Errors GROUP BY PackageID";

            if (cmpId != "All")
            {
                query = $"Select  PackageID ,COUNT(*) AS TotalUnits ,SUM(CASE WHEN [Status] = 'Installed' THEN 1 ELSE 0 END) AS StatusInstalledCount ,SUM(CASE WHEN [Status] = 'Failed' THEN 1 ELSE 0 END) AS StatusFailedCount ,SUM(CASE WHEN [Status] != 'Installed' AND [Status] != 'Failed' THEN 1 ELSE 0 END) AS OtherStatusCount ,MAX(PackageName) AS PackageName ,MAX(PackageVersion) AS PackageVersion ,SUM([ErrorCount]) AS TotalErrorCount ,SUM([CancelledCount]) AS TotalCancelledCount From Capa_Errors WHERE CMPID = {cmpId} GROUP BY PackageID";
            }

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

                FileLogging.WriteLine($"ErrorDB.GetCapaErrorSummary: {capaErrorSummary.Count} rows : cmpId {cmpId}");

                return capaErrorSummary;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaErrorSummary (CmpID {cmpId}): {ex.Message}");
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaErrorSummary: {query}");
                return null;
            }
        }

        public List<CapaErrorTypeSummary> GetCapaErrorTypeSummary(string PackageName, string PackageVersion, string cmpId)
        {
            string query = $"SELECT CurrentErrorType, [Status], COUNT(*) AS TotalUnits, SUM([RunCount]) AS TotalRunCount, SUM([ErrorCount]) AS TotalErrorCount, SUM([CancelledCount]) AS TotalCancelledCount, MAX([PackageRecurrence]) AS PackageRecurrence FROM Capa_Errors WHERE PackageName = '{PackageName}' AND PackageVersion = '{PackageVersion}' AND [Status] in ('Failed', 'Cancel', 'NotCompliant', 'PostFailed', 'UninstallFailed') GROUP BY CurrentErrorType, [Status]";
            if (cmpId != "All")
            {
                query = $"SELECT CurrentErrorType, [Status], COUNT(*) AS TotalUnits, SUM([RunCount]) AS TotalRunCount, SUM([ErrorCount]) AS TotalErrorCount, SUM([CancelledCount]) AS TotalCancelledCount, MAX([PackageRecurrence]) AS PackageRecurrence FROM Capa_Errors WHERE PackageName = '{PackageName}' AND PackageVersion = '{PackageVersion}' AND CMPID = {cmpId} AND [Status] in ('Failed', 'Cancel', 'NotCompliant', 'PostFailed', 'UninstallFailed') GROUP BY CurrentErrorType, [Status]";
            }

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

                FileLogging.WriteLine($"ErrorDB.GetCapaErrorTypeSummary: {PackageName} {PackageVersion} - {capaErrorTypeSummary.Count} rows : cmpId {cmpId}");

                return capaErrorTypeSummary;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetCapaErrorTypeSummary: {ex.Message}");
                return null;
            }
        }

        public List<CapaError> GetCapaErrors(string packageName, string packageVersion, string currentErrorType, string cmpId)
        {
            string query = $"SELECT [UnitID]       ,[PackageID]       ,[Status]       ,[LastRunDate]       ,[RunCount]       ,[CurrentErrorType]       ,[UnitUUID]       ,[PackageGUID]       ,[UnitName]       ,[PackageName]       ,[PackageVersion]       ,[CMPID]       ,[TYPE]       ,[ErrorCount]       ,[LastErrorType]       ,[CancelledCount]       ,[PackageRecurrence]   FROM [Capa_Errors]  WHERE [PackageName] = '{packageName}'    AND [PackageVersion] = '{packageVersion}'    AND [CurrentErrorType] = '{currentErrorType}'    AND [Status] in ('Failed', 'Cancel', 'NotCompliant', 'PostFailed', 'UninstallFailed')  ORDER BY [LastRunDate] DESC";
            if (cmpId != "All")
            {
                query = $"SELECT [UnitID]       ,[PackageID]       ,[Status]       ,[LastRunDate]       ,[RunCount]       ,[CurrentErrorType]       ,[UnitUUID]       ,[PackageGUID]       ,[UnitName]       ,[PackageName]       ,[PackageVersion]       ,[CMPID]       ,[TYPE]       ,[ErrorCount]       ,[LastErrorType]       ,[CancelledCount]       ,[PackageRecurrence]   FROM [Capa_Errors]  WHERE [PackageName] = '{packageName}'    AND [PackageVersion] = '{packageVersion}'    AND [CurrentErrorType] = '{currentErrorType}'    AND [CMPID] = {cmpId}    AND [Status] in ('Failed', 'Cancel', 'NotCompliant', 'PostFailed', 'UninstallFailed')  ORDER BY [LastRunDate] DESC";
            }

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

                FileLogging.WriteLine($"ErrorDB.GetCapaError: {packageName} {packageVersion} - {currentErrorType} - {capaError.Count} rows : cmpId {cmpId}");

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

        public List<CapaErrorsExcludedPackages> GetCapaErrorsExcludedPackages()
        {
            string query = "SELECT Capa_Errors.PackageName, Capa_Errors.PackageVersion, Capa_Errors.[TYPE], CAST(CASE WHEN MAX(Capa_Errors_Excluded_GUI.PackageName) IS NOT NULL THEN 1 ELSE 0 END AS BIT)AS IsExcluded, CAST (CASE WHEN MAX(Capa_Errors.[TYPE]) = 1 THEN 'Computer' ELSE 'User' END AS  [varchar](8)) AS TypePrettie FROM Capa_Errors LEFT JOIN Capa_Errors_Excluded_GUI ON  Capa_Errors.PackageName = Capa_Errors_Excluded_GUI.PackageName AND Capa_Errors.PackageVersion = Capa_Errors_Excluded_GUI.PackageVersion AND Capa_Errors.[TYPE] = Capa_Errors_Excluded_GUI.[TYPE] GROUP BY  Capa_Errors.PackageName, Capa_Errors.PackageVersion, Capa_Errors.[TYPE]";

            List<CapaErrorsExcludedPackages> capaErrorsExcludedPackages = new List<CapaErrorsExcludedPackages>();
            CapaErrorsExcludedPackages capaErrorsExcludedPackagesItem;

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
                                capaErrorsExcludedPackagesItem = new CapaErrorsExcludedPackages();

                                capaErrorsExcludedPackagesItem.PackageName = reader.GetString(0);
                                capaErrorsExcludedPackagesItem.PackageVersion = reader.GetString(1);
                                capaErrorsExcludedPackagesItem.Type = reader.GetInt16(2);
                                capaErrorsExcludedPackagesItem.IsExcluded = reader.GetBoolean(3);
                                capaErrorsExcludedPackagesItem.TypePrettie = reader.GetString(4);

                                capaErrorsExcludedPackages.Add(capaErrorsExcludedPackagesItem);
                            }
                        }
                    }
                }

                FileLogging.WriteLine($"ErrorDB.capaErrorsExcludedPackages: {capaErrorsExcludedPackages.Count} rows");

                return capaErrorsExcludedPackages;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.capaErrorsExcludedPackages: {ex.Message}");
                return null;
            }
        }

        public void SaveCapaErrorsExcludedPackages(List<CapaErrorsExcludedPackages> capaErrorsExcludedPackages)
        {
            int countAdded = 0;
            int countDeleted = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    foreach (CapaErrorsExcludedPackages item in capaErrorsExcludedPackages)
                    {
                        if (item.IsExcluded)
                        {
                            string query = $"INSERT INTO Capa_Errors_Excluded_GUI (PackageName, PackageVersion, [TYPE]) VALUES ('{item.PackageName}', '{item.PackageVersion}', {item.Type})";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            countAdded++;
                        }
                        else
                        {
                            string query = $"DELETE FROM Capa_Errors_Excluded_GUI WHERE PackageName = '{item.PackageName}' AND PackageVersion = '{item.PackageVersion}' AND [TYPE] = {item.Type}";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            countDeleted++;
                        }
                    }
                }

                FileLogging.WriteLine($"ErrorDB.SaveCapaErrorsExcludedPackages: {countAdded} added, {countDeleted} deleted");
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.capaErrorsExcludedPackages: {ex.Message}");
            }
        }
    }
}
