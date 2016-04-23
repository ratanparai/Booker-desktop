using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Booker
{
    class Booker
    {
        private HttpClient client;
        String url;
        
        // initialize booker http request
        public Booker()
        {
            this.client = new HttpClient();

            // get username and password from settings
            string username = Properties.Settings.Default.username;
            string password = Properties.Settings.Default.password;
            this.url = Properties.Settings.Default.apiURL;

            if(username == "" || password == "" || url=="")
            {
                Settings frm1 = new Settings();
                frm1.Show();
            }

            // Basic Authorization
            var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
            this.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public void searchBook(string title, string fileName, string author = "")
        {
            string partUrl = "/api/search/book?title="+ title;

            if(author != "")
            {
                partUrl += "&author=" + author;
            }

            Uri callUrl = new Uri(this.url + partUrl);

            var response = this.client.GetAsync(callUrl).Result;

            int status = (int) response.StatusCode;

            Console.WriteLine("Status code " + status);
            string saveContent = response.Content.ReadAsStringAsync().Result;
            string name = System.IO.Path.GetFileNameWithoutExtension(fileName);

            string pathname = System.IO.Path.ChangeExtension(fileName, "booker");

            Console.WriteLine(pathname);
            System.IO.File.WriteAllText(pathname, saveContent);

        }

        public void postProgress(double progressPercentage, string book_id)
        {
            Uri callUrl = new Uri(this.url + "/api/progress");

            Console.WriteLine("Book id " + book_id + " progress " + progressPercentage.ToString());

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("book_id", book_id),
                new KeyValuePair<string, string>("progress", progressPercentage.ToString())
            });

            var response = this.client.PostAsync(callUrl, content).Result;

            string saveContent = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(saveContent);
        }

        public double getProgress(string book_id)
        {
            Uri callUrl = new Uri(this.url + "/api/progress/"+book_id);
            var response = this.client.GetAsync(callUrl).Result;

            string jsonResponse = response.Content.ReadAsStringAsync().Result;

            var progress = Json.JsonParser.Deserialize(jsonResponse);

            try
            {
                Console.WriteLine("Progress found!");
                return progress.progress;
            }
            catch
            {
                return 0;
            }

        }

    }
}
