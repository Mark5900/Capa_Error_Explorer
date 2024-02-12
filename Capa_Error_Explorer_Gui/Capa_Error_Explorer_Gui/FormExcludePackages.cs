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
    public partial class FormExcludePackages : Form
    {
        internal GlobalSettings globalSettings = new GlobalSettings();
        internal ErrorDB errorDB = new ErrorDB();
        internal FileLogging fileLogging = new FileLogging();

        List<CapaErrorsExcludedPackages> capaErrorsExcludedPackages = new List<CapaErrorsExcludedPackages>();

        public FormExcludePackages()
        {
            InitializeComponent();
        }

        private void FormExcludePackages_Load(object sender, EventArgs e)
        {
            this.AddColumnsToGridView();

            try
            {
                this.errorDB.SetConnectionString(globalSettings.SQLServer, globalSettings.ErrorExplorerSQLDB);
                capaErrorsExcludedPackages = errorDB.GetCapaErrorsExcludedPackages();
            }
            catch (Exception ex)
            {
                fileLogging.WriteErrorLine(ex.Message);
                MessageBox.Show(ex.Message);
            }

            this.AddDataToGridView();
        }

        private void AddColumnsToGridView()
        {

            dataGridView1.Columns.Add("Exclude", "Exclude");
            dataGridView1.Columns.Add("PackageName", "Package name");
            dataGridView1.Columns.Add("PackageVersion", "Package version");
        }

        private void AddDataToGridView()
        {
            foreach (var item in capaErrorsExcludedPackages)
            {
                dataGridView1.Rows.Add(item.IsExcluded, item.PackageName, item.PackageVersion);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
