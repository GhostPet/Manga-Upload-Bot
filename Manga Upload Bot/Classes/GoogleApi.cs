using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Manga_Upload_Bot
{
    public class GoogleApi
    {
        public string version;
        private string credentials;
        private string spreadsheetId;
        private SheetsService service;
        private string[] Scopes = { SheetsService.Scope.Spreadsheets };
        public bool IsCredentialExists = true;

        public GoogleApi(string v, string c, string s)
        {
            this.version = v;
            this.credentials = c;
            this.spreadsheetId = s;

            if (!File.Exists(credentials))
            {
                MessageBox.Show("Botu kullanmak için botun bulunduğu klasöre şu dosyayı atın: " + credentials);
                IsCredentialExists = false;
                return;
            }

            GoogleCredential credential;
            using (var stream = new FileStream(credentials, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            this.service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Turktoon Upload Bot by GhostPet",
            });
        }

        public IList<IList<Object>> GetData(string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = this.service.Spreadsheets.Values.Get(this.spreadsheetId, range);
            ValueRange response = request.Execute();
            return response.Values;
        }

        public void SetData(IList<IList<object>> data, string range)
        {
            ValueRange valueRange = new ValueRange();
            valueRange.Values = data;
            SpreadsheetsResource.ValuesResource.AppendRequest request = this.service.Spreadsheets.Values.Append(valueRange, this.spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            request.Execute();
        }

        public void Checkforupdates(bool show)
        {
            String range = "usage!C2:C2";
            SpreadsheetsResource.ValuesResource.GetRequest request = this.service.Spreadsheets.Values.Get(this.spreadsheetId, range);
            ValueRange response = request.Execute();
            String latestversion = response.Values[0][0].ToString();

            if (latestversion != this.version)
            {
                MessageBox.Show("Yeni bir sürüm mevcut. \nCihazınızdaki sürüm: " + version + "\nGüncel sürüm: " + latestversion + "\nİndirme bağlantısı: https://github.com/GhostPet/MangaUploadBot/releases");
            }
            else
            {
                if (show) MessageBox.Show("Şu anda en güncel sürümü kullanmaktasınız.");
            }
        }

    }
}
