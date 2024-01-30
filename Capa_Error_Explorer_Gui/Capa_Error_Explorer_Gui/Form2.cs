using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capa_Error_Explorer_Gui
{
    public partial class Form2 : Form
    {
        public Form2(string packageName, string packageVersion)
        {
            InitializeComponent();

            this.Text = $"Capa Error Explorer - Package: {packageName} {packageVersion}";

            //TODO: Group by [CurrentErrorType]
        }
    }
}
