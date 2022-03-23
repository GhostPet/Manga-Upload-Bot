using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace Manga_Upload_Bot
{
    internal class Driver
    {
        private ChromeDriver driver;

        internal Driver(bool hidden)
        {
            ChromeOptions options = new ChromeOptions();
            if (hidden) options.AddArgument("headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            this.driver = new ChromeDriver(service, options);
        }

        internal void WaitForElementExist(By by)
        {
            while (true)
            {
                try
                {
                    driver.FindElement(by);
                    return;
                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        internal void CheckAlert()
        {
            try
            {
                driver.SwitchTo().Alert().Accept();
            }
            catch (NoAlertPresentException Ex)
            {
                System.Threading.Thread.Sleep(10);
            }
        }

        internal void GoToUrl(string Url)
        {
            CheckAlert();
            driver.Navigate().GoToUrl(Url);
        }

        internal string GetTitle()
        {
            CheckAlert();
            return driver.Title;
        }

        internal void SendKeys(By by, string keys)
        {
            CheckAlert();
            driver.FindElement(by).SendKeys(keys);
        }

        internal void Click(By by)
        {
            CheckAlert();
            driver.FindElement(by).Click();
        }

        internal void GetAttribute(By by, string attribute)
        {
            CheckAlert();
            driver.FindElement(by).GetAttribute(attribute);
        }

        internal IWebElement FindElement(By by)
        {
            CheckAlert();
            return driver.FindElement(by);
        }
        internal System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            CheckAlert();
            return driver.FindElements(by);
        }

        internal void Exit()
        {
            driver.Close();
            driver.Quit();
        }
    }
}
