using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataSecurity_pr2
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


                typeNode.InnerText = bill.getType();
                yearNode.InnerText = bill.getYear().ToString();
                monthNode.InnerText = bill.getMonth();
                valueNode.InnerText = bill.getValue().ToString();


                billNode.AppendChild(typeNode);
                billNode.AppendChild(yearNode);
                billNode.AppendChild(monthNode);
                billNode.AppendChild(valueNode);

                rootNode.AppendChild(billNode);

                objXml.Save("../../Database/bills.fxml");
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
