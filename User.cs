using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSecurity_pr2
{
    public class User
    {
        private string name;
        private string surname;
        private string email;
        private int id;
        private string password;
        private string salt;

        public User(string name,string surname, string email, int id, string password, string salt)
        {
            this.name = name;
            this.surname = surname;
            this.email = email;
            this.id = id;
            this.password = password;
            this.salt = salt;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string getSurname()
        {
            return surname;
        }

        public void setSurname(string surname)
        {
            this.surname = surname;
        }
        public string getEmail()
        {
            return email;
        }

        public void setEmail(string email)
        {
            this.email = email;
        }

        public int getId()
        {
            return id;
        }

        public string getPassword()
        {
            return password;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public string getSalt()
        {
            return salt;
        }

        public void setSalt(string salt)
        {
            this.salt = salt;
        }
    }
}
