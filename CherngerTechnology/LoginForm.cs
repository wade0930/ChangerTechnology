using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CherngerTechnology
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Set to no text.
            textBox1.Text = "";
            // The password character is an asterisk.
            textBox1.PasswordChar = '*';
            // The control will allow no more than 14 characters.
            textBox1.MaxLength = 14;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "1111" || textBox1.Text.ToLower() == "3mgary")
                DialogResult = System.Windows.Forms.DialogResult.OK;
            else
                MessageBox.Show("密碼錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
    }
}
