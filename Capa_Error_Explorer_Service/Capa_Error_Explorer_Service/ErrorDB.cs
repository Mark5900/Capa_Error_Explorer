using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.DoesErrorExist: {ex.Message}");
            }

            return bReturn;
        }

        public CapaError GetError(int UnitID, int PackageID, bool bDebug)
        {
            CapaError capaError = new CapaError();
            string query = $"SELECT [UnitID],[PackageID],[Status],[LastRunDate],[RunCount],[CurrentErrorType],[UnitUUID],[PackageGUID],[UnitName],[PackageName],[PackageVersion],[CMPID],[TYPE],[ErrorCount],[LastErrorType],[CancelledCount],[PackageRecurrence]FROM [Capa_Errors] WHERE UnitID = {UnitID} AND PackageID = {PackageID}";

            if (bDebug)
            {
                FileLogging.WriteLine($"Query: {query}");
            }

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
                                capaError.UnitUUID = reader.GetGuid(6);
                                capaError.PackageGUID = reader.GetGuid(7);
                                capaError.UnitName = reader.GetString(8);
                                capaError.PackageName = reader.GetString(9);
                                capaError.PackageVersion = reader.GetString(10);
                                capaError.CMPID = reader.GetInt32(11);
                                capaError.Type = reader.GetInt16(12);
                                capaError.ErrorCount = reader.GetInt32(13);
                                capaError.CancelledCount = reader.GetInt32(15);
                                capaError.PackageRecurrence = reader.GetString(16);

                                if (reader.IsDBNull(5))
                                {
                                    capaError.CurrentErrorType = null;
                                }
                                else
                                {
                                    capaError.CurrentErrorType = reader.GetString(5);
                                }

                                if (reader.IsDBNull(14))
                                {
                                    capaError.LastErrorType = null;
                                }
                                else
                                {
                                    capaError.LastErrorType = reader.GetString(14);
                                }

                            }
                        }
                    }

                    connection.Close();
                }

                if (bDebug)
                {
                    FileLogging.WriteLine($"GetError : UnitID: {capaError.UnitID} PackageID: {capaError.PackageID} Status: {capaError.Status} LastRunDate: {capaError.LastRunDate} RunCount: {capaError.RunCount} CurrentErrorType: {capaError.CurrentErrorType} UnitUUID: {capaError.UnitUUID} PackageGUID: {capaError.PackageGUID} UnitName: {capaError.UnitName} PackageName: {capaError.PackageName} PackageVersion: {capaError.PackageVersion} CMPID: {capaError.CMPID} Type: {capaError.Type} ErrorCount: {capaError.ErrorCount} LastErrorType: {capaError.LastErrorType} CancelledCount: {capaError.CancelledCount} PackageRecurrence: {capaError.PackageRecurrence}");
                }

                return capaError;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.GetError: {ex.Message}");
                return null;
            }
        }

        public void UpdateErrorStatus(CapaError capaErrorNewData, bool bDebug)
        {
            CapaError capaErrorFromErrorDB = this.GetError(capaErrorNewData.UnitID, capaErrorNewData.PackageID, bDebug);

            if (string.IsNullOrEmpty(capaErrorFromErrorDB.CurrentErrorType) == false)
            {
                capaErrorFromErrorDB.LastErrorType = capaErrorFromErrorDB.CurrentErrorType;
            }

            capaErrorFromErrorDB.Status = capaErrorNewData.Status;
            capaErrorFromErrorDB.LastRunDate = capaErrorNewData.LastRunDate;
            capaErrorFromErrorDB.Log = capaErrorNewData.Log;
            capaErrorFromErrorDB.RunCount = capaErrorFromErrorDB.RunCount + 1;

            switch (capaErrorNewData.Status.ToLower())
            {
                case "cancel":
                case "uninstallcancel":
                    capaErrorFromErrorDB.CancelledCount = capaErrorFromErrorDB.CancelledCount + 1;
                    break;
                case "failed":
                case "postfailed":
                case "uninstallfailed":
                    capaErrorFromErrorDB.ErrorCount = capaErrorFromErrorDB.ErrorCount + 1;
                    break;
            }

            capaErrorFromErrorDB.SetErrorType();

            string query = $"UPDATE [Capa_Errors] SET [Status] = '{capaErrorFromErrorDB.Status}',[LastRunDate] = {capaErrorFromErrorDB.LastRunDate},[RunCount] = {capaErrorFromErrorDB.RunCount},[CurrentErrorType] = '{capaErrorFromErrorDB.CurrentErrorType}',[ErrorCount] = {capaErrorFromErrorDB.ErrorCount},[LastErrorType] = '{capaErrorFromErrorDB.LastErrorType}',[CancelledCount] = {capaErrorFromErrorDB.CancelledCount} WHERE UnitID = {capaErrorFromErrorDB.UnitID} AND PackageID = {capaErrorFromErrorDB.PackageID}";

            if (bDebug)
            {
                FileLogging.WriteLine($"Query: {query}");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"UnitID: {capaErrorFromErrorDB.UnitID} PackageID: {capaErrorFromErrorDB.PackageID}");
                FileLogging.WriteErrorLine($"ErrorDB.UpdateErrorStatus: {ex.Message}");
            }
        }

        public void InsertError(CapaError capaError)
        {
            string sCurrentErrorType = "NULL";
            string sLastErrorType = "NULL";

            capaError.SetErrorType();

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

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.InsertError: {ex.Message}");
            }
        }

        public List<CapaError> Get_NotIn_Capa_Error()
        {
            string query = "SELECT TOP (1000) [UNITID],[PackageID],[STATUS],[LASTRUNDATE],[LOG],[UnitUUID],[UnitName],[TYPE],[PackageGUID],[PackageName],[PackageVersion],[RECURRENCE] FROM [V_CE_NotIn_Capa_Errors]";
            List<CapaError> capaErrors = new List<CapaError>();
            CapaError capaError = new CapaError();

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
                                capaError = new CapaError();

                                capaError.UnitID = reader.GetInt32(0);
                                capaError.PackageID = reader.GetInt32(1);
                                capaError.Status = reader.GetString(2);
                                capaError.UnitUUID = reader.GetGuid(5);
                                capaError.UnitName = reader.GetString(6);
                                capaError.Type = reader.GetInt32(7);
                                capaError.PackageGUID = reader.GetGuid(8);
                                capaError.PackageName = reader.GetString(9);
                                capaError.PackageVersion = reader.GetString(10);
                                capaError.PackageRecurrence = reader.GetString(11);

                                if (!reader.IsDBNull(3))
                                {
                                    capaError.LastRunDate = reader.GetInt32(3);
                                }
                                if (!reader.IsDBNull(4))
                                {
                                    capaError.Log = reader.GetString(4);
                                }

                                capaErrors.Add(capaError);
                            }
                        }
                    }

                    connection.Close();
                }

                return capaErrors;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.Get_NotIn_Capa_Error: {ex.Message}");
                FileLogging.WriteErrorLine($"UnitID: {capaError.UnitID} PackageID: {capaError.PackageID}");
                return null;
            }
        }

        public List<CapaError> Get_LastRunDate_Has_Changed()
        {
            string query = "SELECT TOP (1000) [UNITID] ,[PackageID] ,[STATUS] ,[LASTRUNDATE] ,[LOG] FROM [dbo].[V_CE_LastRunDate_Has_Changed]";
            List<CapaError> capaErrors = new List<CapaError>();
            CapaError capaError = new CapaError();

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
                                capaError = new CapaError();

                                capaError.UnitID = reader.GetInt32(0);
                                capaError.PackageID = reader.GetInt32(1);
                                capaError.Status = reader.GetString(2);
                                capaError.LastRunDate = reader.GetInt32(3);
                                capaError.Log = reader.GetString(4);

                                capaErrors.Add(capaError);
                            }
                        }
                    }
                }

                return capaErrors;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.Get_LastRunDate_Has_Changed: {ex.Message}");
                return null;
            }
        }

        public List<CapaError> Get_NotIn_UNITJOB()
        {
            string query = "SELECT TOP (1000) [UnitID],[PackageID] FROM [dbo].[V_CE_NotIN_UNITJOB]";
            List<CapaError> capaErrors = new List<CapaError>();
            CapaError capaError = new CapaError();

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
                                capaError = new CapaError();

                                capaError.UnitID = reader.GetInt32(0);
                                capaError.PackageID = reader.GetInt32(1);

                                capaErrors.Add(capaError);
                            }
                        }
                    }
                }

                return capaErrors;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.Get_NotIn_UNITJOB: {ex.Message}");
                return null;
            }
        }

        public void DeleteError(int UnitID, int PackageID)
        {
            string query = "";

            if (PackageID == 0)
            {
                query = $"DELETE FROM [Capa_Errors] WHERE UnitID = {UnitID}";
            }
            else
            {
                query = $"DELETE FROM [Capa_Errors] WHERE UnitID = {UnitID} AND PackageID = {PackageID}";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.DeleteError: {ex.Message}");
            }
        }

        public List<UnitInstallDate> Get_NotIn_UnitInstallDate()
        {
            string query = "SELECT TOP (1000) [UNITID],[VALUE] FROM [V_CE_NotIn_UnitInstallDate]";
            List<UnitInstallDate> unitInstallDates = new List<UnitInstallDate>();
            UnitInstallDate unitInstallDate = new UnitInstallDate();

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
                                unitInstallDate = new UnitInstallDate();

                                unitInstallDate.UnitID = reader.GetInt32(0);
                                unitInstallDate.VALUE = reader.GetString(1);

                                unitInstallDates.Add(unitInstallDate);
                            }
                        }
                    }
                }

                return unitInstallDates;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.Get_NotIn_UNITJOB: {ex.Message}");
                return null;
            }
        }

        public void InsertInstallDate(UnitInstallDate unitInstallDate)
        {
            string query = $"INSERT INTO [UnitInstallDate] ([UNITID], [VALUE]) VALUES ('{unitInstallDate.UnitID}','{unitInstallDate.VALUE}')";
            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.InsertError: {ex.Message}");
            }
        }

        public List<UnitInstallDate> Get_UnitInstallDate_Has_Changed()
        {
            string query = "SELECT TOP (1000) [UNITID],[VALUE] FROM [dbo].[V_CE_UnitInstallDate_Has_Changed]";
            List<UnitInstallDate> unitInstallDates = new List<UnitInstallDate>();
            UnitInstallDate unitInstallDate = new UnitInstallDate();

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
                                unitInstallDate = new UnitInstallDate();

                                unitInstallDate.UnitID = reader.GetInt32(0);
                                unitInstallDate.VALUE = reader.GetString(1);

                                unitInstallDates.Add(unitInstallDate);
                            }
                        }
                    }
                }

                return unitInstallDates;
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.Get_NotIn_UNITJOB: {ex.Message}");
                return null;
            }
        }

        public void UpdateUnitInstallDate(UnitInstallDate unitInstallDate)
        {
            string query = $"UPDATE [UnitInstallDate] SET [VALUE] = '{unitInstallDate.VALUE}' WHERE UNITID = {unitInstallDate.UnitID}";
            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                FileLogging.WriteErrorLine($"ErrorDB.InsertError: {ex.Message}");
            }
        }
    }
}
