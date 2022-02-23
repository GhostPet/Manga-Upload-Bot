using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

        internal void GoToUrl(string Url)
        {
            driver.Navigate().GoToUrl(Url);
        }

        internal string GetTitle()
        {
            return driver.Title;
        }

        internal void SendKeys(By by, string keys)
        {
            driver.FindElement(by).SendKeys(keys);
        }

        internal void Click(By by)
        {
            driver.FindElement(by).Click();
        }

        internal void GetAttribute(By by, string attribute)
        {
            driver.FindElement(by).GetAttribute(attribute);
        }

        internal IWebElement FindElement(By by)
        {
            return driver.FindElement(by);
        }
        internal System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return driver.FindElements(by);
        }

        internal void Exit()
        {
            driver.Close();
            driver.Quit();
        }
    }
}
