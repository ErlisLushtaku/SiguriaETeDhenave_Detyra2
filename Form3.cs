using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Security.Cryptography.X509Certificates;
using DataSecurity_pr2;
using DataSecurity_pr2.Models;
using System.Text.RegularExpressions;

namespace Siguri_Projekti2
{
    public partial class Form3 : Form
    {
        User user;
        string payload;
        public Form3(string payload)
        {
            InitializeComponent();
            // this.user = user;
            this.payload = payload;
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    
        private void button3_Click(object sender, EventArgs e)
        {
            //IJwtAlgorithm algorithm = new JWT.Algorithms.RS256Algorithm(ServerSide.certifikata);
            //IJsonSerializer serializeri = new JsonNetSerializer();
            //IBase64UrlEncoder base64i = new JwtBase64UrlEncoder();
            //IJwtValidator validator = new JwtValidator(serializeri, new UtcDateTimeProvider());
            //IJwtDecoder ide = new JwtDecoder(serializeri, validator, base64i, algorithm);
            //var jsoni = ide.Decode(ServerSide.JWTSignature(1,"a","a","a"), secret, true);
            textBox5.Text = payload;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // validimet
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox6.Text != "")
            {
                Form1.client.requestToServer("registerbill*" + textBox1.Text + ">" + textBox2.Text + ">" + textBox3.Text + ">" + textBox4.Text + ">" + textBox6.Text);
                String response = Form1.client.responseFromServer();
                response = Regex.Replace(response, @"[\0]+", "");
                if (response == "OK")
                {
                    MessageBox.Show("Bill registered", "Alert");
                }
                else
                {
                    MessageBox.Show("Bill not registered", "Error");
                }
            }
        }
    }
}
