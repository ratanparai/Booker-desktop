using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;

namespace Booker
{
    public partial class Settings : Form
    {

        // delegate Signature
        public delegate void delUpdateTextUi(string text);

        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // send login request to the server client

            // testing settings
            

            Task t = new Task(HTTP_GET);
            t.Start();
           
        }

        async void HTTP_GET()
        {
            string url = urlTextBox.Text + "/api/auth";
            Uri myUrl = new Uri(url);
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            HttpClient client = new HttpClient();

            // Basic Authorization
            var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            HttpResponseMessage response = await client.GetAsync(myUrl);
            HttpContent content = response.Content;

            // check status code 
            int statusCode = (int)response.StatusCode;

            delUpdateTextUi DelUpdateTextBox = new delUpdateTextUi(UpdateTextBox);

            Console.WriteLine(statusCode);

            // if login successful then save information and show message
            if (statusCode ==200)
            {
                // update ui with the help of delegate
                Properties.Settings.Default.username = usernameTextBox.Text;
                Properties.Settings.Default.password = passwordTextBox.Text;
                Properties.Settings.Default.apiURL = urlTextBox.Text;

                string result = await content.ReadAsStringAsync();
                var userInfo = Json.JsonParser.Deserialize(result);

                Properties.Settings.Default.user_id = userInfo.user.userid;
                Properties.Settings.Default.Save();

                loginStatus.BeginInvoke(DelUpdateTextBox, "Login Successfull");
            } else
            {
                loginStatus.BeginInvoke(DelUpdateTextBox, "Wrong username and password.");
            }

            // Read the string
            

            
        }

        private void UpdateTextBox(String text)
        {
            loginStatus.Text = text;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            usernameTextBox.Text = Properties.Settings.Default.username;
            passwordTextBox.Text = Properties.Settings.Default.password;
            urlTextBox.Text = Properties.Settings.Default.apiURL;
        }
    }
}
