using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataSecurity_pr2;

namespace Siguri_Projekti2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form = new Form1();
            form.ShowDialog();
            this.Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // validimet
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "")
            {
                string salt = new Random().Next(100000, 1000000).ToString();
                string password = ClientSide.computeHash(textBox5.Text + salt);

                Form1.client.requestToServer("register*" + textBox1.Text + ">" + textBox2.Text + ">" + textBox3.Text + ">" + textBox4.Text + ">" + salt + ">" + password);
                String response = Form1.client.responseFromServer();
                response = Regex.Replace(response, @"[\0]+", "");

                if (response == "ERROR")
                {
                    MessageBox.Show("Error registering", "Error");
                }
                else
                {
                    MessageBox.Show("Register succsessful", "Alert");

                    this.Hide();
                    Form3 form = new Form3(ClientSide.getJwtPayload(response));
                    form.ShowDialog();
                    this.Close();
                }
            }
        }
    }
}
