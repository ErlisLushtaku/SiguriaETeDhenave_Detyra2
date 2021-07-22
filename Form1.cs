using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Siguri_Projekti2;

namespace DataSecurity_pr2
{
    public partial class Form1 : Form
    {
        public static ClientSide client;
        public Form1()
        {
            InitializeComponent();
            //ServerSide.serverConn();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new ClientSide();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // validimet
            if(textBox1.Text!="" && textBox2.Text!="")
            {
                client.requestToServer("login-" + textBox1.Text + ">" + textBox2.Text);
                string response = client.responseFromServer();
                response = response.Substring(0, response.Length - 3);
                if(response == "ERROR")
                {
                    MessageBox.Show("You should sign up first!", "Alert");
                }
                else
                {
                    // Nenshkrimi, pass check

                    this.Hide();
                    Form3 form = new Form3(ClientSide.getJwtPayload(response));
                    form.ShowDialog();
                    this.Close();
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form = new Form2();
            form.ShowDialog();
            this.Close();
        }
 
    }
}
