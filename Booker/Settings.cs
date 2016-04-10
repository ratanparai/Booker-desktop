using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Booker
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // send login request to the server client

            // testing settings
            Properties.Settings.Default.username = usernameTextBox.Text;
            Properties.Settings.Default.password = passwordTextBox.Text;
            Properties.Settings.Default.apiURL = urlTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            usernameTextBox.Text = Properties.Settings.Default.username;
            passwordTextBox.Text = Properties.Settings.Default.password;
            urlTextBox.Text = Properties.Settings.Default.apiURL;
        }
    }
}
