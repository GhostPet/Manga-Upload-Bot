using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Manga_Upload_Bot
{
    internal class Uploader
    {

        internal static string CoverUpload(Driver driver, string filepath)
        {
            // Resim Yükleme Sayfasını Aç
            driver.GoToUrl("https://turktoon.com/wp-admin/media-new.php");

            // Siteye Yükle
            driver.FindElement(By.XPath("//input[starts-with(@id,'html5_')]")).SendKeys(filepath);

            // Yüklenen dosyayı bul
            IWebElement row = driver.FindElement(By.Id("media-items")).FindElement(By.ClassName("media-item"));

            // Yüklemesi bitti mi kontrol et.
            string imglink;
            while (true)
            {
                try
                {
                    IWebElement pinkynail = row.FindElement(By.ClassName("pinkynail"));
                    imglink = pinkynail.GetAttribute("src");
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(200);
                }
            }

            return "<img src=\"" + imglink + "\" alt = \"\" class=\"aligncenter size-full\" />";
        }

        internal static void Share(Driver driver,
                                 DataGridViewRowCollection chapters,
                                 System.ComponentModel.BackgroundWorker backgroundWorker,
                                 ref string proccess,
                                 Dictionary<string, object> settings
                                )
        {
            int total = chapters.Count;
            int startfrom = 0;

            // Dosya Ad Kontrolü
            int count0 = 0;
            foreach (DataGridViewRow chapter in chapters)
            {
                string manganame = (string)chapter.Cells[0].Value;
                string chapnum = (string)chapter.Cells[2].Value;
                string filepath = (string)chapter.Cells[5].Value;
                proccess = count0 + 1 + "/" + total + ": " + manganame + " " + chapnum + ". bölüm ismi kontrol ediliyor...";
                backgroundWorker.ReportProgress((int)Math.Round((double)(5 * count0 / total)));
                Image.CheckImageNames(filepath);
                count0++;
            }
            startfrom += 5;

            // Resimleri Kırpma
            if ((bool)settings["cut"]) {
                int count1 = 0;
                foreach (DataGridViewRow chapter in chapters)
                {
                    string manganame = (string)chapter.Cells[0].Value;
                    string chapnum = (string)chapter.Cells[2].Value;
                    string filepath = (string)chapter.Cells[5].Value;
                    proccess = count1 + 1 + "/" + total + ": " + manganame + " " + chapnum + ". bölüm kırpılıyor...";
                    backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (15 * count1 / total))));
                    if (!Image.CropImages(filepath, (int)settings["cut-height"]))
                    {
                        MessageBox.Show(manganame + " " + chapnum + ". bölüm kırpılırken bir hata meydana geldi.");
                    }
                    count1++;
                }
                startfrom += 15;
            }

            // Logo Basma
            if ((bool)settings["watermark"])
            {
                int count2 = 0;
                foreach (DataGridViewRow chapter in chapters)
                {
                    string manganame = (string)chapter.Cells[0].Value;
                    string chapnum = (string)chapter.Cells[2].Value;
                    string filepath = (string)chapter.Cells[5].Value;
                    proccess = count2 + 1 + "/" + total + ": " + manganame + " " + chapnum + ". bölüme logo basılıyor...";
                    backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (15 * count2 / total))));
                    if (!Image.WatermarkImages(filepath, (string)settings["watermark-img"], (int)settings["watermark-opacity"]))
                    {
                        MessageBox.Show(manganame + " " + chapnum + ". bölüme logo basılırken bir hata meydana geldi.");
                    }
                    count2++;
                }
                startfrom += 15;
            }

            // Boyut Küçültme
            if ((bool)settings["optimize"])
            {
                int count3 = 0;
                foreach (DataGridViewRow chapter in chapters)
                {
                    string manganame = (string)chapter.Cells[0].Value;
                    string chapnum = (string)chapter.Cells[2].Value;
                    string filepath = (string)chapter.Cells[5].Value;
                    proccess = count3 + 1 + "/" + total + ": " + manganame + " " + chapnum + ". bölüm optimize ediliyor...";
                    backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (5 * count3++ / total))));
                    if (!Image.OptimizeImages(filepath, (int)settings["optimize-quality"]))
                    {
                        MessageBox.Show(manganame + " " + chapnum + ". bölüm optimize edilirken bir hata meydana geldi.");
                    }
                }
                startfrom += 5;
            }

            // Paylaşım
            int max = 100 - startfrom;
            int a = 0;
            foreach (DataGridViewRow chapter in chapters)
            {
                string manganame = (string)chapter.Cells[0].Value;
                string mangaid = (string)chapter.Cells[1].Value;
                string chapnum = (string)chapter.Cells[2].Value;
                string covercode = (string)chapter.Cells[4].Value;
                string filepath = (string)chapter.Cells[5].Value;
                proccess = a+1 + "/" + total + ": "+ manganame + " " + chapnum + ". bölüm upload ediliyor...";
                backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (max * a / total))));

                // Resim Yükleme Sayfasını Aç
                driver.GoToUrl("https://turktoon.com/wp-admin/media-new.php");

                // Resimleri Siteye Yükle
                string[] imgdirs = Directory.GetFiles(filepath);
                foreach (string imgdir in imgdirs)
                {
                    driver.FindElement(By.XPath("//input[starts-with(@id,'html5_')]")).SendKeys(imgdir);
                }
                backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (max * (a + 0.02) / total))));

                // Tek tek yüklemesi bitti mi kontrol et ve linkini al.
                IReadOnlyCollection<IWebElement> rows = driver.FindElement(By.Id("media-items")).FindElements(By.ClassName("media-item"));
                int tempint = 0;
                List<string> imglinks = new List<string>();
                foreach (IWebElement row in rows)
                {
                    while (true)
                    {
                        try
                        {
                            IWebElement pinkynail = row.FindElement(By.ClassName("pinkynail"));
                            imglinks.Add(pinkynail.GetAttribute("src"));
                            break;
                        }
                        catch
                        {
                            System.Threading.Thread.Sleep(200);
                        }
                    }
                    tempint++;
                    double temppercent = tempint / rows.Count / 10 * 7;
                    backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (max * (a + 0.05 + temppercent) / total))));
                }

                // Yükleme Sayfasını Aç
                proccess = a + 1 + "/" + total + ": " + manganame + " " + chapnum + ". bölüm paylaşılıyor...";
                backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (max * (a + 0.80) / total))));
                driver.GoToUrl("https://turktoon.com/wp-admin/post-new.php?post_type=post&ts_add_chapter=" + mangaid);

                // Bölüm Bilgilerini Doldur
                driver.FindElement(By.CssSelector("#title")).SendKeys("#" + chapnum);
                driver.FindElement(By.Id("ero_chapter")).SendKeys(chapnum);
                try { driver.FindElement(By.ClassName("switch-html")).Click(); }
                catch { System.Threading.Thread.Sleep(10); }
                backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (max * (a + 0.85) / total))));

                // Bölüm Kapağını (Var ise) ve Bölüm Resimlerini Ekle
                if (covercode != null || covercode != "")
                {
                    driver.FindElement(By.Id("content")).SendKeys(covercode.ToString() + OpenQA.Selenium.Keys.Enter + OpenQA.Selenium.Keys.Enter);
                }
                int tempint2 = 0;
                foreach (String imglink in imglinks)
                {
                    driver.FindElement(By.Id("content")).SendKeys("<img src=\"" + imglink + "\" alt = \"\" class=\"aligncenter size-full\" />" + OpenQA.Selenium.Keys.Enter + OpenQA.Selenium.Keys.Enter);
                    tempint++;
                    double temppercent2 = tempint2 / imglinks.Count / 10;
                    backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (max * (a + 0.85 + temppercent2) / total))));
                }

                // Paylaş
                backgroundWorker.ReportProgress((int)Math.Round((double)(startfrom + (max * (a + 0.96) / total))));
                driver.Click(By.Name("publish"));
                System.Threading.Thread.Sleep(200);

                a++;
            }
        }

    }
}
