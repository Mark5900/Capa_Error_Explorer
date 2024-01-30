

using System.ComponentModel;

namespace Capa_Error_Explorer_Gui
{
    public partial class FormMain : Form
    {
        internal GlobalSettings globalSettings = new GlobalSettings();
        internal ErrorDB errorDB = new ErrorDB();
        internal FileLogging fileLogging = new FileLogging();
        internal List<CapaErrorSummary> capaErrorSummary = new List<CapaErrorSummary>();

        //TODO: Add a way to exclude packages from the summary

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                this.errorDB.SetConnectionString(globalSettings.SQLServer, globalSettings.ErrorExplorerSQLDB);
            }
            catch (Exception ex)
            {
                fileLogging.WriteErrorLine(ex.Message);
                MessageBox.Show(ex.Message);
            }

            capaErrorSummary = errorDB.GetCapaErrorSummary();

            this.AddColumnsToGridView();
            this.AddDataToGridView();
            dataGridView1.Sort(dataGridView1.Columns["TotalErrorCount"], ListSortDirection.Descending);
            dataGridView1.Columns["PackageName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            int newX;
            int newHeight;
            int newWidth;

            newX = this.Width - buttonRefresh.Width - 30;
            buttonRefresh.Location = new Point(newX, buttonRefresh.Location.Y);

            newHeight = this.Height - dataGridView1.Location.Y - 20;
            newWidth = this.Width - 40;
            dataGridView1.Size = new Size(newWidth, newHeight);
        }

        private void AddColumnsToGridView()
        {
            dataGridView1.Columns.Add("PackageName", "Package Name");
            dataGridView1.Columns.Add("PackageVersion", "Package Version");
            dataGridView1.Columns.Add("TotalUnits", "Total Units");
            dataGridView1.Columns.Add("StatusInstalledCount", "Status Installed Count");
            dataGridView1.Columns.Add("StatusFailedCount", "Status Failed Count");
            dataGridView1.Columns.Add("OtherStatusCount", "Other Status Count");
            dataGridView1.Columns.Add("TotalErrorCount", "Total Error Count");
            dataGridView1.Columns.Add("TotalCancelledCount", "Total Cancelled Count");
        }
        private void AddDataToGridView()
        {
            dataGridView1.Rows.Clear();
            foreach (CapaErrorSummary capaError in capaErrorSummary)
            {
                dataGridView1.Rows.Add(capaError.PackageName, capaError.PackageVersion, capaError.TotalUnits, capaError.StatusInstalledCount, capaError.StatusFailedCount, capaError.OtherStatusCount, capaError.TotalErrorCount, capaError.TotalCancelledCount);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            capaErrorSummary = errorDB.GetCapaErrorSummary();
            this.AddDataToGridView();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string packageName = dataGridView1.Rows[e.RowIndex].Cells["PackageName"].Value.ToString();
            string packageVersion = dataGridView1.Rows[e.RowIndex].Cells["PackageVersion"].Value.ToString();

            Form2 form2 = new Form2(packageName, packageVersion);
            form2.Show();
        }
    }
}
