using OpenQA.Selenium;
using System;

namespace Manga_Upload_Bot
{
    internal class User
    {
        public bool LoggedIn = false;
        Properties.Settings settings;
        private Driver driver;

        internal User(Driver d, Properties.Settings s)
        {
            this.settings = s;
            this.driver = d;
        }

        internal void LogIn()
        {
            if (this.settings.UserName == null) return;
            if (this.settings.Password == null) return;
            RealLogIn(this.settings.UserName, this.settings.Password);
        }

        internal void LogIn(string UserName, string Password, bool save)
        {
            if (save)
            {
                this.settings.UserName = UserName;
                this.settings.Password = Password;
            }
            RealLogIn(UserName, Password);
        }

        internal void RealLogIn(string UserName, string Password)
        {
            driver.GoToUrl("https://turktoon.com/wp-login.php");
            driver.SendKeys(By.Id("user_login"), UserName);
            driver.SendKeys(By.Id("user_pass"), Password);
            driver.Click(By.Id("wp-submit"));
            if (driver.GetTitle() == "Başlangıç ‹ TurkToon — WordPress") this.LoggedIn = true;
        }

        internal void LogOut()
        {
            this.LoggedIn = false;
            this.settings.UserName = null;
            this.settings.Password = null;
        }
    }
}
