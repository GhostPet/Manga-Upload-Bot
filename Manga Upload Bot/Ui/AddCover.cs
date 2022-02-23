using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Manga_Upload_Bot
{
    internal partial class AddCover : Form
    {
        Driver driver;
        GoogleApi GoogleApi;
        string uploadfile;
        string outputcode;

        internal AddCover(Driver d, GoogleApi api, DataTable mangas)
        {
            this.driver = d;
            this.GoogleApi = api;
            InitializeComponent();

            comboBox1.DataSource = mangas;
            comboBox1.DisplayMember = "DisplayMember";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.uploadfile = openFileDialog1.FileName;
                label5.Text = "Kapak siteye yükleniyor...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataRowView selectedmanga = (DataRowView)comboBox1.SelectedItem;
            IList<IList<object>> data = new List<IList<object>>();
            data.Add(new List<object>() { selectedmanga.Row["DisplayMember"].ToString(), textBox1.Text, "", textBox3.Text });
            GoogleApi.SetData(data, "covers!A1");
            this.Close();
        }

        private void AddCover_DragDrop(object sender, DragEventArgs e)
        {
            string[] uploadfile = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (uploadfile.Length == 1) {
                this.uploadfile = uploadfile[0];
                label5.Text = "Kapak siteye yükleniyor...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void AddCover_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            outputcode = Uploader.CoverUpload(driver, uploadfile);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            textBox3.Text = this.outputcode;
            label5.Text = "Kapak kodu alındı.";
        }
    }
}
