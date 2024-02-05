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
    public partial class Form3 : Form
    {
        internal GlobalSettings globalSettings = new GlobalSettings();
        internal ErrorDB errorDB = new ErrorDB();
        internal FileLogging fileLogging = new FileLogging();
        internal List<CapaError> capaErrors = new List<CapaError>();
        internal string packageName;
        internal string packageVersion;
        internal string currentErrorType;

        public Form3(string packageName, string packageVersion, string currentErrorType)
        {
            InitializeComponent();

            this.packageName = packageName;
            this.packageVersion = packageVersion;
            this.currentErrorType = currentErrorType;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = $"Capa Error Explorer - Package: {packageName} {packageVersion} - Error Type: {currentErrorType}";
            label1.Text = $"Package: {packageName} {packageVersion} - Error Type: {currentErrorType}";
            try
            {
                this.errorDB.SetConnectionString(globalSettings.SQLServer, globalSettings.ErrorExplorerSQLDB);
                capaErrors = errorDB.GetCapaErrors(packageName, packageVersion, currentErrorType);
            }
            catch (Exception ex)
            {
                fileLogging.WriteErrorLine(ex.Message);
                MessageBox.Show(ex.Message);
            }

            this.AddColumnsToGridView();
            this.AddDataToGridView();
            dataGridView1.Sort(dataGridView1.Columns["LastRunDate"], ListSortDirection.Descending);
            dataGridView1.Columns["UnitName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void AddColumnsToGridView()
        {
            dataGridView1.Columns.Add("UnitName", "Unit name");
            dataGridView1.Columns.Add("Status", "Status");
            dataGridView1.Columns.Add("LastRunDate", "Last run date");
            dataGridView1.Columns.Add("RunCount", "Run count");
            dataGridView1.Columns.Add("ErrorCount", "Error count");
            dataGridView1.Columns.Add("CancelledCount", "Cancelled count");
            dataGridView1.Columns.Add("LastErrorType", "Last error type");
        }

        private void AddDataToGridView()
        {
            foreach (CapaError capaErrorItem in this.capaErrors)
            {
                // Convert LastRunDate to DateTime
                DateTime lastRunDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                lastRunDate = lastRunDate.AddSeconds(capaErrorItem.LastRunDate).ToLocalTime();
                dataGridView1.Rows.Add(capaErrorItem.UnitName, capaErrorItem.Status, lastRunDate, capaErrorItem.RunCount, capaErrorItem.ErrorCount, capaErrorItem.CancelledCount, capaErrorItem.LastErrorType);
            }
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            //TODO: handle resize
        }
    }
}
