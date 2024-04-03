using SQLBakVersion.Class;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SQLBakVersion
{
    public partial class Main : Form
    {
        private readonly SQLVersion sqlVersion = new SQLVersion();
        public Main()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQL Server backup files (*.bak)|*.bak";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtBakFilePath.Text = openFileDialog.FileName;
            }

            string filePath = txtBakFilePath.Text;
            string output = string.Empty;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                output = "Please select a valid .bak file.";
            else
                output = sqlVersion.GetVersion(filePath);

            lblVersion.Text = output;
            if (output.StartsWith("SQL"))
                lblVersion.ForeColor = Color.LightGreen;
            else
                lblVersion.ForeColor = Color.Red;

            if (!lblVersion.Visible)
                lblVersion.Visible = true;
        }
    }
}
