using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Capa_Error_Explorer_Gui
{

    internal class FileLogging
    {
        private static string _path = @"C:\Program Files\Capa_Error_Explorer\Logs\";
        public FileLogging()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
                this.WriteLine("Directory created");
            }

            string[] files = Directory.GetFiles(_path);
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    if (info.CreationTime < DateTime.Now.AddDays(-30))
                    {
                        info.Delete();
                        this.WriteLine($"Log file deleted: {file}");
                    }
                }
            }
        }

        public void WriteLine(string message)
        {
            StreamWriter writer = new StreamWriter($"{_path}Capa_Error_Explorer_Gui-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.log", append: true);
            writer.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} : TRUE : {message}");
            writer.Close();
        }

        public void WriteErrorLine(string message)
        {
            StreamWriter writer = new StreamWriter($"{_path}Capa_Error_Explorer_Gui-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.log", append: true);
            writer.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} : FALSE : {message}");
            writer.Close();
        }
    }
}
