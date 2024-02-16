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
        internal FormMain formMain;

        List<CapaErrorsExcludedPackages> capaErrorsExcludedPackages = new List<CapaErrorsExcludedPackages>();

        public FormExcludePackages(FormMain formMain)
        {
            this.formMain = formMain;
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
            dataGridView1.Sort(dataGridView1.Columns["PackageName"], ListSortDirection.Ascending);
            dataGridView1.Columns["PackageName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns["Exclude"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void AddColumnsToGridView()
        {
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.ValueType = typeof(bool);
            chk.Name = "Exclude";
            chk.HeaderText = "Exclude";

            dataGridView1.Columns.Add(chk);
            dataGridView1.Columns.Add("PackageName", "Package name");
            dataGridView1.Columns.Add("PackageVersion", "Package version");
            dataGridView1.Columns.Add("Type", "Type");

            dataGridView1.Columns["PackageName"].ReadOnly = true;
            dataGridView1.Columns["PackageVersion"].ReadOnly = true;
            dataGridView1.Columns["Type"].ReadOnly = true;
        }

        private void AddDataToGridView()
        {
            foreach (var item in capaErrorsExcludedPackages)
            {
                dataGridView1.Rows.Add(item.IsExcluded, item.PackageName, item.PackageVersion, item.TypePrettie);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region Export the data from the gridview
            List<CapaErrorsExcludedPackages> capaErrorsExcludedPackagesNew = new List<CapaErrorsExcludedPackages>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                CapaErrorsExcludedPackages capaErrorsExcludedPackages = new CapaErrorsExcludedPackages();
                capaErrorsExcludedPackages.PackageName = dataGridView1.Rows[i].Cells["PackageName"].Value.ToString();
                capaErrorsExcludedPackages.PackageVersion = dataGridView1.Rows[i].Cells["PackageVersion"].Value.ToString();
                capaErrorsExcludedPackages.IsExcluded = Convert.ToBoolean(dataGridView1.Rows[i].Cells["Exclude"].Value);
                capaErrorsExcludedPackages.TypePrettie = dataGridView1.Rows[i].Cells["Type"].Value.ToString();

                if (dataGridView1.Rows[i].Cells["Type"].Value.ToString() == "Computer")
                {
                    capaErrorsExcludedPackages.Type = 1;
                }
                else
                {
                    capaErrorsExcludedPackages.Type = 2;
                }

                capaErrorsExcludedPackagesNew.Add(capaErrorsExcludedPackages);
            }
            #endregion

            #region Find changes
            List<CapaErrorsExcludedPackages> capaErrorsExcludedPackagesChanges = new List<CapaErrorsExcludedPackages>();
            foreach (var item in capaErrorsExcludedPackagesNew)
            {
                var itemOld = capaErrorsExcludedPackages.Where(x => x.PackageName == item.PackageName && x.PackageVersion == item.PackageVersion && x.Type == item.Type).FirstOrDefault();
                if (itemOld == null)
                {
                    capaErrorsExcludedPackagesChanges.Add(item);
                }
                else
                {
                    if (itemOld.IsExcluded != item.IsExcluded)
                    {
                        capaErrorsExcludedPackagesChanges.Add(item);
                        fileLogging.WriteLine($"Package was changed: {item.PackageName} {item.PackageVersion} ({item.TypePrettie}[{item.Type}]) {item.IsExcluded}");
                    }
                }
            }
            #endregion

            #region Save changes
            try
            {
                errorDB.SaveCapaErrorsExcludedPackages(capaErrorsExcludedPackagesChanges);
            }
            catch (Exception ex)
            {
                fileLogging.WriteErrorLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
            #endregion

            string cmpId = formMain.GetCmpId();
            formMain.capaErrorSummary = errorDB.GetCapaErrorSummary(cmpId);
            formMain.RemoveAllDataFromGridView();
            formMain.AddDataToGridView();
            formMain.MakeDataGridViewPretty();
            this.formMain.Show();
            this.Close();
        }
    }
}
