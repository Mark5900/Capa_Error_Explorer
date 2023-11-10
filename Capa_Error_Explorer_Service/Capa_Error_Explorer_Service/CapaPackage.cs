using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Error_Explorer_Service
{
    internal class CapaPackage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public Guid GUID { get; set; }
        public int CMPID { get; set; }
        public int Type { get; set; }
    }
}
