using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataSecurity_pr2.Models;

namespace DataSecurity_pr2.Repositories
{
    class BillRepository
    {
        private static XmlDocument objXml = new XmlDocument();
        public static void addBill(Bill bill)
        {
            createBillDbIfItDoesntExists();
            objXml.Load("../../Database/bills.fxml");
            XmlElement rootNode = objXml.DocumentElement;
            XmlElement billNode = objXml.CreateElement("bill");
            XmlElement typeNode = objXml.CreateElement("type");
            XmlElement yearNode = objXml.CreateElement("year");
            XmlElement monthNode = objXml.CreateElement("month");
            XmlElement valueNode = objXml.CreateElement("value");
            XmlElement userIdNode = objXml.CreateElement("user");


            typeNode.InnerText = bill.getType();
            yearNode.InnerText = bill.getYear().ToString();
            monthNode.InnerText = bill.getMonth();
            valueNode.InnerText = bill.getValue().ToString();
            userIdNode.InnerText = bill.getUserId().ToString();

            billNode.AppendChild(typeNode);
            billNode.AppendChild(yearNode);
            billNode.AppendChild(monthNode);
            billNode.AppendChild(valueNode);
            billNode.AppendChild(userIdNode);
            rootNode.AppendChild(billNode);

            objXml.Save("../../Database/bills.fxml");
        }
        public static List<Bill> listUserBills(int userId){
            List<Bill> userBills = new List<Bill>();
            XmlNodeList billList = objXml.GetElementsByTagName("bill");
            foreach (XmlNode bill in billList)
            {
                if (string.Equals(bill["user"].InnerText, userId.ToString()))
                    userBills.Add(new Bill(bill["type"].InnerText, int.Parse(bill["year"].InnerText), bill["month"].InnerText, double.Parse(bill["value"].InnerText), int.Parse(bill["user"].InnerText)));
            }
            return userBills;
            }
        private static void createBillDbIfItDoesntExists()
        {
            if (!File.Exists("../../Database/bills.fxml"))
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter("../../Database/users.fxml", Encoding.UTF8);
                xmlTextWriter.WriteStartElement("bills");
                xmlTextWriter.Close();
            }
        }
    }
}
