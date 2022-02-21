using OpenQA.Selenium;
using System;

namespace Manga_Upload_Bot
{
    public class User
    {
        private String UserName;
        private String Password;
        public bool LoggedIn = false;

        private Driver driver;

        public User(Driver d)
        {
            this.driver = d;
        }

        public void LogIn(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
            driver.GoToUrl("https://turktoon.com/wp-login.php");
            driver.SendKeys(By.Id("user_login"), UserName);
            driver.SendKeys(By.Id("user_pass"), Password);
            driver.Click(By.Id("wp-submit"));
            if (driver.GetTitle() == "Başlangıç ‹ TurkToon — WordPress") this.LoggedIn = true;
        }

        public void LogOut()
        {
            this.UserName = null;
            this.Password = null;
        }
    }
}
