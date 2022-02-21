using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Manga_Upload_Bot
{
    public partial class AddManga : Form
    {
        GoogleApi GoogleApi;
        Driver driver;
        string mangaId;

        public AddManga(Driver d, GoogleApi api)
        {
            this.GoogleApi = api;
            this.driver = d;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            label3.Text = "ID aranıyor...";
            backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IList<IList<object>> data = new List<IList<object>>();
            data.Add(new List<object>() { textBox2.Text, textBox1.Text });
            GoogleApi.SetData(data, "series!A1");
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            driver.GoToUrl("https://turktoon.com/wp-admin/edit.php?post_type=manga");
            driver.SendKeys(By.Id("post-search-input"), textBox1.Text);
            driver.Click(By.Id("search-submit"));
            try
            {
                string mangalink = driver.FindElement(By.ClassName("add_chapter")).FindElement(By.ClassName("page-title-action")).GetAttribute("href");
                this.mangaId = mangalink.Replace("https://turktoon.com/wp-admin/post-new.php?post_type=post&ts_add_chapter=", "");
                backgroundWorker1.ReportProgress(1);
                return;
            }
            catch
            {
                backgroundWorker1.ReportProgress(0);
                return;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0) label3.Text = "ID bulunamadı.";
            else
            {
                textBox2.Text = mangaId;
                label3.Text = "ID bulundu.";
            }

        }
    }
}
