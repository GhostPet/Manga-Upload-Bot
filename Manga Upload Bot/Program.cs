using System;
using System.Windows.Forms;

namespace Manga_Upload_Bot
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Botu kendinize uyarlamak isterseniz buradakileri değiştirmeniz gerekebilir.
            string version = "1.2.3";
            string credentials = "turktoon-bot-333a3035d81b.json";
            string spreadsheetId = "1-71OojtQ3941aO203ZIYUMFAtAxsYoXSSPCxrvDsRpY";
            GoogleApi googleapi = new GoogleApi(version, credentials, spreadsheetId);
            if (!googleapi.IsCredentialExists) return;

            Properties.Settings settings = new Properties.Settings();
            Driver driver = new Driver(true);
            User user = new User(driver, settings);

            user.LogIn();
            if (!user.LoggedIn) Application.Run(new LoginUi(user));
            if (user.LoggedIn) Application.Run(new MainUi(user, driver, googleapi, settings));

            settings.Save();
            driver.Exit();
        }
    }
}
