using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSecurity_pr2
{
    class Bill
    {
        private string type;
        private int year;
        private string month;
        private double value;

        public Bill(string billType, int year, string month, double value)
        {
            this.type = billType;
            this.year = year;
            this.month = month;
            this.value = value;
        }

        public void setType(string billType)
        {
            this.type = billType;
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
    }
}

