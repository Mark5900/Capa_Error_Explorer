using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{

    internal class FileLogging
    {
        private static string _path = @"C:\Program Files\Capa_Error_Explorer\Logs\";
        public FileLogging()
        {
            if (!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
                this.WriteLine("Directory created");
            }
        }

        public void WriteLine(string message)
        {
            StreamWriter writer = new StreamWriter($"{_path}Capa_Error_Explorer_Service-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.log", append: true);
            writer.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} : {message}");
            writer.Close();
        }
    }
}
