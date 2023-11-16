using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    internal class ErrorDB : SQL
    {
        public bool DoesErrorExist(CapaError capaError)
        {
            bool bReturn = false;
            string query = $"SELECT COUNT(*) FROM [Capa_Errors] WHERE UnitID = {capaError.UnitID} AND PackageID = {capaError.PackageID}";

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
                                if (reader.GetInt32(0) > 0)
                                {
                                    bReturn = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.DoesErrorExist: {ex.Message}");
            }

            return bReturn;
        }

        public CapaError GetError(int UnitID, int PackageID)
        {
            CapaError capaError = new CapaError();
            string query = $"SELECT [UnitID],[PackageID],[Status],[LastRunDate],[RunCount],[CurrentErrorType],[UnitUUID],[PackageGUID],[UnitName],[PackageName],[PackageVersion],[CMPID],[TYPE],[ErrorCount],[LastErrorType],[CancelledCount],[PackageRecurrence]FROM [Capa_Errors] WHERE UnitID = {UnitID} AND PackageID = {PackageID}";

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
                                capaError.UnitID = reader.GetInt32(0);
                                capaError.PackageID = reader.GetInt32(1);
                                capaError.Status = reader.GetString(2);
                                capaError.LastRunDate = reader.GetInt32(3);
                                capaError.RunCount = reader.GetInt32(4);
                                capaError.CurrentErrorType = reader.GetString(5);
                                capaError.UnitUUID = reader.GetGuid(6);
                                capaError.PackageGUID = reader.GetGuid(7);
                                capaError.UnitName = reader.GetString(8);
                                capaError.PackageName = reader.GetString(9);
                                capaError.PackageVersion = reader.GetString(10);
                                capaError.CMPID = reader.GetInt32(11);
                                capaError.Type = reader.GetInt32(12);
                                capaError.ErrorCount = reader.GetInt32(13);
                                capaError.LastErrorType = reader.GetString(14);
                                capaError.CancelledCount = reader.GetInt32(15);
                                capaError.PackageRecurrence = reader.GetString(16);
                            }
                        }
                    }
                }

                return capaError;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetError: {ex.Message}");
                return null;
            }
        }

        public void UpdateErrorStatus(CapaError capaError, CapaInstallerDB capaInstallerDB, string currentErrorTypeFromErrorDB)
        {
            string sCurrentErrorType = "NULL";
            string sLastErrorType = "NULL";

            if (string.IsNullOrEmpty(currentErrorTypeFromErrorDB) == false)
            {
                capaError.LastErrorType = currentErrorTypeFromErrorDB;
            }

            // Used to sort away the types we don't want to see logs on
            switch (capaError.Status.ToLower())
            {
                case "not compliant":
                case "notcompliant":
                case "installed":
                    break;
                default:
                    string packageLog = capaInstallerDB.GetPackageLog(capaError.UnitID, capaError.PackageID);
                    capaError.GetErrorType(packageLog);
                    break;
            }

            if (capaError.CurrentErrorType != null)
            {
                sCurrentErrorType = $"'{capaError.CurrentErrorType}'";
            }
            if (capaError.LastErrorType != null)
            {
                sLastErrorType = $"'{capaError.LastErrorType}'";
            }

            string query = @$"UPDATE [Capa_Errors]
                                SET [Status] = '{capaError.Status}',
                                    [LastRunDate] = '{capaError.LastRunDate}',
                                    [RunCount] = [RunCount] + {capaError.RunCount},
                                    [CurrentErrorType] = {sCurrentErrorType},
                                    [UnitUUID] = '{capaError.UnitUUID}',
                                    [PackageGUID] = '{capaError.PackageGUID}',
                                    [UnitName] = '{capaError.UnitName}',
                                    [PackageName] = '{capaError.PackageName}',
                                    [PackageVersion] = '{capaError.PackageVersion}',
                                    [CMPID] = '{capaError.CMPID}',
                                    [TYPE] = '{capaError.Type}',
                                    [ErrorCount] = [ErrorCount] + {capaError.ErrorCount},
                                    [LastErrorType] = {sLastErrorType},
                                    [CancelledCount] = [CancelledCount] + {capaError.CancelledCount},
                                    [PackageRecurrence] = '{capaError.PackageRecurrence}'
                                WHERE [UnitID] = {capaError.UnitID}AND [PackageID] = {capaError.PackageID}";

            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.UpdateErrorStatus: {ex.Message}");
            }
        }

        public void InsertError(CapaError capaError, CapaInstallerDB capaInstallerDB)
        {
            string sCurrentErrorType = "NULL";
            string sLastErrorType = "NULL";

            // Used to sort away the types we don't want to see logs on
            switch (capaError.Status.ToLower())
            {
                case "not compliant":
                case "notcompliant":
                case "installed":
                    break;
                default:
                    string packageLog = capaInstallerDB.GetPackageLog(capaError.UnitID, capaError.PackageID);
                    capaError.GetErrorType(packageLog);
                    break;
            }

            if (capaError.CurrentErrorType != null)
            {
                sCurrentErrorType = $"'{capaError.CurrentErrorType}'";
            }
            if (capaError.LastErrorType != null)
            {
                sLastErrorType = $"'{capaError.LastErrorType}'";
            }

            string query = "INSERT INTO [Capa_Errors] ([UnitID],[PackageID],[Status],[LastRunDate],[RunCount],[CurrentErrorType],[UnitUUID],[PackageGUID],[UnitName],[PackageName],[PackageVersion],[CMPID],[TYPE],[ErrorCount],[LastErrorType],[CancelledCount],[PackageRecurrence])" +
                $"VALUES ('{capaError.UnitID}','{capaError.PackageID}','{capaError.Status}','{capaError.LastRunDate}','{capaError.RunCount}',{sCurrentErrorType},'{capaError.UnitUUID}','{capaError.PackageGUID}','{capaError.UnitName}','{capaError.PackageName}','{capaError.PackageVersion}','{capaError.CMPID}','{capaError.Type}','{capaError.ErrorCount}',{sLastErrorType},'{capaError.CancelledCount}','{capaError.PackageRecurrence}')";

            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.InsertError: {ex.Message}");
            }
        }
    }
}
