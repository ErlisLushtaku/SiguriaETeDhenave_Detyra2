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
        public static bool is_Numeric(string s)
        {
            if (Int32.TryParse(s, out int value))
            {
                return true;

            }
            return false;
        }

        public static bool is_Valid_Email(String email)
        {
            String regex = "^[a-zA-Z0-9_!#$%&'*+/=?`{|}~^.-]+@[a-zA-Z0-9.-]+$";
            //Compile regular expression to get the pattern
            Regex pattern = new Regex(regex);
            if (pattern.IsMatch(email))
            {
                return true;
            }
            return false;
        }
        public static bool is_Valid_Password(String password)
        {

            if (password.Length < 8)
            {
                return false;
            }

            int charCount = 0;
            int numCount = 0;
            for (int i = 0; i < password.Length; i++)
            {
                char ch = password[i];

                if (Char.IsDigit(ch)) numCount++;
                else if (Char.IsLetter(ch)) charCount++;
                else return false;
            }

            return (charCount >= 1 && numCount >= 1);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // validimet
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "")
            {
                if (is_Valid_Email(textBox3.Text) && is_Numeric(textBox4.Text) && is_Valid_Password(textBox5.Text))
                {
                    string salt = new Random().Next(100000, 1000000).ToString();
                    string password = ClientSide.computeHash(textBox5.Text+salt);

                    Form1.client.requestToServer("register*" + textBox1.Text + ">" + textBox2.Text + ">" + textBox3.Text + ">" + textBox4.Text + ">" + salt + ">" + password);
                    String response = Form1.client.responseFromServer();
                    response = Regex.Replace(response, @"[\0]+", "");

                    if (response == "ERROR")
                    {
                        MessageBox.Show("This email address belongs to an existing user", "Error");
                    }
                    else
                    {
                        MessageBox.Show("Register succsessful", "Alert");

                        this.Hide();
                        //Form3 form = new Form3(ClientSide.getJwtPayload(response));
                        Form1 form = new Form1();
                        form.ShowDialog();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("ID must be a number, email must be valid adn password must contain at least 8 characters(including one number and one letter)", "Error");
                }
                
            }
            else
            {
                MessageBox.Show("Please fill out the fields", "Error");
            }
        }
    }
}
