using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Manga_Upload_Bot
{
    public partial class LoginUi : Form
    {
        User user;
        public LoginUi(User u)
        {
            this.user = u;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start();
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter) Start();
        }
        private void Start()
        {
            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
            {
                label4.Text = "Giriş yapılıyor...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.user.LogIn(textBox1.Text, textBox2.Text);
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (user.LoggedIn) this.Close();
            else label4.Text = "Kullanıcı adı veya şifre yanlış.";
        }
    }
}
