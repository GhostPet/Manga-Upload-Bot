using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Manga_Upload_Bot
{
    public class Driver
    {
        private ChromeDriver driver;

        public Driver(bool hidden)
        {
            ChromeOptions options = new ChromeOptions();
            if (hidden) options.AddArgument("headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            this.driver = new ChromeDriver(service, options);
        }

        public void GoToUrl(string Url)
        {
            driver.Navigate().GoToUrl(Url);
        }

        public string GetTitle()
        {
            return driver.Title;
        }

        public void SendKeys(By by, string keys)
        {
            driver.FindElement(by).SendKeys(keys);
        }

        public void Click(By by)
        {
            driver.FindElement(by).Click();
        }

        public void GetAttribute(By by, string attribute)
        {
            driver.FindElement(by).GetAttribute(attribute);
        }

        public IWebElement FindElement(By by)
        {
            return driver.FindElement(by);
        }
        public System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return driver.FindElements(by);
        }

        public void Exit()
        {
            driver.Close();
            driver.Quit();
        }
    }
}
