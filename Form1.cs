﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Siguri_Projekti2;
using DataSecurity_pr2.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                    string payload = ClientSide.getJwtPayload(response);

                    if(payload == "invalidSignature")
                    {
                        MessageBox.Show("Invalid Signature", "Error");
                    }
                    else
                    {
                        User user = JsonSerializer.Deserialize<User>(payload);
                        if (ClientSide.computeHash(textBox2.Text + user.getSalt()) == user.getPassword()) { 
                            this.Hide();
                            Form3 form = new Form3(payload);
                            form.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Credentials", "Error");
                        }                        
                    }
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
