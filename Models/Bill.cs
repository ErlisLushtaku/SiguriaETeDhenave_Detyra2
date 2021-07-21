using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSecurity_pr2.Models
{
    class Bill
    {
        private string type;
        private int year;
        private string month;
        private double value;
        private int userId;

        public Bill(string type, int year, string month, double value, int userId)
        {
            this.type = type;
            this.year = year;
            this.month = month;
            this.value = value;
            this.userId = userId;
        }

        public void setType(string type)
        {
            this.type = type;
        }

        public string getType()
        {
            return type;
        }

        public void setyear(int year)
        {
            this.year = year;
        }

        public int getYear()
        {
            return year;
        }

        public void setMonth(string month)
        {
            this.month = month;
        }

        public string getMonth()
        {
            return month;
        }

        public void setValue(double value)
        {
            this.value = value;
        }

        public double getValue()
        {
            return value;
        }

        public void setUserId(int userId)
        {
            this.userId = userId;
        }

        public double getUserId()
        {
            return userId;
        }
    }
}

