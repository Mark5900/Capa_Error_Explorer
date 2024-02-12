
namespace Capa_Error_Explorer_Gui
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonRefresh = new Button();
            dataGridView1 = new DataGridView();
            label1 = new Label();
            comboBoxManagementPoint = new ComboBox();
            buttonExcludePck = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // buttonRefresh
            // 
            buttonRefresh.Location = new Point(1015, 12);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new Size(75, 23);
            buttonRefresh.TabIndex = 0;
            buttonRefresh.Text = "Refresh";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += buttonRefresh_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 47);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(1078, 391);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Cursor = Cursors.No;
            label1.Location = new Point(773, 15);
            label1.Name = "label1";
            label1.Size = new Size(109, 15);
            label1.TabIndex = 2;
            label1.Text = "Management Point";
            // 
            // comboBoxManagementPoint
            // 
            comboBoxManagementPoint.FormattingEnabled = true;
            comboBoxManagementPoint.Items.AddRange(new object[] { "All" });
            comboBoxManagementPoint.Location = new Point(888, 12);
            comboBoxManagementPoint.Name = "comboBoxManagementPoint";
            comboBoxManagementPoint.Size = new Size(121, 23);
            comboBoxManagementPoint.TabIndex = 3;
            comboBoxManagementPoint.SelectionChangeCommitted += comboBoxManagementPoint_SelectionChangeCommitted;
            // 
            // buttonExcludePck
            // 
            buttonExcludePck.Location = new Point(12, 12);
            buttonExcludePck.Name = "buttonExcludePck";
            buttonExcludePck.Size = new Size(118, 23);
            buttonExcludePck.TabIndex = 4;
            buttonExcludePck.Text = "Exclude package";
            buttonExcludePck.UseVisualStyleBackColor = true;
            buttonExcludePck.Click += buttonExcludePck_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1102, 450);
            Controls.Add(buttonExcludePck);
            Controls.Add(comboBoxManagementPoint);
            Controls.Add(label1);
            Controls.Add(dataGridView1);
            Controls.Add(buttonRefresh);
            ForeColor = SystemColors.ControlText;
            Name = "FormMain";
            Text = "Capa Error Explorer";
            Load += FormMain_Load;
            Resize += FormMain_Resize;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonRefresh;
        private DataGridView dataGridView1;
        private Label label1;
        private ComboBox comboBoxManagementPoint;
        private Button buttonExcludePck;
    }
}
