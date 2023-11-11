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
            string query = $"SELECT TOP (1000) [UnitID],[PackageID],[Status],[LastRunDate],[RunCount],[CurrentErrorType],[UnitUUID],[PackageGUID],[UnitName],[PackageName],[PackageVersion],[CMPID],[TYPE],[ErrorCount],[LastErrorType],[CancelledCount],[PackageRecurrence]FROM [Capa_Errors]\r\n WHERE UnitID = {UnitID} AND PackageID = {PackageID}";

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
    }
}
