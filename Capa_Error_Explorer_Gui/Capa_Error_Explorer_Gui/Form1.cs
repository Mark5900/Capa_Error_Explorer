

namespace Capa_Error_Explorer_Gui
{
    public partial class Form1 : Form
    {
        internal GlobalSettings globalSettings = new GlobalSettings();
        internal ErrorDB errorDB = new ErrorDB();
        internal FileLogging fileLogging = new FileLogging();
        internal List<CapaErrorSummary> capaErrorSummary = new List<CapaErrorSummary>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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
            dataGridView1.DataSource = capaErrorSummary;
        }
    }
}
