using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capa_Error_Explorer_Service
{
    internal class CapaInstallerDB : Capa
    {
        public List<CapaPackage> GetPackages()
        {
            List<CapaPackage> packages = new List<CapaPackage>();
            string query = "SELECT [JOBID], [NAME], [VERSION], [TYPE], [GUID], [CMPID] FROM [JOB]";

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
    }
}
