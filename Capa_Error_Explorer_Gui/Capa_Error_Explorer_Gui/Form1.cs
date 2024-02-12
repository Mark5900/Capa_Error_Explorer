using CapaInstaller;
using System.Collections;
using System.ComponentModel;

namespace Capa_Error_Explorer_Gui
{
    public partial class FormMain : Form
    {
        internal GlobalSettings globalSettings = new GlobalSettings();
        internal ErrorDB errorDB = new ErrorDB();
        internal FileLogging fileLogging = new FileLogging();
        internal List<CapaErrorSummary> capaErrorSummary = new List<CapaErrorSummary>();
        internal string cmpId = "All";

        //TODO: Add a way to exclude packages from the summary

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.AddDataToCombobox();

            try
            {
                this.errorDB.SetConnectionString(globalSettings.SQLServer, globalSettings.ErrorExplorerSQLDB);
                capaErrorSummary = errorDB.GetCapaErrorSummary();
            }
            catch (Exception ex)
            {
                fileLogging.WriteErrorLine(ex.Message);
                MessageBox.Show(ex.Message);
            }

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

            #region buttonRefresh
            newX = this.Width - buttonRefresh.Width - 30;
            buttonRefresh.Location = new Point(newX, buttonRefresh.Location.Y);
            #endregion

            #region comboBoxManagementPoint
            newX = buttonRefresh.Location.X - comboBoxManagementPoint.Width - 10;
            comboBoxManagementPoint.Location = new Point(newX, comboBoxManagementPoint.Location.Y);
            #endregion

            #region label1
            newX = comboBoxManagementPoint.Location.X - label1.Width - 10;
            label1.Location = new Point(newX, label1.Location.Y);
            #endregion

            #region dataGridView1
            newHeight = this.Height - dataGridView1.Location.Y - 20;
            newWidth = this.Width - 40;
            dataGridView1.Size = new Size(newWidth, newHeight);
            #endregion
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
            dataGridView1.Sort(dataGridView1.Columns["TotalErrorCount"], ListSortDirection.Descending);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string packageName = dataGridView1.Rows[e.RowIndex].Cells["PackageName"].Value.ToString();
            string packageVersion = dataGridView1.Rows[e.RowIndex].Cells["PackageVersion"].Value.ToString();

            Form2 form2 = new Form2(packageName, packageVersion, this.cmpId);
            form2.Show();
        }

        private void AddDataToCombobox()
        {
            try
            {
                SDK oSDK = new SDK();
                bool bStatus = true;

                bStatus = oSDK.SetDatabaseSettings(this.globalSettings.SQLServer, this.globalSettings.CapaSQLDB, false);
                if (bStatus == false)
                {
                    throw new Exception("CI SDK: Error setting database settings");
                }
                else
                {
                    fileLogging.WriteLine("CI SDK: Database settings set");
                }

                var aCmp = new ArrayList();
                aCmp = oSDK.GetManagementPoints();
                foreach (string cmp in aCmp)
                {
                    string[] item = cmp.Split("|");
                    comboBoxManagementPoint.Items.Add($"{item[1]} | {item[0]}");
                }

                comboBoxManagementPoint.SelectedItem = comboBoxManagementPoint.Items[0];
            }
            catch (Exception ex)
            {
                fileLogging.WriteErrorLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBoxManagementPoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedCmp = comboBoxManagementPoint.SelectedItem.ToString();
            if (selectedCmp == "All")
            {
                capaErrorSummary = errorDB.GetCapaErrorSummary();
            }
            else
            {
                string[] item = selectedCmp.Split(" | ");
                this.cmpId = item[1];
                capaErrorSummary = errorDB.GetCapaErrorSummary(cmpId);
            }

            this.AddDataToGridView();
            dataGridView1.Sort(dataGridView1.Columns["TotalErrorCount"], ListSortDirection.Descending);
        }

        private void buttonExcludePck_Click(object sender, EventArgs e)
        {
            FormExcludePackages formExcludePackages = new FormExcludePackages();
            formExcludePackages.Show();
            this.Hide();
        }
    }
}
