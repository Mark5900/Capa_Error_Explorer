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
                this.FileLogging.WriteLine(ex.Message);
                return null;
            }

            return packages;
        }
    }
}
