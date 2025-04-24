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
        private bool isDragging = false;
        private Point lastLocation;

        public Main()
        {
            InitializeComponent();

            pnlDragDrop.Location = new Point(10, 37);
        }

        private void pnlTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastLocation = e.Location;
            }
        }

        private void pnlTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X,(this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void pnlTitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pnlDragDrop_Click(object sender, EventArgs e)
        {
            SelectBakFile();
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && Path.GetExtension(files[0]).ToLower() == ".bak")
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && Path.GetExtension(files[0]).ToLower() == ".bak")
            {
                ProcessBakFile(files[0]);
            }
        }

        private void SelectBakFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQL Server backup files (*.bak)|*.bak";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ProcessBakFile(openFileDialog.FileName);
            }
        }

        private void ProcessBakFile(string filePath)
        {
            lblDragDrop.Text = Path.GetFileName(filePath);

            string versionInfo = File.Exists(filePath) ? sqlVersion.GetVersion(filePath) : "File not found";
            lblVersion.Text = versionInfo;
            lblVersion.ForeColor = versionInfo.StartsWith("SQL") ? Color.LightGreen : Color.Red;
            lblVersion.Visible = true;
        }
    }
}
