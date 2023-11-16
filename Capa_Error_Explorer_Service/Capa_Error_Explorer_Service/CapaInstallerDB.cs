using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capa_Error_Explorer_Service
{
    internal class CapaInstallerDB : SQL
    {
        public string GetPackageRecurrence(int PackageID, bool bDebug)
        {
            string sRecurrence = string.Empty;
            string query = $"SELECT [RECURRENCE] FROM [SCHEDULE] WHERE [ID] = {PackageID}";

            if (bDebug)
            {
                this.FileLogging.WriteLine($"Query: {query}");
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
                                // Can be null if the job is not scheduled
                                if (reader.IsDBNull(0) == false)
                                {
                                    sRecurrence = reader.GetString(0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.FileLogging.WriteErrorLine(ex.ToString());
                return null;
            }

            return sRecurrence;
        }

        public List<CapaPackage> GetPackages(bool bDebug)
        {
            List<CapaPackage> packages = new List<CapaPackage>();
            string query = "SELECT [JOBID], [NAME], [VERSION], [TYPE], [GUID], [CMPID], [SCHEDULEID] FROM [JOB]";

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
                                CapaPackage package = new CapaPackage();
                                package.ID = reader.GetInt32(0);
                                package.Name = reader.GetString(1);
                                package.Version = reader.GetString(2);
                                package.Type = reader.GetInt16(3);
                                package.GUID = reader.GetGuid(4);
                                package.CMPID = reader.GetInt32(5);

                                // Can be null if the job is not scheduled
                                if (reader.IsDBNull(6) == false)
                                {
                                    package.Recurrence = this.GetPackageRecurrence(reader.GetInt32(6), bDebug);
                                }
                                packages.Add(package);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.FileLogging.WriteErrorLine(ex.ToString());
                return null;
            }

            return packages;
        }

        public List<CapaUnitJob> GetUnitJob(int PackageID, bool bDebug)
        {
            if (bDebug)
            {
                this.FileLogging.WriteLine($"GetUnitJob: {PackageID}");
            }

            List<CapaUnitJob> unitJobs = new List<CapaUnitJob>();
            string query = $"SELECT [UNITID], [JOBID], [STATUS], [LASTRUNDATE] FROM [UNITJOB] WHERE [JOBID] = {PackageID}";

            if (bDebug)
            {
                this.FileLogging.WriteLine($"Query: {query}");
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
                                CapaUnitJob unitJob = new CapaUnitJob();
                                unitJob.UnitID = reader.GetInt32(0);
                                unitJob.JobID = reader.GetInt32(1);
                                unitJob.Status = reader.GetString(2);

                                // Can be null if the job has never run
                                if (reader.IsDBNull(3) == false)
                                {
                                    unitJob.LastRunDate = reader.GetInt32(3);
                                }

                                unitJobs.Add(unitJob);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.FileLogging.WriteErrorLine(ex.ToString());
                return null;
            }

            return unitJobs;
        }

        public CapaUnit GetUnit(int UnitID, bool bDebug)
        {
            CapaUnit capaUnit = new CapaUnit();
            string query = $"SELECT [UNITID], [NAME], [UUID] FROM [UNIT] WHERE [UNITID] = {UnitID}";

            if (bDebug)
            {
                this.FileLogging.WriteLine($"Query: {query}");
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
                                capaUnit.UnitID = reader.GetInt32(0);
                                capaUnit.Name = reader.GetString(1);
                                capaUnit.UUID = reader.GetGuid(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.FileLogging.WriteErrorLine(ex.ToString());
                return null;
            }

            return capaUnit;
        }

        public string GetPackageLog(int UnitID, int PackageID)
        {
            string sLog;
            string query = $"SELECT [LOG] FROM [UNITJOB] WHERE [UNITID] = {UnitID} AND [JOBID] = {PackageID}";

            try
            {
                using (SqlConnection connection = new SqlConnection(this.sConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        object oLog = command.ExecuteScalar();
                        if (oLog.GetType() == typeof(DBNull))
                        {
                            sLog = string.Empty;
                        }
                        else
                        {
                            sLog = (string)oLog;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.FileLogging.WriteErrorLine(ex.ToString());
                return null;
            }

            return sLog;
        }
    }
}
