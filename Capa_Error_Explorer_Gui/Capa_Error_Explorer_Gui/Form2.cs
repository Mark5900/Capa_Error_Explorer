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
        internal GlobalSettings globalSettings = new GlobalSettings();
        internal ErrorDB errorDB = new ErrorDB();
        internal FileLogging fileLogging = new FileLogging();
        internal List<CapaErrorTypeSummary> capaErrorTypeSummary = new List<CapaErrorTypeSummary>();
        internal string packageName;
        internal string packageVersion;

        public Form2(string packageName, string packageVersion)
        {
            InitializeComponent();
            this.packageName = packageName;
            this.packageVersion = packageVersion;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = $"Capa Error Explorer - Package: {packageName} {packageVersion}";
            label1.Text = $"Package: {packageName} {packageVersion}";
            try
            {
                this.errorDB.SetConnectionString(globalSettings.SQLServer, globalSettings.ErrorExplorerSQLDB);
                capaErrorTypeSummary = errorDB.GetCapaErrorTypeSummary(packageName, packageVersion);
            }
            catch (Exception ex)
            {
                fileLogging.WriteErrorLine(ex.Message);
                MessageBox.Show(ex.Message);
            }

            this.AddColumnsToGridView();
            this.AddDataToGridView();
            dataGridView1.Sort(dataGridView1.Columns["TotalErrorCount"], ListSortDirection.Descending);
            dataGridView1.Columns["CurrentErrorType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void AddColumnsToGridView()
        {
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.ValueType = typeof(bool);
            chk.Name = "Select";
            chk.HeaderText = "";

            dataGridView1.Columns.Add(chk);
            dataGridView1.Columns.Add("CurrentErrorType", "Current Error Type");
            dataGridView1.Columns.Add("Status", "Status");
            dataGridView1.Columns.Add("TotalUnits", "Total Units");
            dataGridView1.Columns.Add("TotalRunCount", "Total Run Count");
            dataGridView1.Columns.Add("TotalErrorCount", "Total Error Count");
            dataGridView1.Columns.Add("TotalCancelledCount", "Total Cancelled Count");
            dataGridView1.Columns.Add("PackageRecurrence", "Package Recurrence");
        }

        private void AddDataToGridView()
        {
            dataGridView1.Rows.Clear();
            foreach (CapaErrorTypeSummary item in capaErrorTypeSummary)
            {
                dataGridView1.Rows.Add(false, item.CurrentErrorType, item.Status, item.TotalUnits, item.TotalRunCount, item.TotalErrorCount, item.TotalCancelledCount, item.PackageRecurrence);
            }
        }

        private void buttonRerunPackage_Click(object sender, EventArgs e)
        {
            //TODO: Add code to rerun package
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            int newX;
            int newHeight;
            int newWidth;

            newX = this.Width - buttonRerunPackage.Width - 30;
            buttonRerunPackage.Location = new Point(newX, buttonRerunPackage.Location.Y);

            newHeight = this.Height - dataGridView1.Location.Y - 20;
            newWidth = this.Width - 40;
            dataGridView1.Size = new Size(newWidth, newHeight);
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // If the cell is a checkbox, then toggle the value
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Select")
            {
                dataGridView1.Rows[e.RowIndex].Cells["Select"].Value = !(bool)dataGridView1.Rows[e.RowIndex].Cells["Select"].Value;
            }
            else
            {
                // Show msgbox with details
                string currentErrorType = dataGridView1.Rows[e.RowIndex].Cells["CurrentErrorType"].Value.ToString();
                Form3 form3 = new Form3(packageName, packageVersion, currentErrorType);
                form3.Show();
            }
        }
    }
}
