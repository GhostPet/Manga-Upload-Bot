using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Manga_Upload_Bot
{
    public partial class MainUi : Form
    {
        GoogleApi GoogleApi;
        Driver Driver;
        string proccess;
        string watermark;

        public MainUi(Driver d, GoogleApi api)
        {
            this.Driver = d;
            this.GoogleApi = api;
            InitializeComponent();

            GoogleApi.Checkforupdates(false);

            RefreshData();
        }

        // Menü Çubuğu
        private void çıkışYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void kullanıcıDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Restart();
        }
        private void güncelleştirmeleriDenetleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoogleApi.Checkforupdates(true);
        }
        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Turktoon Upload Bot Sürüm " + GoogleApi.version + "\n\nGhostPet tarafından yapılmıştır. Tüm hakları saklıdır.");
        }
        private void mangaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool button2state = button2.Enabled;
            button2.Enabled = false;
            dataGridView1.Enabled = false;
            label4.Text = "Ayarların kapatılması bekleniyor...";
            AddManga newMDIChild = new AddManga(this.Driver, this.GoogleApi);
            newMDIChild.ShowDialog();
            label4.Text = "";
            button2.Enabled = button2state;
            dataGridView1.Enabled = true;
        }
        private void kapakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool button2state = button2.Enabled;
            button2.Enabled = false;
            dataGridView1.Enabled = false;
            label4.Text = "Ayarların kapatılması bekleniyor...";
            AddCover newMDIChild = new AddCover(this.Driver, this.GoogleApi, dataSet1.Tables["Mangas"]);
            newMDIChild.ShowDialog();
            label4.Text = "";
            button2.Enabled = button2state;
            dataGridView1.Enabled = true;
        }

        // Bilgilendirmeler
        private void label3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Seçeceğiniz dosya tüm bölümleri içeren bir klasör olmalıdır. Tüm bölümleri içeren klasörlerin adları bölüm numaraları şeklinde olmalıdır.");
        }

        // Manga ve Kapak Listeleri
        private void button3_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void RefreshData()
        {
            dataSet1.Tables["Mangas"].Rows.Clear();
            dataSet1.Tables["Covers"].Rows.Clear();
            comboBox2.Items.Clear();
            label3.Text = "Bir manga seçin.";
            button1.Enabled = false;

            var mangas = GoogleApi.GetData("series!A2:B");
            foreach (var manga in mangas)
            {
                DataRow newrow = dataSet1.Tables["Mangas"].NewRow();
                newrow["ID"] = manga[0];
                newrow["DisplayMember"] = manga[1];
                dataSet1.Tables["Mangas"].Rows.Add(newrow);
            }

            var covers = GoogleApi.GetData("covers!A2:D");
            foreach (var cover in covers)
            {
                DataRow newrow = dataSet1.Tables["Covers"].NewRow();
                newrow["DisplayMember"] = cover[1];
                newrow["CoverEmbed"] = cover[3];
                newrow["Linked Manga"] = cover[0];
                dataSet1.Tables["Covers"].Rows.Add(newrow);
            }

            // İlk Manga Seçili Gelecek
            if ((DataRowView)comboBox1.SelectedItem != null) SetCovers();
        }

        // Manga ve Kapak Seçim
        private void SetCovers()
        {
            comboBox2.Items.Clear();
            DataRowView selectedmanga = (DataRowView)comboBox1.SelectedItem;
            foreach (DataRow row in dataSet1.Tables["Covers"].Rows)
            {
                if (selectedmanga.Row["DisplayMember"].ToString() == row["Linked Manga"].ToString())
                {
                    comboBox2.Items.Add(row["DisplayMember"].ToString());
                }
            }
            label3.Text = "Bir dosya seçin ya da sürükleyip bırakın.";
            button1.Enabled = true;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DataRowView)comboBox1.SelectedItem != null) SetCovers();
        }

        // Dosya Ekleme
        private void AddFiles(string[] files)
        {
            DataRowView selectedmanga = (DataRowView)comboBox1.SelectedItem;
            string[] covertemp = new string[] { "-", "" };

            if (comboBox2.SelectedItem != null)
            {
                foreach (DataRow coverrow in dataSet1.Tables["Covers"].Rows)
                {
                    if (coverrow["DisplayMember"].ToString() == comboBox2.SelectedItem.ToString())
                    {
                        covertemp = new string[] { coverrow["CoverEmbed"].ToString(), coverrow["DisplayMember"].ToString() };
                    }
                }
            }

            foreach (string file in files)
            {
                string[] temp = file.Split("\\".ToCharArray());
                string filename = temp[temp.Length - 1];

                dataGridView1.Rows.Add(
                    selectedmanga.Row["DisplayMember"].ToString(),
                    selectedmanga.Row["ID"].ToString(),
                    filename,
                    covertemp[1],
                    covertemp[0],
                    file
                    );
            }

            label3.Text = files.Length + " adet bölüm eklendi.";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Bir Dosya Seçin";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AddFiles(Directory.GetDirectories(dlg.SelectedPath));
                }
            }
        }
        private void MainUi_DragEnter(object sender, DragEventArgs e)
        {
            if (!button1.Enabled) return;
            label3.Text = "Klasörü buraya bırakın.";
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void MainUi_DragDrop(object sender, DragEventArgs e)
        {
            if (!button1.Enabled) return;
            string[] dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (dropfiles.Length != 1)
            {
                foreach (string filename in dropfiles)
                {
                    if (!Directory.Exists(filename))
                    {
                        label3.Text = "Canım benim, buraya SADECE klasör atacaksın.";
                        return;
                    }
                }
                AddFiles(dropfiles);
                return;
            }
            else if (Directory.Exists(dropfiles[0]))
            {
                foreach (string item in Directory.GetDirectories(dropfiles[0]))
                {
                    if (Directory.Exists(item))
                    {
                        AddFiles(Directory.GetDirectories(dropfiles[0]));
                        return;
                    }
                }
                AddFiles(dropfiles);
                return;
            }
            else
            {
                label3.Text = "Canım benim, buraya resim değil, dosya atacaksın.";
                return;
            }
        }
        private void MainUi_DragLeave(object sender, EventArgs e)
        {
            if (!button1.Enabled) return;
            label3.Text = "Bir dosya seçin ya da sürükleyip bırakın.";
        }

        // Bölüm Paylaşma
        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            ekleToolStripMenuItem.Enabled = false;
            dataGridView1.Enabled = false;
            backgroundWorker.RunWorkerAsync();
        }
        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Dictionary<string, object> settings = new Dictionary<string, object>();

            this.proccess = "Yüklemeye hazırlanılıyor...";
            backgroundWorker.ReportProgress(0);

            settings.Add("optimize", checkBox1.Checked);
            settings.Add("optimize-quality", (int)numericUpDown3.Value);

            if (this.watermark != null) settings.Add("watermark", checkBox2.Checked);
            else settings.Add("watermark", false);
            settings.Add("watermark-img", this.watermark);
            settings.Add("watermark-opacity", (int)numericUpDown2.Value);

            settings.Add("cut", checkBox3.Checked);
            settings.Add("cut-height", (int)numericUpDown1.Value);

            Uploader.Share(Driver, dataGridView1.Rows, backgroundWorker, ref this.proccess, settings);
            backgroundWorker.ReportProgress(100);
        }
        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label4.Text = this.proccess;
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
            ekleToolStripMenuItem.Enabled = true;
            dataGridView1.Enabled = true;
            dataGridView1.Rows.Clear();
            label4.Text = "Tamamlandı.";
        }

        // Yükleme Butonu Açma Kapama
        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (dataGridView1.RowCount == 0) button2.Enabled = false;
            label4.Text = proccess;
        }
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            button2.Enabled = true;
        }

        // Ayarları Açma-Kapama
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox3.Enabled;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            button4.Enabled = checkBox2.Enabled;
            numericUpDown2.Enabled = checkBox2.Enabled;
            trackBar1.Enabled = checkBox2.Enabled;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = checkBox1.Enabled;
            trackBar2.Enabled = checkBox1.Enabled;
        }

        // Logo için Dosya Seçimi
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.watermark == null)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.watermark = openFileDialog1.FileName;
                    button4.Text = "Temizle";
                }
            }
            else
            {
                this.watermark = null;
                button4.Text = "Dosya Seç";
            }
        }

        // (Numara - Çubuk) Eşitleme
        // Optimize:
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numericUpDown3.Value;
        }
        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown3.Value = trackBar2.Value;
        }
        // Logo:
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)numericUpDown2.Value;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown2.Value = trackBar1.Value;
        }

        // Ayarları Sıfırla
        private void button5_Click(object sender, EventArgs e)
        {
            // Optimize
            checkBox1.Checked = true;
            numericUpDown3.Value = 72;
            trackBar2.Value = 72;

            // Logo:
            checkBox2.Checked = true;
            this.watermark = null;
            button4.Text = "Dosya Seç";
            numericUpDown2.Value = 40;
            trackBar1.Value = 40;

            // Kırpma:
            checkBox3.Checked = true;
            numericUpDown1.Value = 5000;
            numericUpDown4.Value = 3;

        }
    }
}
